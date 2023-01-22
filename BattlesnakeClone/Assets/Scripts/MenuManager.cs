using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] int startScene;
    private string sceneName = null;

    private void Update()
    {
        sceneName = SceneManager.GetActiveScene().name;
        //Debug.Log(Time.timeSinceLevelLoad);

        if (sceneName == "Credits")
        {
            if (Time.timeSinceLevelLoad > 10)
            {
                Debug.Log("App quit");
       
                Application.Quit();
                Debug.Break();
            }
        }
    }
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("Credits");
    }
}
