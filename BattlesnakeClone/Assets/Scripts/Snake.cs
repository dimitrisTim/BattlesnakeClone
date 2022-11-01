using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    private Vector2Int gridMoveDirection;
    private Vector2Int gridPosition;
    private float gridMoveTimer;
    public float gridMoveTimerMax;

    private void Awake()
    {
        gridPosition = new Vector2Int(0, 0);
        gridMoveTimer = gridMoveTimerMax;
        gridMoveDirection = new Vector2Int(1, 0);
    }

    private void Update()
    {   
        HandleInput();             
        HandleGridMovement();        
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (gridMoveDirection.y != -1)
            {
                gridMoveDirection.x = 0;
                gridMoveDirection.y = 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (gridMoveDirection.y != 1)
            {
                gridMoveDirection.x = 0;
                gridMoveDirection.y = -1;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (gridMoveDirection.x != 1)
            {
                gridMoveDirection.x = -1;
                gridMoveDirection.y = 0;
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (gridMoveDirection.x != -1)
            {
                gridMoveDirection.x = 1;
                gridMoveDirection.y = 0;
            }
        }
    }

    private void HandleGridMovement()
    {
        gridMoveTimer += Time.deltaTime;
        if (gridMoveTimer >= gridMoveTimerMax)
        {
            gridPosition += gridMoveDirection;
            gridMoveTimer -= gridMoveTimerMax;

            transform.position = new Vector3(gridPosition.x, gridPosition.y);
            transform.eulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(gridMoveDirection) - 90);
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
