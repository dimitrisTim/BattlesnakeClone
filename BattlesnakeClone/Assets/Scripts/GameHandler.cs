using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private Snake snake;   
    // Start is called before the first frame update
    private void Start()
    {        
        Debug.Log("Start() called");
                
        LevelGrid levelGrid = new LevelGrid(5, 5); 
        snake.Setup(levelGrid);
        levelGrid.Setup(snake);
    }
}
