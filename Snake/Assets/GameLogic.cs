using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameLogic : MonoBehaviour
{
    Player player;
    Enemy enemy;

    bool gameOver;
    bool gamePaused;

    float time;
    float gameSpeed;
    int score;

    float mapX;
    float mapY;

    Transform game;
    Transform snake;
    Transform food;

    TextMesh scoreObj;
    GameObject gameOverNote;
    GameObject gamePausedNote;

    // Use this for initialization
    void Start()
    {
        gameOver = false;
        gamePaused = false;

        time = 0;
        gameSpeed = 0.085f;
        score = 0;

        mapX = 10;
        mapY = 5;

        game = gameObject.transform;
        
        snake = game.GetChild(0);
        player = new Player(snake, mapX, mapY);

        food = game. GetChild(1);
        scoreObj = game.GetChild(3).GetComponent<TextMesh>();
        RespawnFood();

        enemy = null;

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
        else
            player.CheckForInput();

        // spawn enemy - temporary
        if (Input.GetKeyDown("h") && enemy == null)
            enemy = new Enemy(game, mapX, mapY);
        // destroy enemy - temporary
        else if (Input.GetKeyDown("j") && enemy != null && enemy.exit == null)
            enemy.CreateEnemyExit();

        time += Time.deltaTime;
        if (time >= gameSpeed)
        {

            if (enemy != null)
            {
                enemy.pathFinder.ClearGrid();
                enemy.GetGridObstacles();

                if (enemy.exit == null)
                {
                    enemy.MoveSnake(food);
                }
                else
                {
                    enemy.MoveSnake(enemy.exit);

                    if (enemy.CheckForExit())
                        enemy = null;

                }

            }

            time = 0;
            player.MoveSnake();
            if (CheckCollision()) return;
            CheckForFood();

        }

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

        if (enemy != null)
        {
            // making sure the food doesn't spawn inside the enemy
            for (int i = 0; i < enemy.snake.transform.childCount; i++)
            {
                Vector2 tailPos = enemy.snake.transform.GetChild(i).position;
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
        else if (enemy != null && food.position == enemy.head.position)
        {
            // limiting enemy snake length
            if (enemy.snake.transform.childCount < 10)
                GrowSnake(enemy.snake.transform, "enemyTail");
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
        player.currDir = "right";
        player.lastDir = "right";

        // resetting score
        UpScore(-score);

        // preventing last tail piece staying bent
        player.SetSpr(snake.GetChild(2), "square");

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
