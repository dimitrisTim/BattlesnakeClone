using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;
using System.Linq;

public class LevelGrid
{
    private int width;
    private int height;
    private List<Vector2Int> foodGridPositionList = new List<Vector2Int>();
    private List<GameObject> foodGameObjectList = new List<GameObject>();
    private Snake snake;

    public LevelGrid(int width, int height)
    {
        this.width = width;
        this.height = height;
        
        
    }

    public void Setup(Snake snake)
    {
        this.snake = snake;
        SpawnFood();
        FunctionPeriodic.Create(SpawnFood, 5f);
    }

    private void SpawnFood()
    {
        Vector2Int newFoodPosition;
        do {
            newFoodPosition = new Vector2Int(Random.Range(-width, width), Random.Range(-height, height));             
        }
        while (newFoodPosition.Equals(snake.GetGridPosition()));

        foodGridPositionList.Add(newFoodPosition);           

        foodGameObjectList.Add(new GameObject("Food", typeof(SpriteRenderer)));
        foodGameObjectList.Last().GetComponent<SpriteRenderer>().sprite = GameAssets.i.foodSprite;
        foodGameObjectList.Last().transform.position = new Vector3(foodGridPositionList.Last().x, foodGridPositionList.Last().y);
    }

    public void SnakeMoved(Vector2Int snakeGridPosition)
    {
        if (foodGridPositionList.Any(pos=> pos.Equals(snakeGridPosition)))
        {
            var foodIndex = foodGridPositionList.FindIndex(pos => pos.Equals(snakeGridPosition));
            Object.Destroy(foodGameObjectList[foodIndex]);
            //SpawnFood();
            CMDebug.TextPopupMouse("Food Eaten!");
        }
    }
}
