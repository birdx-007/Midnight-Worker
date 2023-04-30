using JetBrains.Annotations;
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
    public MapBuildingData(int X,int Y, short variety)
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
public class MapBankData
{
    public MapBankData(int X,int Y,int coins)
    {
        totalCoins = coins;
        posX = X;
        posY = Y;
    }
    public int totalCoins;
    public int posX;
    public int posY;
}

[Serializable]
public class MapEnemyData
{
    public MapEnemyData(int X, int Y, EnemyType type, EnemyState state, List<Vector2Int> points = null)
    {
        enemyType = type;
        enemyState = state;
        waypoints = points;
        posX = X;
        posY = Y;
    }
    public EnemyType enemyType;
    public EnemyState enemyState;
    public List<Vector2Int> waypoints;
    public int posX;
    public int posY;
}

public class Map : ISerializationCallbackReceiver
{
    [SerializeField] public int mapWidth = 25;
    [SerializeField] public int mapHeight = 25;
    static public short[,] mapArray;
    [SerializeField] public List<MapBuildingData> mapBuildingList;
    [SerializeField] public List<MapBankData> mapBankList;
    [SerializeField] public List<MapEnemyData> mapEnemyList;
    private string jsonFilePath;
    private string mapJson;
    public string MapJson { get { return mapJson; } set { mapJson = value; } }
    static private Vector2Int mapCenter; // ��ʾ��������(0,0)��map�е��±�λ��
    static public Vector2Int MapCenter { get { return mapCenter; } }
    public Map(int levelIndex)
    {
        mapArray = new short[mapWidth, mapHeight];
        mapCenter = new Vector2Int(mapWidth / 2 + 1, mapHeight / 2 + 1);
        jsonFilePath = Application.dataPath + "/Resources/Level-" + levelIndex.ToString() + "/map.json";
        LoadMap();
    }
    public void OnBeforeSerialize() // before save
    {
        mapBuildingList.Clear();
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                if (mapArray[i, j] > 0) // building's variety should greater than 0.
                {
                    mapBuildingList.Add(new MapBuildingData(i - mapCenter.x, j - mapCenter.y, mapArray[i, j]));
                }
            }
        }
    }
    public void OnAfterDeserialize() // after load
    {
        mapArray = new short[mapWidth, mapHeight];
        mapCenter = new Vector2Int(mapWidth / 2 + 1, mapHeight / 2 + 1);
        foreach (MapBuildingData data in mapBuildingList)
        {
            mapArray[data.posX + mapCenter.x, data.posY + mapCenter.y] = data.variety;
        }
        foreach(MapBankData data in mapBankList)
        {
            mapArray[data.posX + mapCenter.x, data.posY + mapCenter.y] = -1;
        }
        foreach(MapEnemyData data in mapEnemyList)
        {
            mapArray[data.posX + mapCenter.x, data.posY + mapCenter.y] = -2;
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
        mapBuildingList = new List<MapBuildingData>();
        mapBankList = new List<MapBankData>();
        mapEnemyList = new List<MapEnemyData>();
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
    public int GetTargetCoinCount()
    {
        int targetCoinCount = 0;
        foreach (MapBankData data in mapBankList)
        {
            mapArray[data.posX + mapCenter.x, data.posY + mapCenter.y] = -1;
            targetCoinCount += data.totalCoins;
        }
        return targetCoinCount;
    }
    public bool isReachable(int X, int Y)
    {
        return mapArray[X + mapCenter.x, Y + mapCenter.y] == 0
            || mapArray[X + mapCenter.x, Y + mapCenter.y] == -2; // enemy
    }
    public bool IsOutOfMap(int posX, int posY)
    {
        return posX + mapCenter.x < 0
            || posX + mapCenter.x >= mapWidth
            || posY + mapCenter.y < 0
            || posY + mapCenter.y >= mapHeight;
    }
    public bool IsOccupied(int X, int Y)
    {
        if (IsOutOfMap(X, Y))
        {
            return true;
        }
        return mapArray[X + mapCenter.x, Y + mapCenter.y] != 0;
    }
    public void BuildBuilding(int X, int Y, short variety)
    {
        if (!IsOccupied(X, Y) || variety == 0)
        {
            mapArray[X + mapCenter.x, Y + mapCenter.y] = variety;
        }
    }
    public void BuildBank(int X, int Y,int totalCoins)
    {
        if (!IsOccupied(X, Y))
        {
            mapArray[X + mapCenter.x, Y + mapCenter.y] = -1;
        }
        mapBankList.Add(new MapBankData(X, Y, totalCoins));
    }
    public void CreateEnemy(int X, int Y, EnemyType type, EnemyState state, List<Vector2Int> points = null)
    {
        if (!IsOccupied(X, Y))
        {
            mapArray[X + mapCenter.x, Y + mapCenter.y] = -2;
        }
        mapEnemyList.Add(new MapEnemyData(X, Y, type, state, points));
    }
}