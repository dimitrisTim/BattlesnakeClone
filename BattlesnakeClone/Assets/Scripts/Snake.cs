using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using CodeMonkey.Utils;

public class Snake : MonoBehaviour
{
    private Vector2Int gridMoveDirection;
    public Vector2Int GridPosition {get; private set;}
    private Rigidbody2D body;
    private List<KeyInputDirection> keysAndDirections;
    [SerializeField] private float moveSpeed = 1f;

    private int score;
    [SerializeField] Transform txtController;
    private TextMeshProUGUI scoreTXT;
    private int length = 0;
    private List<Vector2Int> snakeMovePositionList;
    private float gridMoveTimer = 0f;
    private float gridMoveTimerMax = 0.1f;
    public bool Alive {get; set;}

    private void Awake()
    {
        GridPosition = new Vector2Int(0, 0);
        gridMoveDirection = new Vector2Int(1, 0);
        keysAndDirections = new List<KeyInputDirection>();
        keysAndDirections.Add(new KeyInputDirection { code = KeyCode.UpArrow, x = 0, y = 1 });
        keysAndDirections.Add(new KeyInputDirection { code = KeyCode.DownArrow, x = 0, y = -1 });
        keysAndDirections.Add(new KeyInputDirection { code = KeyCode.LeftArrow, x = -1, y = 0 });
        keysAndDirections.Add(new KeyInputDirection { code = KeyCode.RightArrow, x = 1, y = 0 });

        score = 0;
        scoreTXT = txtController.Find("Score").GetComponent<TextMeshProUGUI>();
        scoreTXT.text = "Score: " + score;
        Alive = true;
        gridMoveTimer = 0f;
        gridMoveTimerMax = 0.1f;
        snakeMovePositionList = new List<Vector2Int>();
    }

    private void Update()
    {       
        if (Alive)
        {
            HandleInput();             
            HandleGridMovement();    
        }
        else
        {            
            Destroy(this.gameObject);
        }
    }

    private void HandleInput()
    {        
        CheckKeyInputs();
    }

    private void CheckKeyInputs()
    {
        foreach (var keyInput in keysAndDirections)
        {
            if (Input.GetKeyDown(keyInput.code))
            {
                if (gridMoveDirection.x != -keyInput.x && gridMoveDirection.y != -keyInput.y)
                {
                    gridMoveDirection.x = keyInput.x;
                    gridMoveDirection.y = keyInput.y;
                }
            }
        }
    }

 private void HandleGridMovement()
    {
        gridMoveTimer += Time.deltaTime;
        if (gridMoveTimer >= gridMoveTimerMax)
        {
            gridMoveTimer -= gridMoveTimerMax;
            snakeMovePositionList.Insert(0, GridPosition);
            //Debug.Log("position:" + gridPosition);            
            GridPosition += gridMoveDirection;
            transform.position = new Vector3(GridPosition.x, GridPosition.y);
            transform.eulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(gridMoveDirection) - 90);
            if (snakeMovePositionList.Count >= length + 1)
            {
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
            }
            for (int i = 0; i < snakeMovePositionList.Count; i++)
            {
                Vector2Int snakeMovePosition = snakeMovePositionList[i];
                World_Sprite worldSprite = World_Sprite.Create(new Vector3(snakeMovePosition.x, snakeMovePosition.y), Vector3.one * 0.5f, Color.white);
                Debug.Log("spawned at:" + snakeMovePosition.x + snakeMovePosition.y);
                FunctionTimer.Create(worldSprite.DestroySelf, gridMoveTimerMax);
            }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision detected");
        if (collision.CompareTag("Scoring"))
        {
            score++;
            scoreTXT.text = "Score: " + score;
            grow();
            Destroy(collision.gameObject);
        }
    }
    private void grow()
    {
        length++;
    }
}
