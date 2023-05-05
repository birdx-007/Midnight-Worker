using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectMenuControl : MonoBehaviour
{
    public SceneLoaderControl sceneLoader;

    public void BackToMenu()
    {
        sceneLoader.LoadSceneWithName("MainMenu");
    }
    public void PlayGame()
    {
        sceneLoader.LoadSceneWithName("LevelScene");
    }
}
