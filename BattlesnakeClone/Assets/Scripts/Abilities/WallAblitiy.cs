using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class WallAbility : AbstractAbility
{
    public int Cooldown {get; private set;}
    private Snake snake;
    private float previousSpeed;
    private bool used = false;
    private GameObject lastWall;

    private void Awake()
    {
        this.snake = GetComponent<Snake>();
        this.Cooldown = 10;        
    }

    private void Update()
    {      
        if (this.snake.KeyStrategy.CheckAbilityKey() && !used)
        {   
            var endPosition = this.snake.GetNewGridPosition(3);
            var direction = this.snake.GetGridMoveDirectionVector();
            var middleBrickPosition3d = new Vector3(endPosition.x, endPosition.y, -0.5f);            
            lastWall = Instantiate(this.snake.wallPrefab, 
                        new Vector3(this.snake.transform.position.x, this.snake.transform.position.y, -2f), 
                        Quaternion.identity);
            SetWallColliders(lastWall, false);
            if (direction.y == 0)
            {
                lastWall.transform.Rotate(0, 0, 90);
            }
            StartCoroutine(AnimateMovement(lastWall, middleBrickPosition3d));
            used = true;
            this.Activate();
        }           
    }

    private void DeleteInvalidBricks(GameObject wall)
    {
        foreach (var brick in wall.GetComponentsInChildren<Transform>().Skip(1))
        {
            if (IsPositionOutsideGrid(brick.position) || IsPositionOccupied(brick.position))
            {
                Destroy(brick.gameObject);
            }
        }
    }

    private bool IsPositionOutsideGrid(Vector3 position)
    {
        var x = Math.Round(position.x);
        var y = Math.Round(position.y);
        return x > GameAssets.i.Width || 
                x < -GameAssets.i.Width || 
                y > GameAssets.i.Height || 
                y < -GameAssets.i.Height;
    }

    private bool IsPositionOccupied(Vector3 position)
    {
        var occupied = Physics2D.OverlapBox(position, new Vector2(0.1f, 0.1f), 0);
        return occupied != null && 
                occupied.gameObject.CompareTag("Player") && 
                position.z <= -0.48f && 
                position.z >= -0.52f;
    }

    private void SetWallColliders(GameObject wall, bool enabled)
    {
        wall.GetComponentsInChildren<Collider2D>().ToList().
                    ForEach(x => x.enabled = enabled);
    }

    private IEnumerator AnimateMovement(GameObject obj, Vector3 target)
    {
        // Calculate the distance to the target position
        float distance = Vector3.Distance(obj.transform.position, target);

        // Calculate the duration of the animation based on the distance and speed
        float duration = distance / 0.1f;

        // Set the starting time
        float startTime = Time.time;

        // Loop until the elapsed time is greater than the duration
        while (Time.time - startTime < duration)
        {
            if (obj != null) 
            {        
                if (obj.transform.position == target) 
                {
                    // When wall reaches lands in end-position, enable colliders
                    SetWallColliders(lastWall, true);
                    break;
                }
                var t = (Time.time - startTime) / duration;
                // Lerp the object's position towards the target position
                obj.transform.position = Vector3.Slerp(obj.transform.position, target, t);
                DeleteInvalidBricks(obj);
            }
            else
            {                
                break;
            }       
            // Wait for the next frame
            yield return null;
        }
        // If all bricks deleted/invalid, deactivate ability
        if (obj.transform.childCount == 0)            
            Deactivate();   
    }
    
    protected override void Activate()
    {
        Invoke(nameof(Deactivate), Cooldown);
    } 

    protected override void Deactivate()
    {
        Destroy(lastWall);
        Destroy(this);
    }
}
