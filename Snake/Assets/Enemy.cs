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
    Transform target;

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

        mapX = gameLogic.mapX + 1;
        mapY = gameLogic.mapY + 1;

        snake.name = "enemy";
        SetRandomPos();

        pathFinder = new AStarSearch(GenMapGrid());
        exit = null;

        target = gameLogic.food;

        if (GenEnemySpwnCoord() == null)
            Debug.Log("coordintes not found..");

    }

    void Update()
    {

        if (!gameLogic.gameOver)
            CheckForRestart();

        if (gameLogic.stepAvailable)
        {

            pathFinder.ClearGrid();
            GetGridObstacles();

            if (exit == null || ExitInsidePlayer())
            {
                   
                // checking if there's a high food available
                // if not then just go for next best thing
                var highFood = GameObject.Find("highQualityFood");

                if (highFood != null)
                {
                    var closest = ClosestFood(
                        gameLogic.food,
                        highFood.transform
                    );

                    target = closest;
                }
                else
                    target = gameLogic.food;

                MoveSnake(target);
            }
            else
            {
                
                MoveSnake(exit);

                if (CheckForExit())
                    Destroy(gameObject);

            }

        }
    }

    Transform ClosestFood(Transform food1, Transform food2)
    {
        float dis1 = GetDistance(food1);
        float dis2 = GetDistance(food2);
        return dis1 < dis2 ? food1 : food2;
    }

    float GetDistance(Transform obj)
    {
        float enemyX = head.position.x;
        float enemyY = head.position.y;

        float objX = obj.position.x;
        float objY = obj.position.y;

        float valX = GetDifference(enemyX, objX);
        float valY = GetDifference(enemyY, objY);

        return valX + valY;
    }

    // gets x/y coordinate difference
    float GetDifference(float val1, float val2)
    {
        float result = val1 - val2;
        return Mathf.Abs(result);
    }

    void CheckForRestart()
    {
        if (Input.GetKeyDown("r"))
            DestroySelf();
    }

    public void DestroySelf()
    {
        if (exit != null)
            Destroy(exit.gameObject);

        Destroy(gameObject);
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
                Destroy(child.gameObject);
                return false;
            }

            Destroy(snake.gameObject);
            Destroy(exit.gameObject);
            return true;
        }
        return false;
    }

    // sets random starting position
    public void SetRandomPos()
    {

        // generating random coordinates
        float[] coord = GenEnemySpwnCoord();

        /*
         * setting position
         */

        // head
        AssignCoord(head, coord[0], coord[1]);
        
        // tail
        for (int i = 1; i < snake.childCount; i++)
        {
            Transform child = snake.GetChild(i);
            AssignCoord(child, coord[2], coord[3]);
        }

    }

    void AssignCoord(Transform obj, float coord1, float coord2)
    {
        Vector2 pos = obj.position;
        pos.x = coord1;
        pos.y = coord2;
        obj.position = pos;
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

        // head
        float x, y;

        // tail
        float f, g;

        // needed to spawn enemy 1 square away from the edge
        float mapx = mapX - 0.5f;
        float mapy = mapY - 0.5f;

        // right / left
        if (choice == 0 || choice == 1)
        {
            x = choice == 0 ? mapx : -mapx;
            y = (int)Random.Range(-mapy * 2, mapy * 2) / 2f;

            f = x + (choice == 0 ? 0.5f : -0.5f);
            g = y;

        }
        // top / bottom 
        else
        {
            y = choice == 2 ? mapy : -mapy;
            x = (int)Random.Range(-mapx * 2, mapx * 2) / 2f;

            g = y + (choice == 2 ? 0.5f : -0.5f);
            f = x;

        }

        // result
        coord = new float[] { x, y, f, g };

        // making sure the coordinate is valid
        if (!ValidEnemySpwnPos(coord))
            coord = GenEnemySpwnCoord(antistuck);
        
        return coord;
    }

    bool ExitInsidePlayer()
    {
        for (int i = 0; i < player.childCount; i++)
        {
            var pos = player.GetChild(i).position;
            if (exit.position == pos)
                return true;
        }
        return false;
    }

    bool OutsideSnakeCoord(float[] coord, Transform snake)
    {
        for (int i = 0; i < snake.childCount; i++)
        {
            var pos = snake.GetChild(i).position;

            // making sure the enemy doesn't spawn
            // inside the player snake
            if (pos.x == coord[0] && pos.y == coord[1])
                return false;
        }
        return true;
    }

    bool ValidEnemySpwnPos(float[] coord)
    {
        if (!OutsideSnakeCoord(coord, player.transform))
            return false;

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
    public void CreateEnemyExit(int antistuck = 0)
    {
        antistuck++;
        if (antistuck > 200)
            return;

        // generates position
        float[] coord = GenEnemySpwnCoord();
        if (coord == null) return;

        if (!OutsideSnakeCoord(coord, snake))
        {
            CreateEnemyExit(antistuck);
            return;
        }

        var exitObj = Instantiate(
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
