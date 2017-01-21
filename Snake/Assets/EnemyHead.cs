using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHead : MonoBehaviour
{

    GameObject game;
    Enemy enemy;
    GameLogic gameLogic;

    int score;

    // Use this for initialization
    void Start()
    {
        game = GameObject.FindWithTag("Game");
        gameLogic = game.GetComponent<GameLogic>();
        enemy = gameObject.transform.parent.GetComponent<Enemy>();

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.name == "food")
        {
            // limiting enemy snake length
            if (enemy.snake.transform.childCount < 10)
                gameLogic.GrowSnake(enemy.snake.transform, "enemyTail");
            gameLogic.RespawnFood(gameLogic.food);
        }
    }



}
