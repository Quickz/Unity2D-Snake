using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class snakeLogic : MonoBehaviour
{
    bool gameOver = false;

    float time = 0;
    float gameSpeed = (float)0.085;
    int score = 0;

    string direction = "right";
    string lastDir = "right";

    float mapX = (float)10;
    float mapY = (float)5;

    Transform game;
    Transform snake;
    Transform food;
    Transform background;
    TextMesh scoreObj;
    Sprite tailSprite;
    GameObject gameOverNote;

    // Use this for initialization
    void Start()
    {
        game = gameObject.transform;
        snake = game.GetChild(0);
        food = game.GetChild(1);
        tailSprite = snake.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        scoreObj = game.GetChild(3).GetComponent<TextMesh>();
        background = game.GetChild(2);

        RespawnFood();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("r"))
            RestartGame();
        else if (gameOver)
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
            time = 0;
            MoveSnake();
            if (CheckCollision()) return;
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
    void ChangePos(float x = 0, float y = 0)
    {
        var head = snake.GetChild(0);
        Vector2 pos = head.transform.position;
        
        pos.x += (float)x;
        pos.y += (float)y;

        // checking for boundaries
        pos.x = pos.x > mapX ? -mapX : pos.x < -mapX ? mapX : pos.x;
        pos.y = pos.y > mapY ? -mapY : pos.y < -mapY ? mapY : pos.y;

        head.transform.position = pos;

        
        // Debug.Log("X:" + head.localPosition.x);
        // Debug.Log("Y:" + head.localPosition.y);
    }

    // moves the food to a random position
    void RespawnFood()
    {// 11 4.5

        Vector2 pos = food.position;

        pos.x = (float)Random.Range((int)-mapX * 2, (int)mapX * 2) / 2;
        pos.y = (float)Random.Range((int)-mapY * 2, (int)mapY * 2) / 2;
        //Debug.Log(pos.x + " " + pos.y);

        // making sure the food doesn't spawn inside the snake
        for (int i = 0; i < snake.childCount; i++)
        {
            Vector2 tailPos = snake.GetChild(i).position;
            if (tailPos == pos)
            {
                //Debug.Log("food in tail!!!!!!!!!!!!");
                RespawnFood();
                return;
            }
        }

        food.position = pos;

        //Debug.Log("Food eaten!!");
    }

    void CheckForFood()
    {
        if (food.position == snake.GetChild(0).position)
        {
            UpScore();
            GrowSnake();
            RespawnFood();
        }

    }

    void GrowSnake()
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
        gameOverNote = new GameObject();
        gameOverNote.name = "gameOver";

        // changing scale
        var scale = gameOverNote.transform.localScale;
        scale.x = 0.5f;
        scale.y = 0.5f;
        gameOverNote.transform.localScale = scale;

        TextMesh txt = gameOverNote.AddComponent<TextMesh>();
        txt.text = "Game Over";
        
        // making sure the text is in the front
        txt.offsetZ = -1;

        // changing it's anchor point to center
        txt.anchor = TextAnchor.MiddleCenter;

        txt.fontSize = 28;
        
        // assigning built in font
        txt.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;

        txt.color = Color.red;

        // moving position to center
        Vector2 pos = txt.transform.localPosition;
        pos.x = 0;
        pos.y = 0;
        txt.transform.localPosition = pos;

    }

    void RestartGame()
    {
        // resetting length
        for (int i = 3; i < snake.childCount; i++)
        {
            Destroy(snake.GetChild(i).gameObject);
        }

        // resetting position
        for (int i = 0; i < 3; i++)
        {
            Vector2 pos = snake.GetChild(i).position;
            pos.x = (float)-i / 2;
            pos.y = 0;
            snake.GetChild(i).position = pos;
        }

        // resetting direction
        direction = "right";
        lastDir = "right";

        // resetting score
        UpScore(-score);

        SetHeadColor(Color.white);
        Destroy(gameOverNote);
        gameOver = false;
        RespawnFood();
    }

}
