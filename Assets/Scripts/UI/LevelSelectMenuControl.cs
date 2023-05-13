using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectMenuControl : MonoBehaviour
{
    public SceneLoaderControl sceneLoader;

    public void BackToMenu()
    {
        SFXPlayer.Instance.PlaySFX(SFXType.MENU_BUTTON_PRESSED);
        sceneLoader.LoadSceneWithName("MainMenu");
    }
    public void PlayGame()
    {
        SFXPlayer.Instance.PlaySFX(SFXType.MENU_BUTTON_PRESSED);
        BGMPlayer.Instance.FadeOutBGM();
        sceneLoader.LoadSceneWithName("LevelScene");
    }
}
