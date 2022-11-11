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
    private bool isPaused;
    public GameObject pauseMenu;

    // Start is called before the first frame update
    private void Start()
    {        
        pauseMenu.SetActive(false);
        isPaused = false;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            ShowPauseMenu();
        CheckSnakePosition();
    }

    public void ShowPauseMenu()
    {
        if (!snake.Alive)
        {
            pauseMenu.transform.Find("Resume").gameObject.SetActive(false);
        }        
        pauseMenu.SetActive(!isPaused);
        Time.timeScale = !isPaused ? 0f : 1f;
        isPaused = !isPaused;
    }

    public void ShowStartScreen()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ReloadGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
                StopEverything();
            }
        }
    }

    private void StopEverything()
    {
        snake.Alive = false;        
        snake.enabled = false;
        foodSpawner.enabled = false;
        Time.timeScale = 0f;
        ShowPauseMenu();
    }
}
