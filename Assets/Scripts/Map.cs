using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;

[Serializable]
public class MapBuildingData
{
    public MapBuildingData(short variety,int X,int Y)
    {
        this.variety = variety;
        this.posX = X;
        this.posY = Y;
    }
    public short variety;
    public int posX;
    public int posY;
}

[Serializable]
public class MapEnemyData
{
    public MapEnemyData(int X,int Y)
    {
        this.posX = X;
        this.posY = Y;
    }
    public int posX;
    public int posY;
}

public class Map : ISerializationCallbackReceiver
{
    [SerializeField] static public int mapWidth = 25;
    [SerializeField] static public int mapHeight = 25;
    static public short[,] mapArray;
    [SerializeField] public List<MapBuildingData> mapBuildingList;
    [SerializeField] public List<MapEnemyData> mapEnemyList;
    private string jsonFilePath;
    private string mapJson;
    public string MapJson { get { return mapJson; } set { mapJson = value; } }
    static private Vector2Int mapCenter; // ��ʾ��������(0,0)��map�е��±�λ��
    static public Vector2Int MapCenter { get { return mapCenter; } }
    public Map(int levelIndex)
    {
        mapArray = new short[mapWidth, mapHeight];
        mapBuildingList = new List<MapBuildingData>();
        mapEnemyList = new List<MapEnemyData>();
        mapCenter = new Vector2Int(mapWidth / 2 + 1, mapHeight / 2 + 1);
        jsonFilePath = Application.dataPath + "/Resources/Level-" + levelIndex.ToString() + "/map.json";
        LoadMap();
    }
    public void OnBeforeSerialize()
    {
        mapBuildingList.Clear();
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                if (mapArray[i, j] != 0)
                {
                    mapBuildingList.Add(new MapBuildingData(mapArray[i, j], i - mapCenter.x, j - mapCenter.y));
                }
            }
        }
    }
    public void OnAfterDeserialize()
    {
        foreach (MapBuildingData data in mapBuildingList)
        {
            mapArray[data.posX + mapCenter.x, data.posY + mapCenter.y] = data.variety;
        }
    }
    public void SaveMap()
    {
        mapJson = JsonUtility.ToJson(this, true);
        using (StreamWriter sw = new StreamWriter(jsonFilePath))
        {
            sw.WriteLine(mapJson);
            sw.Close();
            sw.Dispose();
        }
        Debug.Log("SaveMap done!");
    }
    public void LoadMap()
    {
        if (File.Exists(jsonFilePath))
        {
            using (StreamReader sr = File.OpenText(jsonFilePath))
            {
                mapJson = sr.ReadToEnd();
                sr.Close();
            }
        }
        else
        {
            mapJson = "";
        }
        JsonUtility.FromJsonOverwrite(mapJson, this);
        Debug.Log("LoadMap done!");
    }
    static public bool isReachable(int X, int Y)
    {
        if (IsOutOfMap(X, Y) || IsOccupied(X, Y))
        {
            return false;
        }
        return true;
    }
    static public bool IsOutOfMap(int posX, int posY)
    {
        return posX + mapCenter.x < 0
            || posX + mapCenter.x >= mapWidth
            || posY + mapCenter.y < 0
            || posY + mapCenter.y >= mapHeight;
    }
    static public bool IsOccupied(int X, int Y)
    {
        if (IsOutOfMap(X, Y))
        {
            return true;
        }
        return mapArray[X + mapCenter.x, Y + mapCenter.y] != 0;
    }
    public void Build(int X, int Y, short variety)
    {
        if (!IsOutOfMap(X, Y))
        {
            mapArray[X + mapCenter.x, Y + mapCenter.y] = variety;
        }
    }
}