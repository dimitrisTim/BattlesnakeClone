using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using CodeMonkey.Utils;

public class Snake : MonoBehaviour
{
    private enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    private Direction gridMoveDirection;
    public Vector2Int GridPosition {get; private set;}
    private Rigidbody2D body;
    [SerializeField] private float moveSpeed = 1f;

    private int score;
    private TextMeshProUGUI scoreTXT;
    private int snakeBodySize = 0;
    private List<SnakeMovePosition> snakeMovePositionList;
    private List<SnakeBodyPart> snakeBodyPartList;
    private float gridMoveTimer = 0f;
    private float gridMoveTimerMax = 0.25f;
    public bool Alive {get; set;}
    private bool snakeAte = false;
    private int prevLength;
    private int width, height;    
    public IKeyStrategy KeyStrategy {get; set;}

    private void Awake()
    {
        prevLength = 0;
        GridPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        SetValidRandomDirection();
        snakeBodyPartList = new List<SnakeBodyPart>();
        snakeMovePositionList = new List<SnakeMovePosition>();
        score = 0;
        scoreTXT = GameObject.Find("Score").GetComponent<TextMeshProUGUI>();
        scoreTXT.text = "Score: " + score;
        Alive = true;
        gridMoveTimer = 0f;
        gridMoveTimerMax = 0.25f;       
    }

    private void Update()
    {
        switch (Alive)
        {
            case true:
                HandleInput();
                HandleGridMovement();
                break;
            case false:
                break;
        }
    }

    private void SetValidRandomDirection()
    {
        var validPositions = new List<Direction>();
        if (GridPosition.x > 0)
        {
            validPositions.Add(Direction.Left);
        }
        if (GridPosition.x < width - 1)
        {
            validPositions.Add(Direction.Right);
        }
        if (GridPosition.y > 0)
        {
            validPositions.Add(Direction.Down);
        }
        if (GridPosition.y < height - 1)
        {
            validPositions.Add(Direction.Up);
        }
        gridMoveDirection = validPositions[Random.Range(0, validPositions.Count)];
    }

    private void HandleInput()
    {   
        if (KeyStrategy.CheckUp())
        {
            if (gridMoveDirection != Direction.Down)
            {
                gridMoveDirection = Direction.Up;
            }
        }
        if (KeyStrategy.CheckDown())
        {
            if (gridMoveDirection != Direction.Up)
            {
                gridMoveDirection = Direction.Down;
            }
        }
        if (KeyStrategy.CheckLeft())
        {
            if (gridMoveDirection != Direction.Right)
            {
                gridMoveDirection = Direction.Left;
            }
        }
        if (KeyStrategy.CheckRight())
        {
            if (gridMoveDirection != Direction.Left)
            {
                gridMoveDirection = Direction.Right;
            }
        }
    }

    private void HandleGridMovement()
    { 
        gridMoveTimer += Time.deltaTime;
        if (gridMoveTimer >= gridMoveTimerMax)
        {
            gridMoveTimer -= gridMoveTimerMax;
            SnakeMovePosition previousSnakeMovePosition = null;
            if (snakeMovePositionList.Count > 0)
            {
                previousSnakeMovePosition = snakeMovePositionList[0];
            }
            SnakeMovePosition snakeMovePosition = new SnakeMovePosition(previousSnakeMovePosition, GridPosition, gridMoveDirection);
            snakeMovePositionList.Insert(0, snakeMovePosition);
            Vector2Int gridMoveDirectionVector;
            switch (gridMoveDirection)
            {
                default:
                case Direction.Right: gridMoveDirectionVector = new Vector2Int(+1, 0); break;
                case Direction.Left: gridMoveDirectionVector = new Vector2Int(-1, 0); break;
                case Direction.Up: gridMoveDirectionVector = new Vector2Int(0, 1); break;
                case Direction.Down: gridMoveDirectionVector = new Vector2Int(0, -1); break;
            }
            GridPosition += gridMoveDirectionVector;

            if (snakeAte)
            {
                snakeBodySize++;
                CreateSnakeBodyPart();
                snakeAte = false;
            }
            if (snakeMovePositionList.Count >= snakeBodySize + 1)
            {
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
            }

            UpdateSnakeBodyParts();

            foreach (SnakeBodyPart snakeBodyPart in snakeBodyPartList)
            {
                Vector2Int snakeBodyPartGridPosition = snakeBodyPart.GetGridPosition();
                if (GridPosition == snakeBodyPartGridPosition)
                {
                    // Game Over!                    
                    Alive = false;
                }
            }
            transform.position = new Vector3(GridPosition.x, GridPosition.y);
            transform.eulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(gridMoveDirectionVector) - 90);           
        }

    }

    private void CreateSnakeBodyPart()
    {
        snakeBodyPartList.Add(new SnakeBodyPart(snakeBodyPartList.Count));
    }

    private void UpdateSnakeBodyParts()
    {
        for (int i = 0; i < snakeBodyPartList.Count; i++)
        {
            snakeBodyPartList[i].SetSnakeMovePosition(snakeMovePositionList[i]);
        }
    }

    private float GetAngleFromVectorFloat(Vector2 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

   public Vector2Int GetGridPosition()
    {
        return GridPosition;
    }
   private void createSnakeBody()
    {
        snakeBodyPartList.Add(new SnakeBodyPart(snakeBodyPartList.Count));
        //GameObject snakeBodyGameObject = new GameObject("SnakeBody", typeof(SpriteRenderer));        
        //snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.snakeBodySprite;
        //snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = -snakeBodyTransformList.Count;
        //snakeBodyPartList.Add(snakeBodyGameObject.transform);
    }
   private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision detected");
        if (collision.CompareTag("Scoring"))
        {
            score++;
            scoreTXT.text = "Score: " + score;
            grow();
            // createSnakeBody();
            Destroy(collision.gameObject);
        }
    }

    // Return the full list of positions occupied by the snake: Head + Body
    public List<Vector2Int> GetFullSnakeGridPositionList()
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>() { GridPosition };
        foreach (SnakeMovePosition snakeMovePosition in snakeMovePositionList)
        {
            gridPositionList.Add(snakeMovePosition.GetGridPosition());
        }
        return gridPositionList;
    }
    private void grow()
    {
        //CreateSnakeBodyPart();
        snakeAte = true;        
    }
    private class SnakeBodyPart
    {
        private SnakeMovePosition snakeMovePosition;
        private Transform transform;
        public SnakeBodyPart(int bodyIndex)
        {
            GameObject snakeBodyGameObject = new GameObject("SnakeBody", typeof(SpriteRenderer));
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.snakeBodySprite;
            transform = snakeBodyGameObject.transform;
        }
        public void SetSnakeMovePosition(SnakeMovePosition snakeMovePosition)
        {
            this.snakeMovePosition = snakeMovePosition;

            transform.position = new Vector3(snakeMovePosition.GetGridPosition().x, snakeMovePosition.GetGridPosition().y);

            float angle;
            switch (snakeMovePosition.GetDirection())
            {
                default:
                case Direction.Up: // Currently going Up
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 0;
                            break;
                        case Direction.Left: // Previously was going Left
                            angle = 0 + 45;
                            transform.position += new Vector3(.2f, .2f);
                            break;
                        case Direction.Right: // Previously was going Right
                            angle = 0 - 45;
                            transform.position += new Vector3(-.2f, .2f);
                            break;
                    }
                    break;
                case Direction.Down: // Currently going Down
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 180;
                            break;
                        case Direction.Left: // Previously was going Left
                            angle = 180 - 45;
                            transform.position += new Vector3(.2f, -.2f);
                            break;
                        case Direction.Right: // Previously was going Right
                            angle = 180 + 45;
                            transform.position += new Vector3(-.2f, -.2f);
                            break;
                    }
                    break;
                case Direction.Left: // Currently going to the Left
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = +90;
                            break;
                        case Direction.Down: // Previously was going Down
                            angle = 180 - 45;
                            transform.position += new Vector3(-.2f, .2f);
                            break;
                        case Direction.Up: // Previously was going Up
                            angle = 45;
                            transform.position += new Vector3(-.2f, -.2f);
                            break;
                    }
                    break;
                case Direction.Right: // Currently going to the Right
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = -90;
                            break;
                        case Direction.Down: // Previously was going Down
                            angle = 180 + 45;
                            transform.position += new Vector3(.2f, .2f);
                            break;
                        case Direction.Up: // Previously was going Up
                            angle = -45;
                            transform.position += new Vector3(.2f, -.2f);
                            break;
                    }
                    break;
            }

            transform.eulerAngles = new Vector3(0, 0, angle);
        }

        public Vector2Int GetGridPosition()
        {
            return snakeMovePosition.GetGridPosition();
        }
    }
    private class SnakeMovePosition
    {
        private SnakeMovePosition previousSnakeMovePosition;
        private Vector2Int gridPosition;
        private Direction direction;

        public SnakeMovePosition(SnakeMovePosition previousSnakeMovePosition, Vector2Int gridPosition, Direction direction)
        {
            this.previousSnakeMovePosition = previousSnakeMovePosition;
            this.gridPosition = gridPosition;
            this.direction = direction;
        }
        public Vector2Int GetGridPosition()
        {
            return gridPosition;
        }
        public Direction GetDirection()
        {
            return direction;
        }
        public Direction GetPreviousDirection()
        {
            if (previousSnakeMovePosition == null)
            {
                return Direction.Right;
            }
            else
            {
                return previousSnakeMovePosition.direction;
            }
        }
    }
}
