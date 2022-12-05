using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private GameObject snakePrefab;
    [SerializeField] private FoodSpawner foodSpawner;
    public int width;
    public int height;
    private float deathOffset = 0.2f;
    private bool isPaused;
    public GameObject pauseMenu;

    private void Awake()
    {
        GameAssets.i.Width = width;
        GameAssets.i.Height = height;

        var newSnake = Instantiate(snakePrefab, new Vector3(
            Random.Range(-width, 0), Random.Range(-height, 0)), 
            Quaternion.identity);
        newSnake.GetComponent<Snake>().KeyStrategy = new ArrowsStrategy();
        newSnake.GetComponent<Snake>().ID = 1;

        var newSnake2 = Instantiate(snakePrefab, new Vector3(
            Random.Range(0, width), Random.Range(0, height)), 
            Quaternion.identity);
        newSnake2.GetComponent<Snake>().KeyStrategy = new WASDStrategy();
        newSnake2.GetComponent<Snake>().ID = 2;
    }

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
        // if (!snake.GetComponent<Snake>().Alive)
        // {
        //     pauseMenu.transform.Find("Resume").gameObject.SetActive(false);
        // }        
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
        // var snakeCurrPosition = snake.transform.position;
        // if (snake.GetComponent<Snake>().Alive)
        // {
        //     if (snakeCurrPosition.x < -width - deathOffset   || 
        //         snakeCurrPosition.x > width  + deathOffset   || 
        //         snakeCurrPosition.y < -height - deathOffset  || 
        //         snakeCurrPosition.y > height + deathOffset)
        //     {
        //         StopEverything();
        //     }
        // }
    }

    private void StopEverything()
    {
        // snake.GetComponent<Snake>().Alive = false;        
        // snake.GetComponent<Snake>().enabled = false;
        // foodSpawner.enabled = false;
        // Time.timeScale = 0f;
        // ShowPauseMenu();
    }
}
