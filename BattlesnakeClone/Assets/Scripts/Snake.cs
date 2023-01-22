using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.UI;
using System.Linq;

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
    public float GridMoveTimerMax = 0.99f;
    public bool Alive {get; set;}
    private bool snakeAte = false;
    private int prevLength;
    public IKeyStrategy KeyStrategy {get; set;}

    private Slider slider;
    private float fillSpeed = 0.5f;
    private float startingHealth = 1f;

    public int ID {get; set;}
    public GameObject wallPrefab;
    public bool CheckAbility {get; set;}

    public AudioSource audioSource1, audioSource2;
    public AudioClip clip1 , clip2;
    public enum AudioClipName {clip1, clip2};
    private void Start()
    {
        audioSource1 = GetComponent<AudioSource>();
        audioSource1.clip = clip1;
        audioSource2 = GetComponent<AudioSource>();
        audioSource2.clip = clip2;
        prevLength = 0;
        GridPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        SetValidRandomDirection();
        snakeBodyPartList = new List<SnakeBodyPart>();
        snakeMovePositionList = new List<SnakeMovePosition>();
        score = 0;
        scoreTXT = GameObject.Find("Score" + ID).GetComponent<TextMeshProUGUI>();
        scoreTXT.text = "Score: " + score;
        Alive = true;
        gridMoveTimer = 0f;
        GridMoveTimerMax = 0.25f;     
        slider = GameObject.Find("Slider" + ID).GetComponent<Slider>();
        slider.value = startingHealth;
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

    private void UpdateHealthBar()
    {
        if (snakeAte)
        {
            slider.value = startingHealth;
        }
        else
        {
            slider.value -= fillSpeed * Time.deltaTime;
            if (slider.value <= 0)
            {
                Alive = false;
            }
        }
    }

    private void SetValidRandomDirection()
    {
        var validPositions = new List<Direction>();
        if (GridPosition.x > -GameAssets.i.Width)
        {
            validPositions.Add(Direction.Left);
        }
        if (GridPosition.x < GameAssets.i.Width)
        {
            validPositions.Add(Direction.Right);
        }
        if (GridPosition.y > -GameAssets.i.Height)
        {
            validPositions.Add(Direction.Down);
        }
        if (GridPosition.y < GameAssets.i.Height)
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

    public Vector2Int GetNewGridPosition(int distance = 1)
    {        
        return GridPosition + GetGridMoveDirectionVector() * distance;
    }

    public Vector2Int GetGridMoveDirectionVector()
    {
        switch (gridMoveDirection)
        {
            default:
            case Direction.Right: return new Vector2Int(+1, 0);
            case Direction.Left: return new Vector2Int(-1, 0);
            case Direction.Up: return new Vector2Int(0, 1);
            case Direction.Down: return new Vector2Int(0, -1);
        }
    }

    private void HandleGridMovement()
    { 
        gridMoveTimer += Time.deltaTime;
        if (gridMoveTimer >= GridMoveTimerMax)
        {
            gridMoveTimer -= GridMoveTimerMax;
            SnakeMovePosition previousSnakeMovePosition = null;
            if (snakeMovePositionList.Count > 0)
            {
                previousSnakeMovePosition = snakeMovePositionList[0];
            }
            SnakeMovePosition snakeMovePosition = new SnakeMovePosition(previousSnakeMovePosition, GridPosition, gridMoveDirection);
            snakeMovePositionList.Insert(0, snakeMovePosition);

            GridPosition = GetNewGridPosition();

            if (snakeAte)
            {
                snakeBodySize++;
                CreateSnakeBodyPart();                
            }
            if (snakeMovePositionList.Count >= snakeBodySize + 1)
            {
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
            }
            UpdateHealthBar();
            snakeAte = false;
            UpdateSnakeBodyParts();
            transform.position = new Vector3(GridPosition.x, GridPosition.y);
            transform.eulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(GetGridMoveDirectionVector()) - 90);           
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
            audioSource1.clip = clip1;
            audioSource1.Play();
            Destroy(collision.gameObject);
        }
        if (collision.CompareTag("Speed"))
        {
            audioSource2.clip = clip2;
            audioSource2.Play();
            Destroy(collision.gameObject);
            this.gameObject.AddComponent<SpeedAbility>();
        }
        if (collision.CompareTag("BrickSphere"))
        {
            Debug.Log("BrickSphere detected");
            Destroy(collision.gameObject);
            // If ability not already active, add it
            if (this.gameObject.TryGetComponent<WallAbility>(out WallAbility wallAbility) == false)
            {
                this.gameObject.AddComponent<WallAbility>();
            }
        }
        if (collision.CompareTag("Player"))
        {
            // Game Over!
            Debug.Log("Body Collision");
            Alive = false;
        }
        if (collision.CompareTag("Wall"))
        {
            // Game Over!
            Debug.Log("Wall Collision");
            Alive = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            // Game Over!
            Debug.Log("Wall Collision");
            Alive = false;
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
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.snakeBodySpriteVertical;
            transform = snakeBodyGameObject.transform;
        }
        public void SetSnakeMovePosition(SnakeMovePosition snakeMovePosition)
        {
            this.snakeMovePosition = snakeMovePosition;
            Destroy(this.transform.gameObject);
            GameObject snakeBodyGameObject = new GameObject("SnakeBody", typeof(SpriteRenderer));
            switch (snakeMovePosition.GetDirection())
            {
                default:
                case Direction.Up: // Currently going Up
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.snakeBodySpriteVertical;
                            this.transform = snakeBodyGameObject.transform;
                            break;
                        case Direction.Left: // Previously was going Left           
                            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.snakeCornerLowerLeft;
                            this.transform = snakeBodyGameObject.transform;
                            transform.position += new Vector3(.2f, .2f);
                            break;
                        case Direction.Right: // Previously was going Right                           
                            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.snakeCornerLowerRight;
                            this.transform = snakeBodyGameObject.transform;
                            transform.position += new Vector3(-.2f, .2f);
                            break;
                    }
                    break;
                case Direction.Down: // Currently going Down
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.snakeBodySpriteVertical;
                            this.transform = snakeBodyGameObject.transform;
                            break;
                        case Direction.Left: // Previously was going Left
                            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.snakeCornerUpperLeft;
                            this.transform = snakeBodyGameObject.transform;
                            transform.position += new Vector3(.2f, -.2f);
                            break;
                        case Direction.Right: // Previously was going Right
                            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.snakeCornerUpperRight;
                            this.transform = snakeBodyGameObject.transform;
                            transform.position += new Vector3(-.2f, -.2f);
                            break;
                    }
                    break;
                case Direction.Left: // Currently going to the Left
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.snakeBodySpriteHorizontal;
                            this.transform = snakeBodyGameObject.transform;
                            break;
                        case Direction.Down: // Previously was going Down
                            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.snakeCornerLowerRight;
                            this.transform = snakeBodyGameObject.transform;
                            transform.position += new Vector3(-.2f, .2f);
                            break;
                        case Direction.Up: // Previously was going Up
                            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.snakeCornerUpperRight;
                            this.transform = snakeBodyGameObject.transform;
                            transform.position += new Vector3(-.2f, -.2f);
                            break;
                    }
                    break;
                case Direction.Right: // Currently going to the Right
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.snakeBodySpriteHorizontal;
                            this.transform = snakeBodyGameObject.transform;
                            break;
                        case Direction.Down: // Previously was going Down
                            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.snakeCornerLowerLeft;
                            this.transform = snakeBodyGameObject.transform;
                            transform.position += new Vector3(.2f, .2f);
                            break;
                        case Direction.Up: // Previously was going Up
                            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.snakeCornerUpperLeft;
                            this.transform = snakeBodyGameObject.transform;
                            transform.position += new Vector3(.2f, -.2f);
                            break;
                    }
                    break;
            }
            snakeBodyGameObject.AddComponent<BoxCollider2D>();
            snakeBodyGameObject.tag = "Player";
            transform.position = new Vector3(snakeMovePosition.GetGridPosition().x, snakeMovePosition.GetGridPosition().y);
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
