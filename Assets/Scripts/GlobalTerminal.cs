using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTerminal : MonoBehaviour
{
    public static GlobalTerminal Instance;

    public int Global_LevelIndex = 1;
    public int Global_UnlockedLevelIndex = 1;
    public int Global_MaxLevelIndex = 10;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void UpdateUnlockedLevelIndex_OnWin()
    {
        if (Global_LevelIndex == Global_UnlockedLevelIndex)
        {
            Global_UnlockedLevelIndex = Math.Clamp(Global_UnlockedLevelIndex + 1, 1, Global_MaxLevelIndex);
        }
    }
}
