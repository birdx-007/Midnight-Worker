using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelPreviewer : MonoBehaviour
{
    public Text curLevelName;
    public List<Sprite> levelOverviewSprites;
    public Image curLevelOverview;
    void Start()
    {
        //levelOverviewSprites = new List<Sprite>(GlobalTerminal.Instance.Global_MaxLevelIndex);
        UpdateDisplay();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            GlobalTerminal.Instance.Global_LevelIndex = Math.Clamp(GlobalTerminal.Instance.Global_LevelIndex - 1, 1, GlobalTerminal.Instance.Global_UnlockedLevelIndex);
            UpdateDisplay();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            GlobalTerminal.Instance.Global_LevelIndex = Math.Clamp(GlobalTerminal.Instance.Global_LevelIndex + 1, 1, GlobalTerminal.Instance.Global_UnlockedLevelIndex);
            UpdateDisplay();
        }

    }
    public void UpdateDisplay()
    {
        curLevelName.text = (GlobalTerminal.Instance.Global_LevelIndex / 10).ToString() + (GlobalTerminal.Instance.Global_LevelIndex % 10).ToString();
        curLevelOverview.sprite = levelOverviewSprites[GlobalTerminal.Instance.Global_LevelIndex - 1];
    }
}
