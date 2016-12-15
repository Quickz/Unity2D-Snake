using UnityEngine;
using System.Collections;

public class snakeLogic : MonoBehaviour
{
    float time = 0;
    float gameSpeed = (float)0.15;

    string direction = "right";
    string lastDir = "right";

    Transform game;
    Transform snake;
    Transform food;
    Sprite tailSprite;

    // Use this for initialization
    void Start()
    {
        game = gameObject.transform;
        snake = game.GetChild(0);
        food = game.GetChild(1);
        tailSprite = snake.GetChild(0).GetComponent<SpriteRenderer>().sprite;
    }

    // Update is called once per frame
    void Update()
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

        time += Time.deltaTime;
        if (time >= gameSpeed)
        {
            time = 0;
            MoveSnake();
            CheckForFood();
        }

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
                MoveTail();
                ChangePos((float)0.5, 0);
                break;
            case "left":
                MoveTail();
                ChangePos((float)-0.5, 0);
                break;
            case "up":
                MoveTail();
                ChangePos(0, (float)0.5);
                break;
            case "down":
                MoveTail();
                ChangePos(0, (float)-0.5);
                break;
        }
    }

    // moves each tail piece based on head position
    void MoveTail()
    {
        int tailLength = snake.childCount - 1;

        for (int i = tailLength; i > 0; i--)
        {
            var frontPiece = snake.GetChild(i - 1);
            var backPiece = snake.GetChild(i);
            Vector2 pos = backPiece.transform.position;

            pos.x = frontPiece.transform.position.x;
            pos.y = frontPiece.transform.position.y;

            backPiece.transform.position = pos;
        }

    }

    // changes head position
    void ChangePos (float x = 0, float y = 0)
    {
        var head = snake.GetChild(0);
        Vector2 pos = head.transform.position;

        pos.x += (float)x;
        pos.y += (float)y;

        head.transform.position = pos;


        //  Debug.Log(head.localPosition.x);
    }

    void RespawnFood()
    {
        Debug.Log("Length:" + snake.childCount);
        Debug.Log("Food eaten!!");
        Debug.Log("Length:" + snake.childCount);
    }

    void CheckForFood ()
    {
        if (food.position == snake.GetChild(0).position)
        {

            GrowSnake();
            RespawnFood();
        }

    }

    void GrowSnake ()
    {
        var tail = new GameObject();
        tail.name = "tail" + (snake.childCount - 1);
        var sprRenderer = tail.gameObject.AddComponent<SpriteRenderer>();

        sprRenderer.sprite = tailSprite;

        // changing scale to value of 2
        Vector2 scale = tail.transform.localScale;
        scale.x = 2; scale.y = 2;
        tail.transform.localScale = scale;

        tail.transform.position = snake.GetChild(0).position;
        tail.transform.parent = snake;
    }

}
