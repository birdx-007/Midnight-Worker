using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapReader : MonoBehaviour
{
    public static MapReader Instance;
    public List<TextAsset> maps;
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

    public string GetCurrentMapJson()
    {
        return maps[GlobalTerminal.Instance.Global_LevelIndex - 1].text;
    }
}
