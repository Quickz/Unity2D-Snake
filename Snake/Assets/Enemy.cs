using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{

    public AStarSearch pathFinder;
    public List<int[]> path;
    public Transform exit;

    GameObject game;
    Transform player;

    public Snake snakeLogic;
    GameLogic gameLogic;
    
    public Transform snake;
    Transform head;

    float mapX;
    float mapY;

    void Start()
    {
        game = GameObject.FindWithTag("Game");
        snakeLogic = gameObject.GetComponent<Snake>();
        gameLogic = game.GetComponent<GameLogic>();

        snake = gameObject.transform;
        head = snake.GetChild(0);

        player = game.transform.GetChild(0);

        mapX = gameLogic.mapX;
        mapY = gameLogic.mapY;

        snake.name = "enemy";
        SetRandomPos();

        pathFinder = new AStarSearch(GenMapGrid());
        exit = null;

        if (GenEnemySpwnCoord() == null)
            Debug.Log("coordintes not found..");

    }

    void Update()
    {
        if (gameLogic.stepAvailable)
        {
            pathFinder.ClearGrid();
            GetGridObstacles();

            if (exit == null)
                MoveSnake(gameLogic.food);
            else
            {
                MoveSnake(exit);

                if (CheckForExit())
                    Destroy(gameObject);

            }

        }
    }

    // checks if enemy has reached exit point
    public bool CheckForExit()
    {
        if (exit.position == head.position)
        {

            // making sure to get rid of the tail
            // before getting rid of the enemy itself
            if (snake.childCount > 1)
            {
                var child = snake.GetChild(
                        snake.childCount - 1
                    );
                Object.Destroy(child.gameObject);
                return false;
            }

            Object.Destroy(snake.gameObject);
            Object.Destroy(exit.gameObject);
            return true;
        }
        return false;
    }

    // creates a red enemy snake
    public void SetRandomPos()
    {

        // generating random coordinates
        float[] coord = GenEnemySpwnCoord();   

        // setting position
        for (int i = 0; i < snake.childCount; i++)
        {
            Transform child = snake.GetChild(i);
            Vector2 pos = child.position;
            pos.x = coord[0];
            pos.y = coord[1];
            child.position = pos;
        }

    }

    // generates position coordinates for the enemy
    float[] GenEnemySpwnCoord(int antistuck = 0)
    {
        antistuck++;
        // if during 200 attempts a valid coordinate
        // is not found null is returned
        if (antistuck > 200)
            return null;

        // generating random number - 0/1/2/3
        float choice = (int)Random.Range(0, 4);

        float[] coord;
        float x;
        float y;

        // right / left
        if (choice == 0 || choice == 1)
        {
            x = choice == 0 ? mapX : -mapX;
            y = (int)Random.Range(-mapY * 2, mapY * 2) / 2f;
        }
        // top / bottom 
        else
        {
            y = choice == 2 ? mapY : -mapY;
            x = (int)Random.Range(-mapX * 2, mapX * 2) / 2f;
        }

        // result
        coord = new float[] { x, y };

        // making sure the coordinate is valid
        if (!ValidEnemySpwnPos(coord))
            coord = GenEnemySpwnCoord(antistuck);
        
        return coord;
    }

    bool ValidEnemySpwnPos(float[] coord)
    {

        for (int i = 0; i < player.transform.childCount; i++)
        {
            var pos = player.transform.GetChild(i).position;

            // making sure the enemy doesn't spawn
            // inside the player snake
            if (pos.x == coord[0] && pos.y == coord[1])
                return false;
        }

        /*
            making sure the snake's head
            is not too close
        */

        var headPos = player.transform.GetChild(0).position;

        // snake
        float playerX = headPos.x + mapX;
        float playerY = headPos.y + mapY;

        // enemy
        float enemyX = coord[0] + mapX;
        float enemyY = coord[1] + mapY;

        float difX = Mathf.Abs(playerX - enemyX);
        float difY = Mathf.Abs(playerY - enemyY);

        // Making sure the enemy coordinate is at least 14 squares
        // away both horizontally and vertically before spawning
        if (difX < 7 && difY < 7 ||
            Mathf.Abs(mapX * 2 - difX) < 7 && Mathf.Abs(mapY * 2 - difY) < 7)
            return false;

        return true;
    }

    // saves current and last enemy direction
    void SetEnemyDir(Vector2 oldPos, Vector2 currPos)
    {
        snakeLogic.lastDir = snakeLogic.currDir;

        if (oldPos.x < currPos.x)
            snakeLogic.currDir = "right";
        else if (oldPos.x > currPos.x)
            snakeLogic.currDir = "left";
        else if (oldPos.y < currPos.y)
            snakeLogic.currDir = "up";
        else if (oldPos.y > currPos.y)
            snakeLogic.currDir = "down";

    }

    // creates an exit point for the enemy snake
    public void CreateEnemyExit()
    {

        // generates position
        float[] coord = GenEnemySpwnCoord();
        if (coord == null) return;

        var exitObj = Object.Instantiate(
            Resources.Load("enemyExit")
        ) as GameObject;

        exitObj.name = "enemyExit";
        exit = exitObj.transform;

        // setting the position
        var pos = exit.position;
        pos.x = coord[0];
        pos.y = coord[1];
        exit.position = pos;

    }

    public void MoveSnake(Transform target)
    {
        var enemyPos = GetGridCoord(head.position);
        var foodPos = GetGridCoord(target.position);

        path = pathFinder.run(enemyPos, foodPos);

        if (path != null)
        {
            snakeLogic.MoveTail();

            var oldPos = head.position;
            SetPosFromGrid(path[1], head);

            SetEnemyDir(oldPos, head.position);
            snakeLogic.BendTail();

        }
    }

    public void GetGridObstacles()
    {

        // player snake
        for (int i = 0; i < player.transform.childCount; i++)
        {
            int[] pos = GetGridCoord(player.transform.GetChild(i).position);
            pathFinder.grid[pos[0], pos[1]] = 1;
        }

        // enemy tail
        for (int i = 1; i < snake.childCount; i++)
        {
            int[] pos = GetGridCoord(snake.GetChild(i).position);
            pathFinder.grid[pos[0], pos[1]] = 1;
        }

    }

    // generates empty grid
    public int[,] GenMapGrid()
    {
        int[,] grid = new int[(int)(mapX * 4) + 1, (int)(mapY * 4) + 1];

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
                grid[i, j] = 0;
        }

        return grid;
    }

    // converts ingame coordinate to a coordinate for the grid
    int[] GetGridCoord(Vector2 position)
    {
        int posX = (int)((position.x + mapX) * 2);
        int posY = (int)((position.y + mapY) * 2);
        return new int[] { posX, posY };
    }

    // converts grid coordinates to the world coordinates
    void SetPosFromGrid(int[] coordinates, Transform obj)
    {
        var pos = obj.position;
        pos.x = coordinates[0] / 2f - mapX;
        pos.y = coordinates[1] / 2f - mapY;
        obj.position = pos;
    }



}//end
