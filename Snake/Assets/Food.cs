﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{

    Transform food;

    GameObject game;
    GameLogic gameLogic;

    public float mapX;
    public float mapY;

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

        // moving the food to a random position
        gameLogic.RespawnFood(food);

    }

    // Update is called once per frame
    void Update()
    {

        CheckForRestart();

    }

    void CheckForRestart()
    {
        if (Input.GetKeyDown("r"))
            Destroy(gameObject);
    }

    public bool CoordIsFree(float x, float y)
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

    // checks if coordinate is for the specified snake
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

}
