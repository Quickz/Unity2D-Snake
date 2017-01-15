using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class snakeLogic : MonoBehaviour
{
    bool gameOver;
    bool gamePaused;

    float time;
    float gameSpeed;
    int score;

    string direction;
    string lastDir;

    string enemyDir;
    string lastEnemyDir;

    float mapX;
    float mapY;

    Transform game;
    Transform snake;
    Transform food;

    Transform enemy;
    Transform enemyHead;

    // contains exit point of enemy snake
    Transform enemyExit;

    TextMesh scoreObj;
    GameObject gameOverNote;
    GameObject gamePausedNote;

    AStarSearch enemyPathFinder;
    List<int[]> enemyPath;

    // Use this for initialization
    void Start()
    {
        gameOver = false;
        gamePaused = false;

        time = 0;
        gameSpeed = 0.085f;
        score = 0;

        direction = "right";
        lastDir = "right";

        mapX = 10;
        mapY = 5;

        game = gameObject.transform;
        snake = game.GetChild(0);
        food = game. GetChild(1);
        scoreObj = game.GetChild(3).GetComponent<TextMesh>();
        RespawnFood();

        enemyPathFinder = new AStarSearch(GenMapGrid());
        enemy = null;
        enemyExit = null;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("r"))
            RestartGame();
        if (gameOver)
            return;
        else if (Input.GetKeyDown("p") || Input.GetKeyDown("escape"))
        {
            if (!gamePaused) PauseGame();
            else ResumeGame();
        }
        else if (gamePaused)
            return;
        else if (Input.GetKeyDown("right") || Input.GetKeyDown("d"))
        {
            ChangeDir("right");
        }
        else if (Input.GetKeyDown("left") || Input.GetKeyDown("a"))
        {
            ChangeDir("left");
        }
        else if (Input.GetKeyDown("up") || Input.GetKeyDown("w"))
        {
            ChangeDir("up");
        }
        else if (Input.GetKeyDown("down") || Input.GetKeyDown("s"))
        {
            ChangeDir("down");
        }
        // spawn enemy - temporary
        else if (Input.GetKeyDown("h"))
            CreateEnemy();
        // destroy enemy - temporary
        else if (Input.GetKeyDown("j") && enemy != null && enemyExit == null)
            CreateEnemyExit();

        time += Time.deltaTime;
        if (time >= gameSpeed)
        {

            if (enemy != null)
            {
                enemyPathFinder.ClearGrid();
                GetGridObstacles();

                if (enemyExit == null)
                {
                    MoveEnemy(food);
                }
                else
                {
                    MoveEnemy(enemyExit);
                    CheckForExit();
                }

            }

            time = 0;
            MoveSnake();
            if (CheckCollision()) return;
            CheckForFood();

        }

    }

    // saves current and last enemy direction
    void SetEnemyDir(Vector2 oldPos, Vector2 currPos)
    {
        lastEnemyDir = enemyDir;

        if (oldPos.x < currPos.x)
            enemyDir = "right";
        else if (oldPos.x > currPos.x)
            enemyDir = "left";
        else if (oldPos.y < currPos.y)
            enemyDir = "up";
        else if (oldPos.y > currPos.y)
            enemyDir = "down";

    }

    // checks if enemy has reached exit point
    void CheckForExit()
    {
        if (enemyExit.position == enemyHead.position)
        {

            // making sure to get rid of the tail
            // before getting rid of the enemy itself
            if (enemy.childCount > 1)
            {
                var child = enemy.GetChild(enemy.childCount - 1);
                Destroy(child.gameObject);
                return;
            }

            Destroy(enemy.gameObject);
            Destroy(enemyExit.gameObject);
        }
    }

    // creates an exit point for the enemy snake
    void CreateEnemyExit()
    {

        // generates position
        float[] coord = GenEnemySpwnCoord();
        if (coord == null) return;

        var exit = Instantiate(
            Resources.Load("enemyExit")    
        ) as GameObject;
        
        exit.name = "enemyExit";
        enemyExit = exit.transform;

        // setting the position
        var pos = enemyExit.position;
        pos.x = coord[0];
        pos.y = coord[1];
        enemyExit.position = pos;

    }

    // create a red enemy snake
    void CreateEnemy()
    {
        if (enemy != null) return;

        // generating random coordinates
        float[] coord = GenEnemySpwnCoord();
        if (coord == null) return;

            var enemySnake = Instantiate(
                Resources.Load("enemy")
            ) as GameObject;

        enemy = enemySnake.transform;
        enemyHead = enemy.GetChild(0);

        enemy.name = "enemy";

        // setting position
        for (int i = 0; i < enemy.childCount; i++)
        {
            Transform child = enemy.GetChild(i);
            Vector2 pos = child.position;
            pos.x = coord[0];
            pos.y = coord[1];
            child.position = pos;
        }

        // resetting direction
        enemyDir = "";
        lastEnemyDir = "";

        enemy.parent = game;
    }

    // generates position coordinates for the enemy
    float[] GenEnemySpwnCoord(int antistuck = 0)
    {
        antistuck++;
        // if during 200 attempts a valid coordinate
        // is not found null is returned
        if (antistuck > 200)
            return null;
        
        // generating random number - 0/1/2/3
        float choice = (int)Random.Range(0, 4);

        float[] coord;
        float x;
        float y;

        // right / left
        if (choice == 0 || choice == 1)
        {
            x = choice == 0 ? mapX : -mapX;
            y = (int)Random.Range(-mapY * 2, mapY * 2) / 2f;
        }
        // top / bottom 
        else
        {
            y = choice == 2 ? mapY : -mapY;
            x = (int)Random.Range(-mapX * 2, mapX * 2) / 2f;
        }

        // result
        coord = new float[] { x, y };

        // making sure the coordinate is valid
        if (!ValidEnemySpwnPos(coord))
            coord = GenEnemySpwnCoord(antistuck);
            
        return coord;
    }

    bool ValidEnemySpwnPos(float[] coord)
    {

        for (int i = 0; i < snake.childCount; i++)
        {
            var pos = snake.GetChild(i).position;

            // making sure the enemy doesn't spawn
            // inside the player snake
            if (pos.x == coord[0] && pos.y == coord[1])
                return false;
        }

        /*
            making sure the snake's head
            is not too close
        */

        var headPos = snake.GetChild(0).position;

        // snake
        float playerX = headPos.x + mapX;
        float playerY = headPos.y + mapY;
        
        // enemy
        float enemyX = coord[0] + mapX;
        float enemyY = coord[1] + mapY;

        float difX = Mathf.Abs(playerX - enemyX);
        float difY = Mathf.Abs(playerY - enemyY);

        // Making sure the enemy coordinate is at least 14 squares
        // away both horizontally and vertically before spawning
        if (difX < 7 && difY < 7 ||
            Mathf.Abs(mapX * 2 - difX) < 7 && Mathf.Abs(mapY * 2 - difY) < 7)
            return false;

        return true;
    }

    void MoveEnemy(Transform target)
    {
        var enemyPos = GetGridCoord(enemyHead.position);
        var foodPos = GetGridCoord(target.position);

        enemyPath = enemyPathFinder.run(enemyPos, foodPos);

        if (enemyPath != null)
        {
            MoveTail(enemy);

            var oldPos = enemyHead.position;
            SetPosFromGrid(enemyPath[1], enemyHead);

            SetEnemyDir(oldPos, enemyHead.position);
            BendTail(enemy, lastEnemyDir, enemyDir);

        }
    }

    void GetGridObstacles()
    {
        
        // player snake
        for (int i = 0; i < snake.childCount; i++)
        {
            int[] pos = GetGridCoord(snake.GetChild(i).position);
            enemyPathFinder.grid[pos[0], pos[1]] = 1;
        }

        // enemy tail
        for (int i = 1; i < enemy.childCount; i++)
        {
            int[] pos = GetGridCoord(enemy.GetChild(i).position);
            enemyPathFinder.grid[pos[0], pos[1]] = 1;
        }
        
    }

    // generates empty grid
    int[,] GenMapGrid()
    {
        int[,] grid = new int[(int)(mapX * 4) + 1, (int)(mapY * 4) + 1];

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
                grid[i, j] = 0;
        }

        return grid;
    }

    // converts ingame coordinate to a coordinate for the grid
    int[] GetGridCoord(Vector2 position)
    {
        int posX = (int)((position.x + mapX) * 2);
        int posY = (int)((position.y + mapY) * 2);
        return new int[] { posX, posY };
    }

    // converts grid coordinates to the world coordinates
    void SetPosFromGrid(int[] coordinates, Transform obj)
    {
        var pos = obj.position;
        pos.x = coordinates[0] / 2f - mapX;
        pos.y = coordinates[1] / 2f - mapY;
        obj.position = pos;
    }

    // changes current snake direction
    void ChangeDir(string dir)
    {
        switch (dir)
        {
            case "right":
                if (lastDir != "left")
                    direction = "right";
                break;
            case "left":
                if (lastDir != "right")
                    direction = "left";
                break;
            case "up":
                if (lastDir != "down")
                    direction = "up";
                break;
            case "down":
                if (lastDir != "up")
                    direction = "down";
                break;
        }
    }

    // moves the whole snake
    void MoveSnake()
    {
        switch (direction)
        {
            case "right":
                MoveTail(snake);
                ChangeHeadPos(0.5f, 0);
                break;
            case "left":
                MoveTail(snake);
                ChangeHeadPos(-0.5f, 0);
                break;
            case "up":
                MoveTail(snake);
                ChangeHeadPos(0, 0.5f);
                break;
            case "down":
                MoveTail(snake);
                ChangeHeadPos(0, -0.5f);
                break;
        }

        BendTail(snake, lastDir, direction);
        lastDir = direction;

    }

    // bends tail piece depending
    // last and current direction
    void BendTail(Transform snake, string lastDir, string currDir)
    {
        int sprNum = 0;

        if (lastDir == "up" && currDir == "right" ||
            lastDir == "left" && currDir == "down")
            sprNum = 1;
        else if (lastDir == "right" && currDir == "down" ||
                 lastDir == "up" && currDir == "left")
            sprNum = 2;
        else if (lastDir == "down" && currDir == "left" ||
                 lastDir == "right" && currDir == "up")
            sprNum = 3;
        else if (lastDir == "left" && currDir == "up" ||
                 lastDir == "down" && currDir == "right")
            sprNum = 4;
        else
            return;

        SetSpr(snake.GetChild(1), "square_turn_" + sprNum);
    }

    void SetSpr(Transform obj, string spritePath)
    {
        var spr = obj.GetComponent<SpriteRenderer>();
        spr.sprite = Resources.Load<Sprite>(spritePath);
    }

    // moves each tail piece based on head position
    void MoveTail(Transform snake)
    {
        int tailLength = snake.childCount - 1;

        for (int i = tailLength; i > 0; i--)
        {
            var frontPiece = snake.GetChild(i - 1);
            var backPiece = snake.GetChild(i);

            backPiece.position = frontPiece.position;

            if (i != tailLength)
                ChangeSpr(backPiece, frontPiece);

        }

    }

    // changes head position
    void ChangeHeadPos(float x = 0, float y = 0)
    {
        var head = snake.GetChild(0);
        Vector2 pos = head.transform.position;
        
        pos.x += (float)x;
        pos.y += (float)y;

        // checking for boundaries
        pos.x = pos.x > mapX ? -mapX : pos.x < -mapX ? mapX : pos.x;
        pos.y = pos.y > mapY ? -mapY : pos.y < -mapY ? mapY : pos.y;

        head.transform.position = pos;

    }

    // changes position of a specified transform
    void ChangePos(Transform obj, float x = 0, float y = 0)
    {
        Vector2 pos = obj.position;
        pos.x = x;
        pos.y = y;
        obj.position = pos;
    }

    // changes the sprites of two given pieces
    void ChangeSpr(Transform back, Transform front)
    {
        var spr1 = back.GetComponent<SpriteRenderer>();
        var spr2 = front.GetComponent<SpriteRenderer>();
        spr1.sprite = spr2.sprite;
    }

    // moves the food to a random position
    void RespawnFood()
    {

        Vector2 pos = food.position;

        // (int) rounds up the random value
        // the f makes sure the result is float
        pos.x = (int)Random.Range(-mapX * 2, mapX * 2) / 2f;
        pos.y = (int)Random.Range(-mapY * 2, mapY * 2) / 2f;

        // making sure the food doesn't spawn inside the snake
        for (int i = 0; i < snake.childCount; i++)
        {
            Vector2 tailPos = snake.GetChild(i).position;
            if (tailPos == pos)
            {
                RespawnFood();
                return;
            }
        }

        if (enemy != null)
        {
            // making sure the food doesn't spawn inside the enemy
            for (int i = 0; i < enemy.childCount; i++)
            {
                Vector2 tailPos = enemy.GetChild(i).position;
                if (tailPos == pos)
                {
                    RespawnFood();
                    return;
                }
            }
        }

        food.position = pos;

    }

    void CheckForFood()
    {
        if (food.position == snake.GetChild(0).position)
        {
            UpScore();
            GrowSnake(snake, "tail");
            RespawnFood();
        }
        else if (enemy != null && food.position == enemyHead.position)
        {
            // limitting enemy snake length
            if (enemy.childCount < 10)
                GrowSnake(enemy, "enemyTail");
            RespawnFood();
        }

    }

    // adds one square to a tail
    void GrowSnake(Transform snake, string resource)
    {
        // generating the piece
        var tail = Instantiate(
            Resources.Load(resource)
            ) as GameObject;

        // changing a few properties
        tail.name = "tail" + (snake.childCount - 1);
        tail.transform.position = snake.GetChild(0).position;
        tail.transform.parent = snake;
    }

    void UpScore(int addition = 10)
    {
        score += addition;
        scoreObj.text = "Score: " + score;
    }

    // checks if the head is inside the tail
    bool CheckCollision()
    {
        var head = snake.GetChild(0);
        for (int i = 1; i < snake.childCount; i++)
        {
            Vector2 tailPos = snake.GetChild(i).position;
            if ((Vector2)head.position == tailPos)
            {
                GameOverNotification();

                SetHeadColor(Color.red);
                gameOver = true;
                return true;
            }
        }
        return false;
    }

    void SetHeadColor(Color color)
    {
        var head = snake.GetChild(0);
        var spr = head.transform.GetComponent<SpriteRenderer>();
        spr.color = color;
    }

    void GameOverNotification()
    {
        gameOverNote = Instantiate(
            Resources.Load("gameOver")
            ) as GameObject;
        gameOverNote.name = "gameOver";
    }

    void RestartGame()
    {
        // resetting length
        for (int i = 3; i < snake.childCount; i++)
            Destroy(snake.GetChild(i).gameObject);

        // resetting position
        for (int i = 0; i < 3; i++)
            ChangePos(snake.GetChild(i), -i / 2f, 0);

        // resetting direction
        direction = "right";
        lastDir = "right";

        // resetting score
        UpScore(-score);

        // preventing last tail piece staying bent
        SetSpr(snake.GetChild(2), "square");

        SetHeadColor(Color.white);
        Destroy(gameOverNote);
        gameOver = false;
        ResumeGame();
        RespawnFood();

    }
    
    void PauseGame()
    {
        gamePausedNote = Instantiate(
            Resources.Load("gamePaused")
            ) as GameObject;
        gamePausedNote.name = "gamePaused";
        gamePaused = true;
    }

    void ResumeGame()
    {
        Destroy(gamePausedNote);
        gamePaused = false;
    }

}
