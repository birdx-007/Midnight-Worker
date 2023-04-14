using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CongratulationsMenuControl : MonoBehaviour
{
    public SceneLoaderControl sceneLoader;
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BackToMenu();
        }
    }
    public void BackToMenu()
    {
        if (GlobalTerminal.Instance != null)
        {
            Destroy(GlobalTerminal.Instance.gameObject);
            GlobalTerminal.Instance = null;
        }
        sceneLoader.LoadSceneWithName("MainMenu");
    }
}
