using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackbroad
{
    static public Map map;
    static public Vector2Int playerIntPosition;
    static public List<Vector2Int> enemyIntPositions;
    public Blackbroad(int levelIndex)
    {
        Initiate(levelIndex);
    }
    public void Initiate(int levelIndex)
    {
        map = new Map(levelIndex); // Now mapArray loaded
        PathSearcher.Initiate();
        playerIntPosition = new Vector2Int();
        enemyIntPositions = new List<Vector2Int>();
    }
}
