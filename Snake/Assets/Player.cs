using UnityEngine;
using System.Collections;

public class Player : Snake
{

    public Player(Transform snake, float x, float y)
        : base(x, y)
    {
        this.snake = snake;
        head = snake.transform.GetChild(0);
    }

    public void CheckForInput()
    {
        if (Input.GetKeyDown("right") || Input.GetKeyDown("d"))
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

    }

    // moves the whole snake
    public void MoveSnake()
    {
        switch (currDir)
        {
            case "right":
                MoveTail();
                ChangeHeadPos(0.5f, 0);
                break;
            case "left":
                MoveTail();
                ChangeHeadPos(-0.5f, 0);
                break;
            case "up":
                MoveTail();
                ChangeHeadPos(0, 0.5f);
                break;
            case "down":
                MoveTail();
                ChangeHeadPos(0, -0.5f);
                break;
        }

        BendTail();
        lastDir = currDir;

    }

    // changes current snake direction
    void ChangeDir(string dir)
    {
        switch (dir)
        {
            case "right":
                if (lastDir != "left")
                    currDir = "right";
                break;
            case "left":
                if (lastDir != "right")
                    currDir = "left";
                break;
            case "up":
                if (lastDir != "down")
                    currDir = "up";
                break;
            case "down":
                if (lastDir != "up")
                    currDir = "down";
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
        pos.x = pos.x > mapX ? -mapX : pos.x < -mapX ? mapX : pos.x;
        pos.y = pos.y > mapY ? -mapY : pos.y < -mapY ? mapY : pos.y;

        head.transform.position = pos;

    }


}//end

