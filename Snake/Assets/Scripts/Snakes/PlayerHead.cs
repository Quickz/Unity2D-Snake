using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHead : MonoBehaviour
{

    GameObject game;
    Transform snake;
    Snake snakeLogic;

    int score;

	// Use this for initialization
	void Start()
    {
        game = GameObject.FindWithTag("Game");
        snake = game.transform.GetChild(0);
        snakeLogic = gameObject.transform.parent.GetComponent<Snake>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.name == "food")
        {
            if (snakeLogic.speed < snakeLogic.defaultSpeed)
            {
                snakeLogic.speed = snakeLogic.defaultSpeed / 2;
                GameLogic.UpScore(20);
            }
            else
                GameLogic.UpScore(10);
            Snake.GrowSnake(snake, "tail");
            GameLogic.RespawnFood(GameLogic.food);
            RandomOccurrence();
            GameLogic.PlaySound("eat");
        }
        else if (col.name == "highQualityFood")
        {
            // the more time is the, the more score is obtained
            float time = col.GetComponent<timeLimit>().timeLeft;
            GameLogic.UpScore((int)Mathf.Round(time) * 10);
            Destroy(col.gameObject);
            GameLogic.PlaySound("eat");
        }
        else if (col.name == "randomBonus")
        {
            var food = col.gameObject.GetComponent<randomFood>();
            food.GetBonus(snakeLogic);
            Destroy(col.gameObject);
            GameLogic.PlaySound("eat");
        }
        else if (col.name == "tinyFood")
        {
            GameLogic.UpScore(5);
            Destroy(col.gameObject);
            GameLogic.PlaySound("eat");
        }
        else if (col.name == "tail" || col.name == "head" || col.name == "poop")
        {
            GameLogic.GameOver();
            GameLogic.PlaySound("hit");
        }

    }

    // spawns a piece of food/enemy or does nothing
    void RandomOccurrence()
    {
        int choice = Random.Range(0, 11);

        if (choice < 4)
            GameLogic.CreateFood("highQualityFood");
        else if (choice < 8)
            GameLogic.CreateFood("randomBonus");
        else if (choice < 10)
            GameLogic.CreateEnemy();
    }

}
