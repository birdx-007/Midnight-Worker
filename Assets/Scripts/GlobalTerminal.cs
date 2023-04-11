using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTerminal : MonoBehaviour
{
    public static GlobalTerminal Instance;

    public int Global_LevelIndex = 1;

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
}
