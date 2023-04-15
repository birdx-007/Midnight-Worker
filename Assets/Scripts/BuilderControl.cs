using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

public class BuilderControl : MonoBehaviour
{
    public bool isEditing = false;
    public bool isEditingBank = false;
    private int currentEditingBankTotalCount = 0;
    public bool isEditingEnemy = false;
    private EnemyState currentEditingEnemyState = EnemyState.Sleep;
    private List<Vector2Int> currentEditingEnemyWaypoints;
    private Vector2Int mapCenter;
    private int mouseX;
    private int mouseY;
    public GameObject building1Prefab;
    public GameObject building2Prefab;
    public GameObject building3Prefab;
    public GameObject wall1Prefab;
    public GameObject bankPrefab;
    public GameObject enemyPrefab;
    private Dictionary<Vector2Int, Transform> objectDictionary;
    public void Initiate()
    {
        objectDictionary = new Dictionary<Vector2Int, Transform>();
        BuildAllfromMapData();
    }
    void Start()
    {
        
    }
    void Update()
    {
        if (isEditing)
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                isEditing = false;
                Blackbroad.map.SaveMap();
                return;
            }
            mouseX = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x);
            mouseY = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            Vector2Int mouseVector = new Vector2Int(mouseX, mouseY);
            if (Input.anyKeyDown && !IsOccupied())
            {
                if (!isEditingBank && !isEditingEnemy)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1))
                    {
                        BuildBuilding(mouseVector, 1);
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha2))
                    {
                        BuildBuilding(mouseVector, 2);
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha3))
                    {
                        BuildBuilding(mouseVector, 3);
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha9))
                    {
                        // Wall1
                        BuildBuilding(mouseVector, 9);
                    }
                    else if (Input.GetKeyDown(KeyCode.B))
                    {
                        // bank
                        isEditingBank = true;
                        currentEditingBankTotalCount = 0;
                        Debug.Log("begin editing a bank!");
                    }
                    else if(Input.GetKeyDown(KeyCode.E))
                    {
                        // enemy
                        isEditingEnemy = true;
                        currentEditingEnemyState = EnemyState.Sleep;
                        currentEditingEnemyWaypoints = new List<Vector2Int>();
                        Debug.Log("begin editing an enemy!");
                    }
                }
                else if(isEditingBank)
                {
                    int keyNumber = 0;
                    if (Input.GetKeyDown(KeyCode.Alpha0) ||
                        Input.GetKeyDown(KeyCode.Alpha1) ||
                        Input.GetKeyDown(KeyCode.Alpha2) ||
                        Input.GetKeyDown(KeyCode.Alpha3) ||
                        Input.GetKeyDown(KeyCode.Alpha4) ||
                        Input.GetKeyDown(KeyCode.Alpha5) ||
                        Input.GetKeyDown(KeyCode.Alpha6) ||
                        Input.GetKeyDown(KeyCode.Alpha7) ||
                        Input.GetKeyDown(KeyCode.Alpha8) ||
                        Input.GetKeyDown(KeyCode.Alpha9))
                    {
                        if (Input.GetKeyDown(KeyCode.Alpha1))
                        {
                            keyNumber = 1;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha2))
                        {
                            keyNumber = 2;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha3))
                        {
                            keyNumber = 3;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha4))
                        {
                            keyNumber = 4;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha5))
                        {
                            keyNumber = 5;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha6))
                        {
                            keyNumber = 6;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha7))
                        {
                            keyNumber = 7;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha8))
                        {
                            keyNumber = 8;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha9))
                        {
                            keyNumber = 9;
                        }
                        currentEditingBankTotalCount *= 10;
                        currentEditingBankTotalCount += keyNumber;
                    }
                    else if (Input.GetKeyDown(KeyCode.Space))
                    {
                        isEditingBank = false;
                        BuildBank(mouseVector, currentEditingBankTotalCount);
                        Debug.Log("Add bank with" + currentEditingBankTotalCount + "coins, at:" + mouseVector);
                        currentEditingBankTotalCount = 0;
                    }
                }
                else if(isEditingEnemy)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha0) ||
                        Input.GetKeyDown(KeyCode.Alpha1) ||
                        Input.GetKeyDown(KeyCode.Alpha2) ||
                        Input.GetKeyDown(KeyCode.Alpha3))
                    {
                        EnemyState enemyState = EnemyState.Sleep;
                        if (Input.GetKeyDown(KeyCode.Alpha1))
                        {
                            enemyState = EnemyState.FixedPatrol;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha2))
                        {
                            enemyState = EnemyState.RandomPatrol;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha3))
                        {
                            enemyState = EnemyState.ChasePlayer;
                        }
                        currentEditingEnemyState = enemyState;
                        Debug.Log("enemyState changes to " + enemyState);
                    }
                    else if (Input.GetKeyDown(KeyCode.P))
                    {
                        if(currentEditingEnemyState==EnemyState.FixedPatrol)
                        {
                            currentEditingEnemyWaypoints.Add(mouseVector);
                            Debug.Log("Add new enemy waypoint:" + mouseVector);
                        }
                    }
                    else if(Input.GetKeyDown(KeyCode.Space))
                    {
                        isEditingEnemy = false;
                        CreateEnemy(mouseVector, currentEditingEnemyState, currentEditingEnemyWaypoints);
                        Debug.Log("Add enemy with state " + currentEditingEnemyState + "at " + mouseVector + " with waypoints: " + currentEditingEnemyWaypoints);
                        currentEditingEnemyState = EnemyState.Sleep;
                        currentEditingEnemyWaypoints = new List<Vector2Int>();
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    Delete(mouseVector);
                }
            }
        }
    }
    void BuildAllfromMapData()
    {
        foreach(MapBuildingData data in Blackbroad.map.mapBuildingList)
        {
            Vector2Int pos = new Vector2Int(data.posX, data.posY);
            BuildBuildingInGameScene(pos, data.variety);
        }
        foreach(MapBankData data in Blackbroad.map.mapBankList)
        {
            Vector2Int pos = new Vector2Int(data.posX, data.posY);
            BuildBankInGameScene(pos, data.totalCoins);
        }
        Debug.Log("Build all map objects done!");
    }
    public List<PoliceControl> CreateAllEnemiesfromMapData()
    {
        List<PoliceControl> policeControls = new List<PoliceControl>();
        foreach (MapEnemyData data in Blackbroad.map.mapEnemyList)
        {
            Vector2Int pos = new Vector2Int(data.posX, data.posY);
            policeControls.Add(CreateEnemyInGameScene(pos, data.enemyState, data.waypoints));
        }
        Debug.Log("Creates all enemies done!");
        return policeControls;
    }
    bool IsOccupied()
    {
        if(isEditingEnemy && currentEditingEnemyState == EnemyState.FixedPatrol)
        {
            return false;
        }
        return Blackbroad.map.IsOccupied(mouseX, mouseY);
    }
    void Delete(Vector2Int pos)
    {
        Blackbroad.map.BuildBuilding(pos.x, pos.y, 0);
        var bankControl = objectDictionary[pos].gameObject.GetComponent<BankControl>();
        if (bankControl != null)
        {
            var toBeDeleted = Blackbroad.map.mapBankList.Where(data => (data.posX == pos.x && data.posY == pos.y)).ToList();
            foreach(MapBankData data in toBeDeleted)
            {
                Blackbroad.map.mapBankList.Remove(data);
            }
        }
        var policeControl = objectDictionary[pos].gameObject.GetComponent<PoliceControl>(); ;
        if( policeControl != null )
        {
            var toBeDelete = Blackbroad.map.mapEnemyList.Where(data => (data.posX == pos.x && data.posY == pos.y)).ToList();
            foreach(MapEnemyData data in toBeDelete)
            {
                Blackbroad.map.mapEnemyList.Remove(data);
            }
        }
        Destroy(objectDictionary[pos].gameObject);
        objectDictionary.Remove(pos);
    }
    void BuildBuilding(Vector2Int pos, short variety)
    {
        Blackbroad.map.BuildBuilding(pos.x, pos.y, variety);
        BuildBuildingInGameScene(pos,variety);
    }
    void BuildBuildingInGameScene(Vector2Int pos,short variety)
    {
        switch (variety)
        {
            case 1:
                GameObject building1 = Instantiate(building1Prefab, (Vector2)pos, Quaternion.identity);
                building1.transform.SetParent(gameObject.transform);
                objectDictionary.Add(pos, building1.transform);
                break;
            case 2:
                GameObject building2 = Instantiate(building2Prefab, (Vector2)pos, Quaternion.identity);
                building2.transform.SetParent(gameObject.transform);
                objectDictionary.Add(pos, building2.transform);
                break;
            case 3:
                GameObject building3 = Instantiate(building3Prefab, (Vector2)pos, Quaternion.identity);
                building3.transform.SetParent(gameObject.transform);
                objectDictionary.Add(pos, building3.transform);
                break;
            case 9:
                GameObject wall1 = Instantiate(wall1Prefab, (Vector2)pos, Quaternion.identity);
                wall1.transform.SetParent(gameObject.transform);
                objectDictionary.Add(pos, wall1.transform);
                break;
            default:
                Debug.LogError(variety + " at (" + pos.x + "," + pos.y + ") does not represents building!");
                break;
        }
    }
    void BuildBank(Vector2Int pos,int totalCoins)
    {
        Blackbroad.map.BuildBank(pos.x,pos.y, totalCoins);
        BuildBankInGameScene(pos, totalCoins);
    }
    void BuildBankInGameScene(Vector2Int pos,int totalCoins)
    {
        GameObject bank = Instantiate(bankPrefab, (Vector2)pos, Quaternion.identity);
        bank.transform.SetParent(gameObject.transform);
        objectDictionary.Add(pos, bank.transform);
        BankControl bankControl = bank.GetComponent<BankControl>();
        bankControl.totalCoins = totalCoins;
    }
    PoliceControl CreateEnemy(Vector2Int pos, EnemyState state,List<Vector2Int> points)
    {
        Blackbroad.map.CreateEnemy(pos.x, pos.y, state, points);
        return CreateEnemyInGameScene(pos,state,points);
    }
    PoliceControl CreateEnemyInGameScene(Vector2Int pos, EnemyState state, List<Vector2Int> points)
    {
        GameObject enemy = Instantiate(enemyPrefab, (Vector2)pos, Quaternion.identity);
        enemy.transform.SetParent(gameObject.transform);
        objectDictionary.Add(pos, enemy.transform);
        PoliceControl policeControl = enemy.GetComponent<PoliceControl>();
        policeControl.SetState(state, points);
        return enemy.GetComponent<PoliceControl>();
    }
}
