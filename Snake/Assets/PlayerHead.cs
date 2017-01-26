using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHead : MonoBehaviour
{

    GameObject game;
    Transform snake;
    GameLogic gameLogic;

    int score;

	// Use this for initialization
	void Start()
    {
        game = GameObject.FindWithTag("Game");
        snake = game.transform.GetChild(0);
        gameLogic = game.GetComponent<GameLogic>();

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.name == "food")
        {
            gameLogic.UpScore();
            gameLogic.GrowSnake(snake, "tail");
            gameLogic.RespawnFood(gameLogic.food);

        }
        else if (col.name == "highQualityFood")
        {
            gameLogic.UpScore();
            Destroy(col.gameObject);
        }
        else if (col.name == "tail" || col.name == "head")
            gameLogic.GameOver();

    }



}
