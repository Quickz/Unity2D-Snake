using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class highQualityFood : MonoBehaviour
{

    Food foodLogic;
    Transform food;
    float time;

    int steps;
    int stepDir;

    // horizontal/ vertical movement
    int movX;
    int movY;

	// Use this for initialization
	void Start()
    {
        foodLogic = gameObject.GetComponent<Food>();
        food = gameObject.transform;

        stepDir = 1;
        steps = 0;

        // moving the food to a random position
        GameLogic.RespawnFood(food);

        SetMovementAxis();

    }

    // Update is called once per frame
    void Update()
    {
        if (GameLogic.gamePaused || GameLogic.gameOver) return;
        time += Time.deltaTime;
        if (time >= GameLogic.gameSpeed * 3)
        {
            MoveFood();
            time = 0;
        }
    }

    void SetMovementAxis()
    {
        int axis = Random.Range(0, 2);
        movX = axis == 1 ? 1 : 0;
        movY = axis != 1 ? 1 : 0;
    }

    void MoveFood()
    {
        float x = 0.5f * stepDir * movX;
        float y = 0.5f * stepDir * movY;

        // checking for obstacles
        if (foodLogic.CoordIsFree(x, y))
            Move(x, y);
        // trying to go to opposite direction
        else if (steps <= 5 && steps >= 0 && foodLogic.CoordIsFree(-x, -y))
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

        ChangePos(food, newX, newY);

        // changing direction of movement
        if (steps > 5 || steps < 0)
            stepDir = -stepDir;

    }

    // changes position of a specified transform
    void ChangePos(Transform obj, float x = 0, float y = 0)
    {
        Vector2 pos = obj.position;
        pos.x = x;
        pos.y = y;
        obj.position = pos;
    }

    bool NewCoordOutOfBounds(float newX, float newY)
    {
        return newX > foodLogic.mapX || newX < -foodLogic.mapX ||
               newY > foodLogic.mapY || newY < -foodLogic.mapY;
    }

}
