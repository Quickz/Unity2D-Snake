using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class snakeLogic : MonoBehaviour
{
    bool gameOver = false;
    bool gamePaused = false;

    float time = 0;
    float gameSpeed = 0.085f;
    int score = 0;

    string direction = "right";
    string lastDir = "right";

    float mapX = 10;
    float mapY = 5;

    Transform game;
    Transform snake;
    Transform food;
    Transform enemy;
    Transform enemyHead;

    TextMesh scoreObj;
    GameObject gameOverNote;
    GameObject gamePausedNote;

    AStarSearch enemyPathFinder;
    List<int[]> enemyPath;

    // Use this for initialization
    void Start()
    {
        game = gameObject.transform;
        snake = game.GetChild(0);
        food = game. GetChild(1);
        enemy = game.GetChild(4);
        enemyHead = enemy.GetChild(0);
        scoreObj = game.GetChild(3).GetComponent<TextMesh>();
        RespawnFood();

        enemyPathFinder = new AStarSearch(GenMapGrid());

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

        time += Time.deltaTime;
        if (time >= gameSpeed)
        {

            enemyPathFinder.ClearGrid();
            GetGridObstacles();
            MoveEnemy();

            time = 0;
            MoveSnake();
            if (CheckCollision()) return;
            CheckForFood();
        }

    }

    void MoveEnemy()
    {
        var enemyPos = GetGridCoord(enemyHead.position);
        var foodPos = GetGridCoord(food.position);

        enemyPath = enemyPathFinder.run(enemyPos, foodPos);

        if (enemyPath != null)
        {
            MoveTail(enemy);
            SetPosFromGrid(enemyPath[1], enemyHead);
        }
    }

    void GetGridObstacles()
    {

       // for (int i = 1; i < enemy.childCount; i++)
      //  {
            int[] pos = GetGridCoord(enemy.GetChild(0).position);
            enemyPathFinder.grid[pos[0], pos[1]] = 1;
       // }
        
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
        lastDir = direction;
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
    }

    // moves each tail piece based on head position
    void MoveTail(Transform snake)
    {
        int tailLength = snake.childCount - 1;

        for (int i = tailLength; i > 0; i--)
        {
            var frontPiece = snake.GetChild(i - 1).position;
            var backPiece = snake.GetChild(i);
            ChangePos(backPiece, frontPiece.x, frontPiece.y);
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
        else if (food.position == enemyHead.position)
        {
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
