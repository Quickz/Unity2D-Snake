using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomFood : MonoBehaviour
{

    GameLogic gameLogic;
    public Transform eatenBy;

	// Use this for initialization
	void Start()
    {
        GameObject game = GameObject.FindWithTag("Game");
        gameLogic = game.GetComponent<GameLogic>();
	}

    public void GetBonus(Snake snake)
    {
        int choice = Random.Range(0, 3);

        if (choice == 1)
            SpeedUp(snake);
        else if (choice == 2)
            SpeedDown(snake);
        else if (snake.name == "snake")
            gameLogic.UpScore();
    }

    void SpeedUp(Snake snake)
    {
        if (snake.name == "snake")
            gameLogic.ChangeScoreColor(176, 255, 179);
        snake.speed = snake.defaultSpeed / 2;
    }

    void SpeedDown(Snake snake)
    {
        if (snake.name == "snake")
            gameLogic.ChangeScoreColor(189, 169, 255);
        snake.speed = snake.defaultSpeed * 2;
    }


}
