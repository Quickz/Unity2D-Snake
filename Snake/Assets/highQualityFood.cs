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

    // horizontal/ vertical movement
    int movX;
    int movY;


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

        SetMovementAxis();

    }
	
	// Update is called once per frame
	void Update()
    {

        CheckForRestart();

        if (stepAvailable())
            MoveFood();

	}

    void CheckForRestart()
    {
        if (Input.GetKeyDown("r"))
            Destroy(gameObject);
    }

    void SetMovementAxis()
    {
        int axis = Random.Range(0, 2);
        movX = axis == 1 ? 1 : 0;
        movY = axis != 1 ? 1 : 0;
    }

    bool CoordIsFree(float x, float y)
    {
        float posX = food.position.x + x;
        float posY = food.position.y + y;

        var player = gameLogic.player.snake;

        bool playerFree = CheckSnakeCoord(player, posX, posY);
        bool enemyFree = true;

        if (gameLogic.enemy != null)
        {
            var enemy = gameLogic.enemy.snake;
            enemyFree = CheckSnakeCoord(enemy, posX, posY);
        }

        return playerFree && enemyFree;
    }

    // checks if coordinate is for for the specified snake
    bool CheckSnakeCoord(Transform snake, float x, float y)
    {
        for (int i = 0; i < snake.childCount; i++)
        {
            var pos = snake.GetChild(i).position;
            if (pos.x == x && pos.y == y)
                return false;
        }
        return true;
    }

    void MoveFood()
    {
        float x = 0.5f * stepDir * movX;
        float y = 0.5f * stepDir * movY;

        // checking for obstacles
        if (CoordIsFree(x, y))
            Move(x, y);
        // trying to go to opposite direction
        else if (steps <= 5 && steps >= 0 && CoordIsFree(-x, -y))
        {
            stepDir = -stepDir;
            Move(-x, -y);
            return;
        }

    }

    void Move(float x, float y)
    {
        float newX = food.position.x + x;
        float newY = food.position.y + y;

        steps += stepDir;

        // checking if it's going outside the bounds
        if (NewCoordOutOfBounds(newX, newY))
        {
            steps = stepDir == 1 ? 5 : 0;
            Move(-x, -y);
            return;
        }

        gameLogic.ChangePos(food, newX, newY);

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
    // needed to be able to move every 3 game steps
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
