using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomFood : MonoBehaviour
{

    public Transform eatenBy;

    public void GetBonus(Snake snake)
    {
        int choice = Random.Range(0, 5);

        if (choice < 2)
            SpeedUp(snake);
        else if (choice == 2)
            SpeedDown(snake);
        else if (snake.name == "snake" && choice == 3)
            GameLogic.UpScore();
        else
            ShortenSnake(snake);
    }

    void SpeedUp(Snake snake)
    {
        if (snake.name == "snake")
            GameLogic.ChangeScoreColor(176, 255, 179);
        snake.speed = snake.defaultSpeed / 2;
    }

    void SpeedDown(Snake snake)
    {
        if (snake.name == "snake")
            GameLogic.ChangeScoreColor(189, 169, 255);
        snake.speed = snake.defaultSpeed * 2;
    }

    void ShortenSnake(Snake snake)
    {
        int count = snake.snake.childCount;
        if (count > 3)
        {
            var tail = snake.snake.GetChild(count - 1);

            // generating poop
            var food = CreatePoop();
            food.transform.position = tail.position;

            Destroy(tail.gameObject);
        }
    }

    GameObject CreatePoop()
    {
        var food = Object.Instantiate(
            Resources.Load("food/poop")
        ) as GameObject;

        food.transform.parent = GameLogic.allObstacles;
        food.name = "poop";
        return food;
    }

}
