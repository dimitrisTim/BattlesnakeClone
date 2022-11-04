using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    private Vector2Int gridMoveDirection;
    private Vector2Int gridPosition;
    private Rigidbody2D body;
    private List<KeyInputDirection> keysAndDirections;
    [SerializeField] private float moveSpeed = 1f;

    private void Awake()
    {
        gridPosition = new Vector2Int(0, 0);
        gridMoveDirection = new Vector2Int(1, 0);
        body = GetComponent<Rigidbody2D>();
        keysAndDirections = new List<KeyInputDirection>();
        keysAndDirections.Add(new KeyInputDirection { code = KeyCode.UpArrow, x = 0, y = 1 });
        keysAndDirections.Add(new KeyInputDirection { code = KeyCode.DownArrow, x = 0, y = -1 });
        keysAndDirections.Add(new KeyInputDirection { code = KeyCode.LeftArrow, x = -1, y = 0 });
        keysAndDirections.Add(new KeyInputDirection { code = KeyCode.RightArrow, x = 1, y = 0 });
    }

    private void Update()
    {   
        HandleInput();             
        HandleGridMovement();        
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
                SetRoundedPosition();
                if (gridMoveDirection.x != -keyInput.x && gridMoveDirection.y != -keyInput.y)
                {
                    gridMoveDirection.x = keyInput.x;
                    gridMoveDirection.y = keyInput.y;
                }
            }
        }
    }

    private void SetRoundedPosition()
    {
        var roundedPosition = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        transform.position = roundedPosition;
    }

    private void HandleGridMovement()
    {                      
        body.velocity = new Vector2(gridMoveDirection.x, gridMoveDirection.y) * moveSpeed;
        transform.eulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(gridMoveDirection) - 90);  
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
        return gridPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision detected");
        if (collision.CompareTag("Scoring"))
        {
            Destroy(collision.gameObject);
        }
    }
}
