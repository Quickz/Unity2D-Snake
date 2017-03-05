using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHead : MonoBehaviour
{

    GameObject game;
    Transform snake;
    Snake snakeLogic;
    GameLogic gameLogic;

    int score;

	// Use this for initialization
	void Start()
    {
        game = GameObject.FindWithTag("Game");
        snake = game.transform.GetChild(0);
        gameLogic = game.GetComponent<GameLogic>();
        snakeLogic = gameObject.transform.parent.GetComponent<Snake>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.name == "food")
        {
            if (snakeLogic.speed < snakeLogic.defaultSpeed)
            {
                snakeLogic.speed = snakeLogic.defaultSpeed / 2;
                gameLogic.UpScore(20);
            }
            else
                gameLogic.UpScore(10);
            gameLogic.GrowSnake(snake, "tail");
            gameLogic.RespawnFood(gameLogic.food);

        }
        else if (col.name == "highQualityFood")
        {
            gameLogic.UpScore();
            Destroy(col.gameObject);
        }
        else if (col.name == "randomBonus")
        {
            var food = col.gameObject.GetComponent<randomFood>();
            food.GetBonus(snakeLogic);
            Destroy(col.gameObject);
        }
        else if (col.name == "tail" || col.name == "head")
            gameLogic.GameOver();

    }



}
