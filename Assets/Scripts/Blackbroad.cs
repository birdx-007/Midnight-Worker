using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackbroad
{
    public Map map;
    private PathSearcher pathSearcher;
    public Vector2Int playerIntPosition;
    public Vector2Int policeIntPosition;
    public Blackbroad()
    {
        Initiate();
    }
    public void Initiate()
    {
        map = new Map(); // Now mapArray loaded
        pathSearcher = new PathSearcher(map.mapArray, map.mapWidth, map.mapWidth);
        playerIntPosition = new Vector2Int();
        policeIntPosition = new Vector2Int();
    }
    public bool CanReach(int X, int Y)
    {
        if (map.IsOutOfMap(X, Y) || map.IsOccupied(X, Y))
        {
            return false;
        }
        return true;
    }
    public List<Vector2Int> FindWayTo(Vector2Int start,Vector2Int end)
    {
        var path = pathSearcher.FindWay(start + map.MapCenter, end + map.MapCenter);
        if (path == null || path.Count == 0)
        {
            return null;
        }
        List<Vector2Int> result = new List<Vector2Int>();
        foreach(var node in path)
        {
            Vector2Int nodePosition = new Vector2Int(node.X, node.Y) - map.MapCenter;
            result.Add(nodePosition);
        }
        return result;
    }
}
