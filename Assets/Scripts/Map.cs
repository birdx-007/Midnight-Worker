using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;

[Serializable]
public class MapObjData
{
    public MapObjData(short variety,int arrayX,int arrayY)
    {
        this.variety = variety;
        this.arrayX = arrayX;
        this.arrayY = arrayY;
    }
    public short variety;
    public int arrayX;
    public int arrayY;
}

public class Map : ISerializationCallbackReceiver
{
    public int mapWidth = 25;
    public int mapHeight = 25;
    public short[,] mapArray;
    [SerializeField] public List<MapObjData> mapList; // 为了序列化map使用的临时数组
    private string mapJson;
    public string MapJson { get { return mapJson; } set { mapJson = value; } }
    private Vector2Int mapCenter; // 表示世界坐标(0,0)在map中的下标位置
    public Vector2Int MapCenter { get { return mapCenter; } }
    public Map()
    {
        mapArray = new short[mapWidth, mapHeight];
        mapList = new List<MapObjData>();
        mapCenter = new Vector2Int(mapWidth / 2 + 1, mapHeight / 2 + 1);
        LoadMap();
    }
    public void OnBeforeSerialize()
    {
        mapList.Clear();
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                if (mapArray[i, j] != 0)
                {
                    mapList.Add(new MapObjData(mapArray[i, j],i,j));
                }
            }
        }
    }
    public void OnAfterDeserialize()
    {
        foreach(MapObjData data in mapList)
        {
            mapArray[data.arrayX, data.arrayY] = data.variety;
        }
    }
    public void SaveMap()
    {
        mapJson = JsonUtility.ToJson(this, true);
        string jsonFilePath = Application.dataPath + "/Resources/" + "map.json";
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
        string jsonFilePath = Application.dataPath + "/Resources/"+"map.json";
        using (StreamReader sr = File.OpenText(jsonFilePath))
        {
            mapJson = sr.ReadToEnd();
            sr.Close();
        }
        JsonUtility.FromJsonOverwrite(mapJson, this);
        Debug.Log("LoadMap done!");
    }
    public bool IsOutOfMap(int X, int Y)
    {
        return X + mapCenter.x < 0
            || X + mapCenter.x >= mapWidth
            || Y + mapCenter.y < 0
            || Y + mapCenter.y >= mapHeight;
    }
    public bool IsOccupied(int X, int Y)
    {
        if (IsOutOfMap(X, Y))
        {
            return true;
        }
        return mapArray[X + (int)mapCenter.x, Y + (int)mapCenter.y] != 0;
    }
    public void Build(int X, int Y, short variety)
    {
        if (!IsOutOfMap(X, Y))
        {
            mapArray[X + (int)mapCenter.x, Y + (int)mapCenter.y] = variety;
        }
    }
}