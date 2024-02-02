using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScr : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("start the game, you heard him");
        UnityEngine.SceneManagement.SceneManager.LoadScene("PrequelStory");
    }

    public void OpenOptions()
    {
        Debug.Log("no options to open");
    }

    public void ExitGame()
    {
        Debug.Log("Get out!");
        Application.Quit();
    }
}
