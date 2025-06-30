using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void StartGameScene()
    {
        SceneManager.LoadScene("TrainScene");
    }

    public void QuitGame()
    {
        Debug.Log("Quit button pressed!"); 
        Application.Quit();
    }
}

