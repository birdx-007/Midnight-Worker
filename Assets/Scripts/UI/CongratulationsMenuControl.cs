using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CongratulationsMenuControl : MonoBehaviour
{
    public SceneLoaderControl sceneLoader;
    private void Awake()
    {
        BGMPlayer.Instance.PlayBGM(BGMType.BGM_Menu);
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
        SFXPlayer.Instance.PlaySFX(SFXType.MENU_BUTTON_PRESSED);
        sceneLoader.LoadSceneWithName("MainMenu");
    }
}
