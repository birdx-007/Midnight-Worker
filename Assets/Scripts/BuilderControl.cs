using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuilderControl : MonoBehaviour
{
    public bool isEditing = false;
    public Map map;
    private int mapWidth;
    private int mapHeight;
    private Vector2Int mapCenter;
    private int mouseX;
    private int mouseY;
    public GameObject building1Prefab;
    public GameObject building2Prefab;
    public GameObject building3Prefab;
    public GameObject wall1Prefab;
    public GameObject bankPrefab;
    private Dictionary<Vector2Int, Transform> buildingDictionary;
    public void Initiate(Map map)
    {
        buildingDictionary = new Dictionary<Vector2Int, Transform>();
        this.map = map;
        mapWidth = map.mapWidth;
        mapHeight = map.mapHeight;
        mapCenter = new Vector2Int(mapWidth / 2 + 1, mapHeight / 2 + 1);
        BuildAllfromArray();
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
                map.SaveMap();
                return;
            }
            mouseX = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x);
            mouseY = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            Vector2Int mouseVector = new Vector2Int(mouseX, mouseY);
            if (Input.anyKeyDown && !IsOccupied())
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    Build(mouseVector,1);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    Build(mouseVector,2);
                }
                else if(Input.GetKeyDown(KeyCode.Alpha3))
                {
                    Build(mouseVector,3);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha9))
                {
                    // Wall1
                    Build(mouseVector,9);
                }
                else if(Input.GetKeyDown(KeyCode.B))
                {
                    // bank
                    Build(mouseVector, -1);
                }
            }
            else
            {
                if(Input.GetKeyDown(KeyCode.Delete))
                {
                    Build(mouseVector,0);
                }
            }
        }
    }
    void BuildAllfromArray()
    {
        foreach(MapObjData data in map.mapList)
        {
            Vector2Int pos = new Vector2Int(data.arrayX-mapCenter.x, data.arrayY - mapCenter.y);
            Build(pos, data.variety);
        }
    }
    bool IsMouseOutOfMap()
    {
        return map.IsOutOfMap(mouseX, mouseY);
    }
    bool IsOccupied()
    {
        return map.IsOccupied(mouseX, mouseY);
    }
    void Build(Vector2Int pos, short variety)
    {
        map.Build(pos.x, pos.y, variety);
        switch (variety)
        {
            case 0: // ���
                Destroy(buildingDictionary[pos].gameObject);
                buildingDictionary.Remove(pos);
                break;
            case 1:
                GameObject building1 = Instantiate(building1Prefab, (Vector2)pos, Quaternion.identity);
                building1.transform.SetParent(gameObject.transform);
                buildingDictionary.Add(pos, building1.transform);
                break;
            case 2:
                GameObject building2 = Instantiate(building2Prefab, (Vector2)pos, Quaternion.identity);
                building2.transform.SetParent(gameObject.transform);
                buildingDictionary.Add(pos, building2.transform);
                break;
            case 3:
                GameObject building3 = Instantiate(building3Prefab, (Vector2)pos, Quaternion.identity);
                building3.transform.SetParent(gameObject.transform);
                buildingDictionary.Add(pos, building3.transform);
                break;
            case 9:
                GameObject wall1 = Instantiate(wall1Prefab, (Vector2)pos, Quaternion.identity);
                wall1.transform.SetParent(gameObject.transform);
                buildingDictionary.Add(pos, wall1.transform);
                break;
            case -1:
                GameObject bank = Instantiate(bankPrefab, (Vector2)pos, Quaternion.identity);
                bank.transform.SetParent(gameObject.transform);
                buildingDictionary.Add(pos, bank.transform);
                break;
            default:
                break;
        }
    }
}
