using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private Snake snake;
    [SerializeField] private FoodSpawner foodSpawner;
    public int width;
    public int height;
    private float deathOffset = 0.2f;
    

    // Start is called before the first frame update
    private void Start()
    {        
        Debug.Log("Start() called");
    }

    private void Update()
    {
        CheckSnakePosition();
    }

    private void CheckSnakePosition()
    {
        var snakeCurrPosition = snake.transform.position;
        if (snake.Alive)
        {
            if (snakeCurrPosition.x < -width - deathOffset   || 
                snakeCurrPosition.x > width  + deathOffset   || 
                snakeCurrPosition.y < -height - deathOffset  || 
                snakeCurrPosition.y > height + deathOffset)
            {
                Pause();
            }
        }
    }

    private void Pause()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 0f;
        snake.enabled = false;
        foodSpawner.enabled = false;
    }
}
