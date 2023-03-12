using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public int X,Y;
    public int F,G,H;//F(路径长度)=G(距起点长度)+H(距终点长度)
    public PathNode parent;
    public bool isStop;
    public PathNode(int x, int y, bool isStop)
    {
        this.X = x;
        this.Y = y;
        this.isStop = isStop;
    }
}
public class PathSearcher
{
    private int mapWidth;
    private int mapHeight;
    public PathNode[,] nodesMap;
    public List<PathNode> openList = new List<PathNode>();
    public List<PathNode> closeList = new List<PathNode>();
    public PathSearcher(short[,] mapArray, int mapWidth, int mapHeight)
    {
        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;
        nodesMap = new PathNode[mapWidth, mapHeight];
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                nodesMap[i, j] = new PathNode(i, j, mapArray[i, j] != 0);
            }
        }
    }
    public List<PathNode> FindWay(Vector2Int start, Vector2Int end)
    {
        if (start.x < 0 | start.x >= mapWidth |
            start.y < 0 | start.y >= mapHeight |
            end.x < 0 | end.x >= mapWidth |
            end.y < 0 | end.y >= mapHeight) //判断起点，终点是否合法
        {
            return null;
        }
        if (nodesMap[start.x, start.y].isStop |
            nodesMap[end.x, end.y].isStop)//判断起点或终点是否可到达
        {
            return null;
        }
        closeList.Clear();
        openList.Clear();
        nodesMap[start.x, start.y].parent = null;
        nodesMap[start.x, start.y].F = 0;
        nodesMap[start.x, start.y].G = 0;
        nodesMap[start.x, start.y].H = 0;
        closeList.Add(nodesMap[start.x, start.y]);
        while (true)//遍历四周方格
        {
            FindOpenlist(start.x, start.y + 1, 1, nodesMap[start.x, start.y], nodesMap[end.x, end.y]);
            FindOpenlist(start.x, start.y - 1, 1, nodesMap[start.x, start.y], nodesMap[end.x, end.y]);
            FindOpenlist(start.x + 1, start.y, 1, nodesMap[start.x, start.y], nodesMap[end.x, end.y]);
            FindOpenlist(start.x - 1, start.y, 1, nodesMap[start.x, start.y], nodesMap[end.x, end.y]);
            if (openList.Count == 0)
            {
                return null;
            }
            openList.Sort((PathNode a, PathNode b) => { if (a.F >= b.F) return 1; else return -1; });//排序选出当前最短距终点的点
            closeList.Add(openList[0]);
            start.Set(openList[0].X, openList[0].Y);
            openList.RemoveAt(0);
            if (nodesMap[start.x, start.y] == nodesMap[end.x, end.y])
            {
                List<PathNode> way = new List<PathNode>();
                way.Clear();
                way.Add(nodesMap[end.x, end.y]);
                while (nodesMap[end.x, end.y].parent != null)
                {
                    way.Add(nodesMap[end.x, end.y].parent);
                    end.Set(nodesMap[end.x, end.y].parent.X, nodesMap[end.x, end.y].parent.Y);
                }
                way.Reverse();//反转列表使路径从开始到终点
                return way;
            }

        }
    }
    private void FindOpenlist(int x, int y, int g, PathNode parent, PathNode end)
    {
        if (x < 0 | x >= mapWidth |
            y < 0 | y >= mapHeight)
        {
            return;
        }
        PathNode Node = nodesMap[x, y];
        if (Node == null |
            Node.isStop |
            closeList.Contains(Node) |
            openList.Contains(Node))
            return;
        Node.parent = parent;
        Node.G = parent.G + g;
        Node.H = Mathf.Abs(end.X - Node.X) + Mathf.Abs(end.Y - Node.Y);
        Node.F = Node.G + Node.H;
        openList.Add(Node);
    }
}
