using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuControl : MonoBehaviour
{
    public SceneLoaderControl sceneLoader;
    private bool _hasStarted;
    public Text startText;
    public StartMenuControl startMenu;
    void Start()
    {
        Initiate();
    }

    void Update()
    {
        if (!_hasStarted)
        {
            if (Input.anyKeyDown)
            {
                _hasStarted = true;
                startText.text = "TO THE CAUSE";
                startText.color = Color.yellow;
                startMenu.ShowStartMenu();
            }
        }
    }
    public void Initiate()
    {
        Time.timeScale = 1f;
        _hasStarted = false;
        startText.text = "PRESS ANY KEY TO START";
        startText.color = Color.white;
        startMenu.HideStartMenu();
    }
    public void PlayGame()
    {
        sceneLoader.LoadSceneWithName("LevelScene");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
