﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameLogic : MonoBehaviour
{
    public Player player;
    public Enemy enemy;

    public bool gameOver;
    public bool gamePaused;
    
    public float time;
    public float gameSpeed;
    int score;

    public float mapX;
    public float mapY;

    Transform game;
    Transform snake;
    public Transform food;

    public TextMesh scoreObj;
    GameObject gameOverNote;
    GameObject gamePausedNote;

    Menu menu;
    GameObject returnWarning;

    // Use this for initialization
    void Start()
    {
        gameOver = false;
        gamePaused = false;

        gameSpeed = 0.085f;
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

        menu = gameObject.GetComponent<Menu>();

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("escape"))
            ToggleReturnWarning();
        if (Input.GetKeyDown("r"))
            RestartGame();
        if (gameOver)
            return;
        else if (Input.GetKeyDown("p"))
        {
            if (!gamePaused) PauseGame();
            else
            {
                if (returnWarning != null)
                    Destroy(returnWarning);
                ResumeGame();

            }
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
            CreateFood("highQualityFood");
        else if (Input.GetKeyDown("g"))
            CreateFood("randomBonus");

        time += Time.deltaTime;
        if (time >= gameSpeed)
            time = 0;

    }

    public void ChangeScoreColor(byte r, byte g, byte b)
    {
        scoreObj.color = new Color32(r, g, b, 200);
    }

    // restores score color and game speed
    public void RestoreScoreColor()
    {
        ChangeScoreColor(255, 255, 255);
    }

    // creates/hides a warning message
    // and pauses/unpauses the game
    void ToggleReturnWarning()
    {
        if (returnWarning == null)
        {
            CreateReturnWarning();
            if (!gamePaused && !gameOver) PauseGame();
        }
        else
        {
            Destroy(returnWarning);
            if (!gameOver)
                ResumeGame();
        }
    }

    public void CreateReturnWarning()
    {
        returnWarning = Object.Instantiate(
            Resources.Load("ReturnWarning")
        ) as GameObject;

        var warning = returnWarning.transform;

        // obtaining button component
        var yesBtn = warning.GetChild(0).GetComponent<Button>();
        var noBtn = warning.GetChild(1).GetComponent<Button>();

        // adding button events
        yesBtn.onClick.AddListener(() => menu.ChangeScene("Main Menu"));
        noBtn.onClick.AddListener(() => ToggleReturnWarning());

    }

    void CreateFood(string name)
    {
        var food = Object.Instantiate(
            Resources.Load(name)
        ) as GameObject;

        food.transform.parent = game.transform;
        food.name = name;

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
        Application.LoadLevel(Application.loadedLevel);
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
