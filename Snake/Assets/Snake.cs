using UnityEngine;
using System.Collections;

public class Snake
{

    public string currDir;
    public string lastDir;

    public float mapX;
    public float mapY;

    public GameObject snake;
    public Transform head;

	public Snake(float x, float y)
    {
        mapX = x;
        mapY = y;

        currDir = "right";
        lastDir = "right";

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

        SetSpr(snake.transform.GetChild(1), "square_turn_" + sprNum);
    }

    // moves each tail piece based on head position
    public void MoveTail()
    {
        int tailLength = snake.transform.childCount - 1;

        for (int i = tailLength; i > 0; i--)
        {
            var frontPiece = snake.transform.GetChild(i - 1);
            var backPiece = snake.transform.GetChild(i);

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

