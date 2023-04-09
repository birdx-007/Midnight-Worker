using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public int X,Y;
    public int F,G,H;//F(·������)=G(����㳤��)+H(���յ㳤��)
    public PathNode parent;
    public bool isStop;
    public PathNode(int x, int y, bool isStop)
    {
        this.X = x;
        this.Y = y;
        this.isStop = isStop;
    }
}
static public class PathSearcher
{
    static public PathNode[,] nodesMap;
    static public List<PathNode> openList = new List<PathNode>();
    static public List<PathNode> closeList = new List<PathNode>();
    static public void Initiate()
    {
        nodesMap = new PathNode[Blackbroad.map.mapWidth, Blackbroad.map.mapHeight];
        for (int i = 0; i < Blackbroad.map.mapWidth; i++)
        {
            for (int j = 0; j < Blackbroad.map.mapHeight; j++)
            {
                bool isStop = !Blackbroad.map.isReachable(i - Map.MapCenter.x, j - Map.MapCenter.y);
                nodesMap[i, j] = new PathNode(i, j, isStop);
            }
        }
    }
    static public List<Vector2Int> FindWayTo(Vector2Int start, Vector2Int end)
    {
        var path = PathSearcher.FindWay(start + Map.MapCenter, end + Map.MapCenter);
        if (path == null || path.Count == 0)
        {
            return null;
        }
        List<Vector2Int> result = new List<Vector2Int>();
        foreach (var node in path)
        {
            Vector2Int nodePosition = new Vector2Int(node.X, node.Y) - Map.MapCenter;
            result.Add(nodePosition);
        }
        return result;
    }
    static private List<PathNode> FindWay(Vector2Int start, Vector2Int end)
    {
        if (start.x < 0 | start.x >= Blackbroad.map.mapWidth |
            start.y < 0 | start.y >= Blackbroad.map.mapHeight |
            end.x < 0 | end.x >= Blackbroad.map.mapWidth |
            end.y < 0 | end.y >= Blackbroad.map.mapHeight) //�ж���㣬�յ��Ƿ�Ϸ�
        {
            return null;
        }
        if (nodesMap[start.x, start.y].isStop |
            nodesMap[end.x, end.y].isStop)//�ж������յ��Ƿ�ɵ���
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
        while (true)//�������ܷ���
        {
            FindOpenlist(start.x, start.y + 1, 1, nodesMap[start.x, start.y], nodesMap[end.x, end.y]);
            FindOpenlist(start.x, start.y - 1, 1, nodesMap[start.x, start.y], nodesMap[end.x, end.y]);
            FindOpenlist(start.x + 1, start.y, 1, nodesMap[start.x, start.y], nodesMap[end.x, end.y]);
            FindOpenlist(start.x - 1, start.y, 1, nodesMap[start.x, start.y], nodesMap[end.x, end.y]);
            if (openList.Count == 0)
            {
                return null;
            }
            openList.Sort((PathNode a, PathNode b) => { if (a.F >= b.F) return 1; else return -1; });//����ѡ����ǰ��̾��յ�ĵ�
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
                way.Reverse();//��ת�б�ʹ·���ӿ�ʼ���յ�
                return way;
            }

        }
    }
    static private void FindOpenlist(int x, int y, int g, PathNode parent, PathNode end)
    {
        if (x < 0 | x >= Blackbroad.map.mapWidth |
            y < 0 | y >= Blackbroad.map.mapHeight)
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
