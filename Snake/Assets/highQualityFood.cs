using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class highQualityFood : MonoBehaviour
{

    Transform food;

    GameObject game;
    GameLogic gameLogic;

    float mapX;
    float mapY;

    // counts game steps
    int gameSteps;

    int steps;
    int stepDir;

	// Use this for initialization
	void Start()
    {

        game = GameObject.FindWithTag("Game");
        gameLogic = game.GetComponent<GameLogic>();
        food = gameObject.transform;

        mapX = gameLogic.mapX;
        mapY = gameLogic.mapY;

        stepDir = 1;
        steps = 0;
        gameSteps = 0;

        // moving the food to a random position
        gameLogic.RespawnFood(food);

	}
	
	// Update is called once per frame
	void Update()
    {

        if (stepAvailable())
        {
            Move(0.5f * stepDir, 0);

        }

	}

    void Move(float x, float y)
    {
        float newX = food.position.x + x;
        float newY = food.position.y + y;

        // checking if it's going outside the bounds
        if (NewCoordOutOfBounds(newX, newY))
        {
            steps = stepDir == 1 ? 5 : 0;
            Move(-x, -y);
            return;
        }

        gameLogic.ChangePos(food, newX, newY);
        steps += stepDir;

        // changing direction of movement
        if (steps > 5 || steps < 0)
            stepDir = -stepDir;

    }

    bool NewCoordOutOfBounds(float newX, float newY)
    {
        return newX > mapX || newX < -mapX ||
               newY > mapY || newY < -mapY;
    }

    // tells the food if it's allowed to move
    // needed to be able to move every 2 game steps
    // instead of 1
    bool stepAvailable()
    {
        if (gameLogic.stepAvailable)
            gameSteps++;

        if (gameSteps > 2)
        {
            gameSteps = 0;
            return true;
        }

        return false;
    }

}
