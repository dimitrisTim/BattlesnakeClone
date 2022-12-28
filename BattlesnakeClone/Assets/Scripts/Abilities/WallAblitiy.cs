using System;
using System.Collections;
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
            var brickPosition3d = new Vector3(endPosition.x, endPosition.y, 0);            
            lastWall = Instantiate(this.snake.wallPrefab, this.snake.transform.position, Quaternion.identity);       
            if (direction.y == 0)
            {
                lastWall.transform.Rotate(0, 0, 90);
            }
            StartCoroutine(AnimateMovement(lastWall, brickPosition3d));
            used = true;
            this.Activate();
        }           
    }

    private void DeleteBricskOutsideGrid(GameObject wall)
    {
        foreach (var brick in wall.GetComponentsInChildren<Transform>())
        {
            if (IsPositionOutsideGrid(brick.position))
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
                var t = (Time.time - startTime) / duration;
                // Lerp the object's position towards the target position
                obj.transform.position = Vector3.Slerp(obj.transform.position, target, t);
                DeleteBricskOutsideGrid(obj);
            }
            else
            {
                break;
            }       
            // Wait for the next frame
            yield return null;
        }

        // Set the object's position to the target position
        if (obj != null) obj.transform.position = target;        
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
