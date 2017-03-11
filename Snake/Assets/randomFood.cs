using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomFood : MonoBehaviour
{

    public Transform eatenBy;

	// Use this for initialization
	void Start()
    {

	}

    public void GetBonus(Snake snake)
    {
        int choice = Random.Range(0, 4);

        if (choice < 2)
            SpeedUp(snake);
        else if (choice == 2)
            SpeedDown(snake);
        else if (snake.name == "snake")
            GameLogic.UpScore();
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


}
