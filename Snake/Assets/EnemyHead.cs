using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHead : MonoBehaviour
{

    GameObject game;
    Enemy enemy;
    Snake snakeLogic;
    GameLogic gameLogic;

    int score;

    // Use this for initialization
    void Start()
    {
        game = GameObject.FindWithTag("Game");
        gameLogic = game.GetComponent<GameLogic>();
        enemy = gameObject.transform.parent.GetComponent<Enemy>();
        snakeLogic = enemy.snakeLogic;

    }

    void OnTriggerEnter2D(Collider2D col)
    {

        if (col.name == "food")
        {
            if (snakeLogic.speed < snakeLogic.defaultSpeed)
                snakeLogic.speed = snakeLogic.defaultSpeed / 2;
            // limiting enemy snake length
            if (enemy.snake.transform.childCount < 10)
                gameLogic.GrowSnake(enemy.snake.transform, "enemyTail");
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
            enemy.DestroySelf();

    }



}
