using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;

public class Snake : MonoBehaviour, System.ICloneable
{
    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    private Direction gridMoveDirection;
    public Vector2Int GridPosition;
    private Rigidbody2D body;
    [SerializeField] private float moveSpeed = 1f;

    private int score;
    private TextMeshProUGUI scoreTXT;
    public int snakeBodySize = 0;
    private List<SnakeMovePosition> snakeMovePositionList = new List<SnakeMovePosition>();
    public List<SnakeBodyPart> snakeBodyPartList = new List<SnakeBodyPart>();
    private float gridMoveTimer = 0f;
    public float GridMoveTimerMax = 0.1f;
    public bool Alive;
    private bool snakeAte = false;
    private int prevLength;
    public IKeyStrategy KeyStrategy {get; set;}

    private Slider slider;
    public float Health;
    private float fillSpeed = 0.2f;
    private float startingHealth = 1f;

    public int ID;
    public GameObject wallPrefab;
    public bool CheckAbility {get; set;}

    public Color MyColor {get; private set;}

    public bool IsSimulation {get; set;}

    public bool IsAiPlayer {get; set;}

    private SimulationObjects simulationObjects;

    private void Awake()
    {
        MyColor = new Color(Random.value, Random.value, Random.value, 1.0f);
        this.GetComponent<SpriteRenderer>().color = MyColor;
        
        SetValidRandomDirection();
        transform.eulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(GetGridMoveDirectionVector()) - 90);

        IsSimulation = false;
    }
    public bool WrappedMode;
    public BoxCollider2D Collider;

    private bool isSimuRunning = false;

    private void Start()
    {                        
        if (!IsSimulation)
        {
            prevLength = 0;                   
            score = 0;
            Alive = true;
            gridMoveTimer = 0f;
            Health = startingHealth * 100;
            WrappedMode = true;

            simulationObjects = ScriptableObject.CreateInstance<SimulationObjects>();
            simulationObjects.Init(LayerMask.NameToLayer("Simulation" + this.ID));

            scoreTXT = GameObject.Find("Score" + ID).GetComponent<TextMeshProUGUI>();
            scoreTXT.text = "Score: " + score;     

            GridMoveTimerMax = 0.25f;
            slider = GameObject.Find("Slider" + ID).GetComponent<Slider>();
            slider.value = startingHealth;            
        }        
        GridPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y); 
        Collider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        switch (Alive)
        {
            case true:
                HandleInput();
                HandleGridMovement();                
                if (checkOutOfBounds())
                {
                    if (WrappedMode)
                    {
                        setWrappedPosition();
                    }
                    else
                    {
                        Alive = false;
                    }
                }
                break;
            case false:
                break;
        }        
    }
    private void setWrappedPosition()
    {
        if (GridPosition.x < -GameAssets.i.Width)
        {
            GridPosition = new Vector2Int((int)GameAssets.i.Width, (int)transform.position.y);
        }
        if (GridPosition.x > GameAssets.i.Width)
        {
            GridPosition = new Vector2Int((int)-GameAssets.i.Width, (int)transform.position.y);
        }
        if (GridPosition.y > GameAssets.i.Height)
        {
            GridPosition = new Vector2Int((int)transform.position.x, (int)-GameAssets.i.Height);
        }
        if (GridPosition.y < -GameAssets.i.Height)
        {
            GridPosition = new Vector2Int((int)transform.position.x, (int)GameAssets.i.Height);
        }
        transform.position = new Vector3(GridPosition.x, GridPosition.y);
    }
    private void UpdateHealthBar()
    {
        if (snakeAte)
        {
            Health = startingHealth * 100;
        }
        else
        {
            Health -= 1;
            if (Health <= 0)
            {
                Alive = false;
            }
        }
        if (!IsSimulation)
        {
            slider.value = Health / 100;
        }        
    }
    private void die()
    {
        Alive = false;
        Debug.Log("Snake " + this.name + " died");
        for (int i = 0; i < snakeBodyPartList.Count; i++)
        {
            snakeBodyPartList[i].removeCollider();
        }
        Collider.enabled = false;
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

    private bool checkOutOfBounds()
    {
        if (GridPosition.x > GameAssets.i.Width)
        {
            return true;
        }
        if (GridPosition.x < -GameAssets.i.Width)
        {
            return true;
        }
        if (GridPosition.y > GameAssets.i.Height)
        {
            return true;
        }
        if (GridPosition.y < -GameAssets.i.Height)
        {
            return true;
        }
        return false;
    }

    private void HandleInput()
    {   
        if (this.IsSimulation)
        {
            return;
        }
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

    public List<Direction> GetPossibleActions()
    {
        isSimuRunning = true;
        var possibleActions = new List<Direction>();
        Debug.Log("Current Grid Position: " + GridPosition);        
        foreach(Direction direction in System.Enum.GetValues(typeof(Direction)))
        {
            Undo.RecordObject(this, "PossibleActions");
            PrefabUtility.RecordPrefabInstancePropertyModifications(this);
            
            //maybe check if direction is possible because of head direction                       
            gridMoveDirection = direction;
            ExecuteMovement();
            var occupied = Physics2D.OverlapBox(GridPosition, new Vector2(0.5f, 0.5f), 0);
            var collisionDetected = occupied != null && 
                occupied.gameObject.CompareTag("Player") && 
                occupied.gameObject.CompareTag("Body");
            // Debug.Log("Trying direction " + direction);
            // Debug.Log("New Grid Position: " + GridPosition);
            // Debug.Log("Simulation Collision detected: " + collisionDetected);
            if (this.Alive && !collisionDetected)
            {
                possibleActions.Add(direction);
            }
            else
            {
                Debug.Log("Collision detected and snake is " + (this.Alive ? "alive" : "dead"));
            } 
            Undo.PerformUndo();
            //Debug.Log("Undo Position: " + GridPosition);
        }
        isSimuRunning = false;
        return possibleActions;
    }

    public void HandleGridMovement()
    { 
        gridMoveTimer += Time.deltaTime;
        if (gridMoveTimer >= GridMoveTimerMax)
        {                      
            gridMoveTimer -= GridMoveTimerMax;

            if (this.IsAiPlayer && !this.IsSimulation)
            {    
                simulationObjects.SetSimuObjects();                

                var snakes = simulationObjects.GetSnakes();
                var you = snakes.Where(x=> x.ID == this.ID).FirstOrDefault();
                // var enemy = snakes.Where(x => x.ID != this.ID).FirstOrDefault();
                // var alphaBeta = new AlphaBeta(new List<Snake>(){ you, enemy});
                var test = you.GetPossibleActions();
                Debug.Log("Possible Actions: " + test.Count);
            }

            ExecuteMovement();
            
        }
    }

    private void ExecuteMovement()
    {        
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

    private void CreateSnakeBodyPart()
    {
        snakeBodyPartList.Add(new SnakeBodyPart(snakeBodyPartList.Count, this.MyColor));
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

   private void createSnakeBody()
    {
        snakeBodyPartList.Add(new SnakeBodyPart(snakeBodyPartList.Count, this.MyColor));
        //GameObject snakeBodyGameObject = new GameObject("SnakeBody", typeof(SpriteRenderer));        
        //snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.snakeBodySprite;
        //snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = -snakeBodyTransformList.Count;
        //snakeBodyPartList.Add(snakeBodyGameObject.transform);
    }
   private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision detected between " + this.gameObject.name + " and " + collision.gameObject.name);
        if (collision.CompareTag("Scoring"))
        {
            score++;
            if (!this.IsSimulation)
            {
                scoreTXT.text = "Score: " + score;    
            }            
            grow();
            // createSnakeBody();
            Destroy(collision.gameObject);
        }
        if (collision.CompareTag("Speed"))
        {
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
            int enemyID = collision.gameObject.GetComponent<Snake>().ID;

            checkHeadToHeadWinner(this.ID, enemyID);
            
        }
        if (collision.CompareTag("Body"))
        {
            Debug.Log("Body Collision");
            die();             
        }
        if (collision.CompareTag("Wall"))
        {
            // Game Over!
            Debug.Log("Wall Collision");
            die();
        }
    }

    private void checkHeadToHeadWinner(int playerID, int enemyID)
    {
        Snake playerSnake = this;
        Snake enemySnake = null;

            GameObject[] possibleEnemySnakeObjects = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject possibleEnemySnakeObject in possibleEnemySnakeObjects)
            {
                Snake possibleEnemySnake = possibleEnemySnakeObject.GetComponent<Snake>();
                if (possibleEnemySnake.ID == enemyID)
                {                    
                    enemySnake = possibleEnemySnake;
                }
            }
        
        if (playerSnake.snakeBodySize <= enemySnake.snakeBodySize)
        { 
            die();
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

    public class SnakeBodyPart
    {
        private SnakeMovePosition snakeMovePosition;
        private Transform transform;
        private Color myColor;       
        private GameObject snakeBodyGameObject;
        public Vector3 TransformPosition {get {return transform.position;}}
        public SnakeBodyPart(int bodyIndex, Color color)
        {
            myColor = color;
            snakeBodyGameObject = new GameObject("SnakeBody", typeof(SpriteRenderer));
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.snakeBodySpriteVertical;
            transform = snakeBodyGameObject.transform;            
            snakeBodyGameObject.GetComponent<SpriteRenderer>().color = color;
        }
        public void SetSnakeMovePosition(SnakeMovePosition snakeMovePosition)
        {
            this.snakeMovePosition = snakeMovePosition;
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
            snakeBodyGameObject.tag = "Body";
            transform.position = new Vector3(snakeMovePosition.GetGridPosition().x, snakeMovePosition.GetGridPosition().y);
        }
        public void removeCollider()
        {
            Destroy(snakeBodyGameObject.GetComponent<BoxCollider2D>());
        }
        public Vector2Int GetGridPosition()
        {
            return snakeMovePosition.GetGridPosition();
        }
    }
    public class SnakeMovePosition
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
