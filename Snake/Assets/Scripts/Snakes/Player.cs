using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

    public Snake snakeLogic;
    public Transform snake;
    float time;

    void Start()
    {
        GameObject game = GameObject.FindWithTag("Game");
        snake = game.transform.GetChild(0);
        snakeLogic = gameObject.GetComponent<Snake>();

        snakeLogic.currDir = "right";
        snakeLogic.lastDir = "right";
        time = 0;

    }

    void Update()
    {

        if (GameLogic.gamePaused || GameLogic.gameOver) return;

        time += Time.deltaTime;
        if (time >= snakeLogic.speed)
        {
            // restoring default color
            if (snakeLogic.speed == snakeLogic.defaultSpeed)
                GameLogic.ChangeScoreColor(255, 255, 255);
            MoveSnake();
            snakeLogic.RestoreSpeed();
            time = 0;
        }

    }

    public void CheckForInput()
    {
        if (Input.GetKeyDown("right") || Input.GetKeyDown("d"))
            ChangeDir("right");
        else if (Input.GetKeyDown("left") || Input.GetKeyDown("a"))
            ChangeDir("left");
        else if (Input.GetKeyDown("up") || Input.GetKeyDown("w"))
            ChangeDir("up");
        else if (Input.GetKeyDown("down") || Input.GetKeyDown("s"))
            ChangeDir("down");

    }

    // moves the whole snake
    public void MoveSnake()
    {

        switch (snakeLogic.currDir)
        {
            case "right":
                snakeLogic.MoveTail();
                ChangeHeadPos(0.5f, 0);
                break;
            case "left":
                snakeLogic.MoveTail();
                ChangeHeadPos(-0.5f, 0);
                break;
            case "up":
                snakeLogic.MoveTail();
                ChangeHeadPos(0, 0.5f);
                break;
            case "down":
                snakeLogic.MoveTail();
                ChangeHeadPos(0, -0.5f);
                break;
        }

        snakeLogic.BendTail();
        snakeLogic.lastDir = snakeLogic.currDir;

    }

    // changes current snake direction
    void ChangeDir(string dir)
    {
        switch (dir)
        {
            case "right":
                if (snakeLogic.lastDir != "left")
                    snakeLogic.currDir = "right";
                break;
            case "left":
                if (snakeLogic.lastDir != "right")
                    snakeLogic.currDir = "left";
                break;
            case "up":
                if (snakeLogic.lastDir != "down")
                    snakeLogic.currDir = "up";
                break;
            case "down":
                if (snakeLogic.lastDir != "up")
                    snakeLogic.currDir = "down";
                break;
        }
    }

    // changes head position
    public void ChangeHeadPos(float x = 0, float y = 0)
    {
        var head = snake.transform.GetChild(0);
        Vector2 pos = head.transform.position;

        pos.x += (float)x;
        pos.y += (float)y;

        // checking for boundaries
        pos.x = pos.x > GameLogic.mapX ? -GameLogic.mapX :
                pos.x < -GameLogic.mapX ? GameLogic.mapX : pos.x;
        pos.y = pos.y > GameLogic.mapY ? -GameLogic.mapY :
                pos.y < -GameLogic.mapY ? GameLogic.mapY : pos.y;

        head.transform.position = pos;

    }


}//end

