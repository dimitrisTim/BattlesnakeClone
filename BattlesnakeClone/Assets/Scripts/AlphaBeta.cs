using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.UI;
using System.Linq;
using System;

public class AlphaBeta
{
    private Snake youSnake;
    private Snake enemySnake;
    private List<Snake> snakes;
    private List<GameObject> spawnedGoodies;
    private int[,] boardState;
    private bool timeExpired;
    
    /// <summary>
    /// First input the AI snake, second the enemy.    
    /// </summary>
    public AlphaBeta(List<Snake> snakes)
    {
        this.youSnake = snakes[0];
        this.enemySnake = snakes[1];
        this.snakes = snakes;
        this.spawnedGoodies = new List<GameObject>();
        this.boardState = new int[GameAssets.i.Width, GameAssets.i.Height];        
        this.timeExpired = false;

        spawnedGoodies.AddRange(GameObject.FindGameObjectsWithTag("BrickSphere"));
        spawnedGoodies.AddRange(GameObject.FindGameObjectsWithTag("Scoring"));
        spawnedGoodies.AddRange(GameObject.FindGameObjectsWithTag("Speed"));
    }

    private void CreateBoardState()
    {
        foreach (var snake in snakes)
        {
            foreach (var bodyPart in snake.snakeBodyPartList)
            {
                boardState[((int)bodyPart.Transform.position.x), 
                            (int)bodyPart.Transform.position.y] = snake.ID;
            }
        }
        foreach (var spawnedGoodie in spawnedGoodies)
        {
            boardState[((int)spawnedGoodie.transform.position.x), 
                        (int)spawnedGoodie.transform.position.y] = 10;
        }

    }

    private float? IsGameOver(int counter = 0, int threshold = 100, string whoCalledMe = "max")
    {
        if (timeExpired)
        {
            return -100;
        }
        var youAlive = youSnake.Alive;
        var enemyAlive = enemySnake.Alive;
        if (!youAlive && !enemyAlive)
        {
            return 0;
        }
        if (!youAlive)
        {
            return -1;
        }
        if (!enemyAlive)
        {            
            return Evaluate(youSnake, enemySnake) + 0.2f;
        }
        if (counter >= threshold)
        {
            if (whoCalledMe == "min")
            {
                return -Evaluate(enemySnake, youSnake);
            }
            else
            {
                return Evaluate(youSnake, enemySnake);
            }
        }
        return null;
    }

    private float Evaluate(Snake you, Snake enemy)
    {
        var boardControl = CalculateBoardControl(you, enemy);
        var lengthAdvantage = CalculateLengthAdvantage(you);
        var health = Math.Sqrt(you.Health) * 0.1f;
        var value = 0.2f * boardControl +  0.6f * lengthAdvantage + 0.2f * ((float)health);
        if (value > 0)
        {
            return value * 0.5f;
        }
        else
        {
            return 0.001f;
        }        
    }

    private float CalculateBoardControl(Snake you, Snake enemy)
    {
        // Initialize a variable to keep track of the advantage
        float ownSpace = 0;
        float enemySpace = 0;

        // Get the coordinates of the snake's head
        int headX = you.GetGridPosition().x;
        int headY = you.GetGridPosition().y;

        // Iterate through the grid
        for (int y = 0; y < boardState.GetLength(0); y++)
        {
            for (int x = 0; x < boardState.GetLength(1); x++)
            {
                // Check if the current cell is within the Voronoi region of the snake's head
                if (IsInVoronoiRegion(headX, headY, x, y))
                {
                    ownSpace += DistanceFromCenter(you);
                }
                else
                {
                    enemySpace += DistanceFromCenter(enemy);
                }
            }
        }

        var boardControl = ownSpace / (enemySpace / (snakes.Count - 1));
        return (float) Math.Tanh(boardControl);
    }

    private float DistanceFromCenter(Snake snake)
    {        
        var distanceFromCenter = Math.Abs((snake.GetGridPosition().x - Math.Floor(GameAssets.i.Width / 2f)) + 
                                        (snake.GetGridPosition().y - Math.Floor(GameAssets.i.Height / 2f)));
        return (float)(2 - distanceFromCenter / GameAssets.i.Width / 2 * 1.5f);
    }

    private bool IsInVoronoiRegion(int headX, int headY, int x, int y)
    {
        float distanceToEnemyHead = Distance(x, y, enemySnake.GetGridPosition().x, enemySnake.GetGridPosition().y);
        
        // Check if the distance to the cell is less than or equal to the distance to the closest enemy snake
        return Distance(headX, headY, x, y) <= distanceToEnemyHead;
    }

    private float Distance(int x1, int y1, int x2, int y2)
    {
        return (float) Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
    }


    private float CalculateLengthAdvantage(Snake you)
    {
        var sum = 0;
        foreach (var snake in snakes)
        {
            if (snake.ID != you.ID)
            {
                sum += snake.snakeBodyPartList.Count;
            }
        }
        if (snakes.Count <= 1)
        {
            return 0.0f;
        }
        var avarageLength = sum / (snakes.Count - 1);
        var ourBodyRelativeLength = you.snakeBodyPartList.Count / avarageLength;
        if (ourBodyRelativeLength >= 2)
        {
            return 0.1f * (ourBodyRelativeLength) + 0.9f;
        }
        if (ourBodyRelativeLength <= 0.5)
        {
            return -2 * ourBodyRelativeLength - 1;
        }
        return ourBodyRelativeLength - 0.5f;
    }

}