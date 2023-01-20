using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private TextMeshProUGUI timerObj;

    private List<GameObject> snakes;

    private void Awake()
    {
        GameAssets.i.Width = width;
        GameAssets.i.Height = height;

        snakes = new List<GameObject>();
        var newSnake = Instantiate(snakePrefab, new Vector3(
            Random.Range(-width, 0), Random.Range(-height, 0)), 
            Quaternion.identity);
        newSnake.GetComponent<Snake>().KeyStrategy = new WASDStrategy();
        newSnake.GetComponent<Snake>().ID = 1;
        newSnake.GetComponent<Snake>().IsAiPlayer = true;
        snakes.Add(newSnake);

        var newSnake2 = Instantiate(snakePrefab, new Vector3(
            Random.Range(0, width), Random.Range(0, height)), 
            Quaternion.identity);
        newSnake2.GetComponent<Snake>().KeyStrategy = new ArrowsStrategy();
        newSnake2.GetComponent<Snake>().ID = 2;
        snakes.Add(newSnake2);
        
        timerObj = GameObject.Find("TimerText").GetComponent<TextMeshProUGUI>();
        SetPlayerNameColors();
    }

    private void SetPlayerNameColors()
    {
        foreach (var snake in snakes)
        {
            var playerName = GameObject.Find("Player" + snake.GetComponent<Snake>().ID + "Name").GetComponent<TextMeshProUGUI>();
            playerName.color = snake.GetComponent<Snake>().MyColor;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {   
        SetSnakeScripts(false);
        pauseMenu.SetActive(false);        
        isPaused = false;
        CountDown();        
    }

    private void CountDown()
    {
        StartCoroutine(CountDownCoroutine());
    }

    private IEnumerator CountDownCoroutine()
    {        
        var textToShow = new List<string> {"3", "2", "1", "GO!"};    
        foreach (var text in textToShow)
        {
            timerObj.text = text;
            yield return new WaitForSeconds(1f);
        }
        timerObj.text = "";
        SetSnakeScripts(true);   
    }

    /// <summary>
    /// Can prevent snakes from moving before the game starts
    /// </summary>
    private void SetSnakeScripts(bool enabled)
    {        
        foreach (var snake in snakes)
        {
            snake.GetComponent<Snake>().enabled = enabled;
        }        
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            ShowPauseMenu();        
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
        //Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ReloadGame()
    {
        //Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
