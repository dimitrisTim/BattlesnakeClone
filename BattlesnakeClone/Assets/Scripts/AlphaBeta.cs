using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.UI;
using System.Linq;
using System;
using static Snake;

public class AlphaBeta: ScriptableObject
{
    private Snake youSnake;
    private Snake enemySnake;
    private List<Snake> snakes;
    // private List<GameObject> spawnedGoodies;
    // private int[,] boardState;
    public bool TimeExpired { get; set; } = false;
    private Dictionary<int, Dictionary<Snake.Direction, float>> maxMovesSorted = new Dictionary<int, Dictionary<Snake.Direction, float>>();
    private Dictionary<int, Dictionary<Snake.Direction, float>> minMovesSorted = new Dictionary<int, Dictionary<Snake.Direction, float>>();
    private int maxDepth = 0;

    public Direction Bestaction {get; set;}

    // private void CreateBoardState()
    // {
    //     foreach (var snake in snakes)
    //     {
    //         foreach (var bodyPart in snake.snakeBodyPartList)
    //         {
    //             boardState[((int)bodyPart.TransformPosition.x), 
    //                         (int)bodyPart.TransformPosition.y] = snake.ID;
    //         }
    //     }
    //     foreach (var spawnedGoodie in spawnedGoodies)
    //     {
    //         boardState[((int)spawnedGoodie.transform.position.x), 
    //                     (int)spawnedGoodie.transform.position.y] = 10;
    //     }

    // }

    private float? IsGameOver(int counter = 0, int threshold = 5, string whoCalledMe = "max")
    {
        if (TimeExpired)
        {
            Debug.Log("TimeExpired");
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
            //Debug.Log("Threshhold reached");
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

    private Tuple<float, Snake.Direction> MaxAlphaBeta(float alpha, float beta, int depth, int maxDepth)
    {
        float maxv = -2f;
        Snake.Direction move = Snake.Direction.Right;
        depth += 1;
        var result = IsGameOver(depth, maxDepth, "max");
        //Debug.Log("result" + depth +": " + result);
        if (depth > this.maxDepth) this.maxDepth = depth;
        if (result != null)
        {
            return new Tuple<float, Snake.Direction>((float)result, Snake.Direction.Right);
        }
        List<Snake.Direction> possibleActions = null;
        if (maxMovesSorted.ContainsKey(depth))
        {
            possibleActions = maxMovesSorted[depth].Keys.ToList();
        }
        else
        {
            possibleActions = youSnake.GetPossibleActions();
        }
        //Debug.Log(possibleActions);
        
        var sortedMoves = new Dictionary<Snake.Direction, float>();
        foreach (var action in possibleActions)
        {
            var nextMove = new Tuple<int, Snake.Direction>(youSnake.ID, action);
            youSnake.ExecuteMovement(nextMove.Item2, true);
            var minAlphaBeta = MinAlphaBeta(alpha, beta, depth, maxDepth);
            sortedMoves[action] = minAlphaBeta.Item1;
            if (minAlphaBeta.Item1 > maxv)
            {
                maxv = minAlphaBeta.Item1;
                move = action;
            }
            youSnake.ResetRecordedState();
            if (maxv >= beta)
            {
                maxMovesSorted[depth] = sortedMoves.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                return new Tuple<float, Snake.Direction>(maxv, move);
            }
            if (maxv > alpha)
            {
                alpha = maxv;
            }
        }
        maxMovesSorted[depth] = sortedMoves.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        return new Tuple<float, Snake.Direction>(maxv, move);
    }

    private Tuple<float, Snake.Direction> MinAlphaBeta(float alpha, float beta, int depth, int maxDepth)
    {
        float minv = 2;
        Snake.Direction move = Snake.Direction.Right;
        depth += 1;
        var result = IsGameOver(depth, maxDepth, "min");
        if (depth > this.maxDepth) this.maxDepth = depth;
        if (result != null)
        {
            return new Tuple<float, Snake.Direction>((float)result, Snake.Direction.Right);
        }
        List<Snake.Direction> possibleActions = null;
        if (minMovesSorted.ContainsKey(depth))
        {
            possibleActions = minMovesSorted[depth].Keys.ToList();
        }
        else
        {
            possibleActions = enemySnake.GetPossibleActions();
        }
        var sortedMoves = new Dictionary<Snake.Direction, float>();
        foreach (var action in possibleActions)
        {
            var nextMove = new Tuple<int, Snake.Direction>(enemySnake.ID, action);
            enemySnake.ExecuteMovement(nextMove.Item2, true);
            var maxAlphaBeta = MaxAlphaBeta(alpha, beta, depth, maxDepth);
            sortedMoves[action] = maxAlphaBeta.Item1;
            if (maxAlphaBeta.Item1 < minv)
            {
                minv = maxAlphaBeta.Item1;
                move = action;
            }
            enemySnake.ResetRecordedState();
            if (minv <= alpha)
            {
                minMovesSorted[depth] = sortedMoves.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                return new Tuple<float, Snake.Direction>(minv, move);
            }
            if (minv < beta)
            {
                beta = minv;
            }
        }
        minMovesSorted[depth] = sortedMoves.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        return new Tuple<float, Snake.Direction>(minv, move);
    }


    private float Evaluate(Snake you, Snake enemy)
    {
        var boardControl = CalculateBoardControl(you, enemy);
        var lengthAdvantage = CalculateLengthAdvantage(you);
        var health = Math.Sqrt(you.Health) * 0.1f;
        var value = 0.1f * boardControl +  0.3f * lengthAdvantage + 0.5f * ((float)health);
        Debug.Log("Eval value: " + value);
        if (value > 0)
        {
            return value * 0.5f;
        }
        else
        {
            return 0.001f;
        }        
    }

    public void PlayAlphaBeta(List<Snake> snakes)
    {
        this.youSnake = snakes[0];
        this.enemySnake = snakes[1];
        this.snakes = snakes;

        var depth = 1;
        var prevSimDepth = 0;
        //while (!TimeExpired)
        //Save start state
        for (int i = 0; i<3;i++)
        {
            //Debug.Log("Start AlphaBeta: " + maxDepth);
            //sim depth
            Vector3 youPosition3d = new Vector3(youSnake.GridPosition.x, youSnake.GridPosition.y, 0);
            Vector3 enemyPosition3d = new Vector3(enemySnake.GridPosition.x, enemySnake.GridPosition.y, 0);
            youSnake.StartRecording(youPosition3d);
            enemySnake.StartRecording(enemyPosition3d);
            var m = MaxAlphaBeta(-2, 2, 0, depth);
            youSnake.ResetRecordedState();
            enemySnake.ResetRecordedState();
            if (prevSimDepth != maxDepth)
            {
                prevSimDepth = maxDepth;
            }
            else
            {
                //break;
            }
            Bestaction = m.Item2;
            depth += 1;
            //Debug.Log("Depth: " + maxDepth);
            //Debug.Log("best action: "+ Bestaction);
        }
        //Bestaction = Direction.Up;
    }

    public IEnumerator StartTimer()
    {
        // Wait for the specified interval
        yield return new WaitForSeconds(1);

        //Debug.Log("Alpha beta timer expired");
        TimeExpired = true;
    }

    private float CalculateBoardControl(Snake you, Snake enemy)
    {
        // Initialize a variable to keep track of the advantage
        float ownSpace = 0;
        float enemySpace = 0;

        // Get the coordinates of the snake's head
        int headX = you.GridPosition.x;
        int headY = you.GridPosition.y;

        // Iterate through the grid
        for (int x = -GameAssets.i.Width; x < GameAssets.i.Width; x++)
        {
            for (int y = -GameAssets.i.Height; x < GameAssets.i.Height; x++)
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
        var distanceFromCenter = Math.Abs((snake.GridPosition.x - Math.Floor(GameAssets.i.Width / 2f)) + 
                                        (snake.GridPosition.y - Math.Floor(GameAssets.i.Height / 2f)));
        return (float)(2 - distanceFromCenter / GameAssets.i.Width / 2 * 1.5f);
    }

    private bool IsInVoronoiRegion(int headX, int headY, int x, int y)
    {
        float distanceToEnemyHead = Distance(x, y, enemySnake.GridPosition.x, enemySnake.GridPosition.y);
        
        // Check if the distance to the cell is less than or equal to the distance to the closest enemy snake
        return Distance(headX, headY, x, y) <= distanceToEnemyHead;
    }

    private float Distance(int x1, int y1, int x2, int y2)
    {
        return (float) Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
    }


    private float CalculateLengthAdvantage(Snake you)
    {
        var sum = 1;
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
