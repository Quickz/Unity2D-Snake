using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{

    Transform food;
    public float mapX;
    public float mapY;

    // horizontal/ vertical movement
    int movX;
    int movY;

    // Use this for initialization
    void Start()
    {
        food = gameObject.transform;
        mapX = GameLogic.mapX;
        mapY = GameLogic.mapY;

        // moving the food to a random position
        GameLogic.RespawnFood(food);

    }

    // checks for any obstacles
    public bool CoordIsFree(float x, float y)
    {
        float posX = food.position.x + x;
        float posY = food.position.y + y;
        
        var player = GameLogic.player.snake;

        bool playerFree = CheckContainerCoord(player, posX, posY);
        bool obstacleFree = CheckContainerCoord(
                GameLogic.allObstacles, posX, posY
            );
        bool enemyFree = true;

        if (GameLogic.enemy != null)
        {
            var enemy = GameLogic.enemy.snake;
            enemyFree = CheckContainerCoord(enemy, posX, posY);
        }

        return playerFree && enemyFree && obstacleFree;
    }

    // checks if coordinate is for the specified transform
    bool CheckContainerCoord(Transform content, float x, float y)
    {
        for (int i = 0; i < content.childCount; i++)
        {
            var pos = content.GetChild(i).position;
            if (pos.x == x && pos.y == y)
                return false;
        }
        return true;
    }

}
