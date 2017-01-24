using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameLogic : MonoBehaviour
{
    public Player player;
    public Enemy enemy;

    public bool gameOver;
    bool gamePaused;

    // tells the game that a one
    // gamespeed cycle has passed
    public bool stepAvailable;

    public float time;
    public float gameSpeed;
    int score;

    public float mapX;
    public float mapY;

    Transform game;
    Transform snake;
    public Transform food;

    TextMesh scoreObj;
    GameObject gameOverNote;
    GameObject gamePausedNote;

    // Use this for initialization
    void Start()
    {
        gameOver = false;
        gamePaused = false;

        gameSpeed = 0.085f;
        stepAvailable = false;
        time = 0;
        score = 0;

        mapX = 10;
        mapY = 5;

        game = gameObject.transform;
        snake = game.GetChild(0);

        player = snake.GetComponent<Player>();

        food = game. GetChild(1);
        scoreObj = game.GetChild(3).GetComponent<TextMesh>();
        RespawnFood(food);

        enemy = null;
        
    }

    // Update is called once per frame
    void Update()
    {

        stepAvailable = false;

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
            CreateEnemy();
        // destroy enemy - temporary
        else if (Input.GetKeyDown("j") && enemy != null && enemy.exit == null)
            enemy.CreateEnemyExit();
        else if (Input.GetKeyDown("f"))
            CreateHighFood();

        time += Time.deltaTime;
        if (time >= gameSpeed)
        {
            time = 0;
            stepAvailable = true;
        }

    }

    void CreateHighFood()
    {
        var food = Object.Instantiate(
            Resources.Load("highQualityFood")
        ) as GameObject;

        food.transform.parent = game.transform;
        food.name = "highQualityFood";

    }

    // creates a red enemy snake
    public void CreateEnemy()
    {
        var enemyObj = Object.Instantiate(
            Resources.Load("enemy")
        ) as GameObject;

        enemyObj.transform.parent = game.transform;
        enemy = enemyObj.GetComponent<Enemy>();

    }

    // changes position of a specified transform
    public void ChangePos(Transform obj, float x = 0, float y = 0)
    {
        Vector2 pos = obj.position;
        pos.x = x;
        pos.y = y;
        obj.position = pos;
    }

    // moves the food to a random position
    public void RespawnFood(Transform food)
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
                RespawnFood(food);
                return;
            }
        }

        if (enemy != null)
        {
            // making sure the food doesn't spawn inside the enemy
            for (int i = 0; i < enemy.snake.childCount; i++)
            {
                Vector2 tailPos = enemy.snake.GetChild(i).position;
                if (tailPos == pos)
                {
                    RespawnFood(food);
                    return;
                }
            }
        }

        food.position = pos;

    }

    // adds one square to a tail
    public void GrowSnake(Transform snake, string resource)
    {
        // generating the piece
        var tail = Instantiate(
            Resources.Load(resource)
            ) as GameObject;

        // changing a few properties
        tail.name = "tail";
        tail.transform.position = snake.GetChild(1).position;
        tail.transform.parent = snake;
    }

    public void UpScore(int addition = 10)
    {
        score += addition;
        scoreObj.text = "Score: " + score;
    }

    // checks if the head is inside the tail
    public void GameOver()
    {
        GameOverNotification();
        SetHeadColor(Color.red);
        gameOver = true;

        time = 0;

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
        player.snakeLogic.currDir = "right";
        player.snakeLogic.lastDir = "right";

        // resetting score
        UpScore(-score);

        // preventing last tail piece staying bent
        player.snakeLogic.SetSpr(snake.GetChild(2), "square");

        SetHeadColor(Color.white);
        Destroy(gameOverNote);
        gameOver = false;
        ResumeGame();
        RespawnFood(food);

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
