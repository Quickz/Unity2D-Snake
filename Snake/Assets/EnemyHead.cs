using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHead : MonoBehaviour
{

    Enemy enemy;
    Snake snakeLogic;

    int score;

    // Use this for initialization
    void Start()
    {
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
                Snake.GrowSnake(enemy.snake.transform, "enemyTail");
            GameLogic.RespawnFood(GameLogic.food);
        }
        else if (col.name == "highQualityFood" || col.name == "tinyFood")
            Destroy(col.gameObject);
        else if (col.name == "randomBonus")
        {
            var food = col.gameObject.GetComponent<randomFood>();
            food.GetBonus(snakeLogic);
            Destroy(col.gameObject);
        }
        else if (col.name == "tail" || col.name == "head" || col.name == "poop")
        {
            enemy.GenerateFood();
            enemy.DestroySelf();
        }

    }

}
