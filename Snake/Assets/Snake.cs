using UnityEngine;
using System.Collections;

public class Snake : MonoBehaviour
{

    public string currDir;
    public string lastDir;

    public Transform snake;
    public Transform head;
    public float defaultSpeed;
    public float speed;

	void Start()
    {
        defaultSpeed = GameLogic.gameSpeed;
        speed = defaultSpeed;
        snake = gameObject.transform;
        head = snake.GetChild(0);

    }

    /**
     * if current speed does not match
     * the default, it is gradually
     * changed towards the default value
     */
    public void RestoreSpeed()
    {
        if (speed < defaultSpeed)
        {
            speed += speed / 75;
            if (speed > defaultSpeed)
                speed = defaultSpeed;
        }
        else if (speed > defaultSpeed)
        {
            speed -= defaultSpeed / 25;
            if (speed < defaultSpeed)
                speed = defaultSpeed;
        }
    }

    // adds one square to a tail
    public static void GrowSnake(Transform snake, string resource)
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

    // bends tail piece depending
    // last and current direction
    public void BendTail()
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

    // moves each tail piece based on head position
    public void MoveTail()
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

    // changes the sprites of two given pieces
    void ChangeSpr(Transform back, Transform front)
    {
        var spr1 = back.GetComponent<SpriteRenderer>();
        var spr2 = front.GetComponent<SpriteRenderer>();
        spr1.sprite = spr2.sprite;
    }

    public void SetSpr(Transform obj, string spritePath)
    {
        var spr = obj.GetComponent<SpriteRenderer>();
        spr.sprite = Resources.Load<Sprite>(spritePath);
    }

}//end
