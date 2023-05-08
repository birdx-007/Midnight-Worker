using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Save
{
    public int unlockedLevelIndex = 1;
}

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;

    public Save save;
    private string path;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        save = new Save();
        path = Path.Combine(Application.persistentDataPath, "save.json");
        Load();
    }
    public void Save()
    {
        save.unlockedLevelIndex = GlobalTerminal.Instance.Global_UnlockedLevelIndex;

        string json = JsonUtility.ToJson(save, false);
        using (StreamWriter sw = new StreamWriter(path))
        {
            sw.WriteLine(json);
            sw.Close();
            sw.Dispose();
        }
        Debug.Log("Save generated at " + path);
    }
    public void Load()
    {
        string json = "";
        if (File.Exists(path))
        {
            using (StreamReader sr = File.OpenText(path))
            {
                json = sr.ReadToEnd();
                sr.Close();
            }
        }
        JsonUtility.FromJsonOverwrite(json, save);

        GlobalTerminal.Instance.Global_UnlockedLevelIndex = save.unlockedLevelIndex;
        Debug.Log("Save loaded from " + path);
    }
}
