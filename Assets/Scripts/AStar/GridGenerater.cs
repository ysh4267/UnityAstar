using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerater : MonoBehaviour
{
    public GameObject groundPrefab;
    GameObject parentGrid;
    public Vector2 gridWorldSize;
    public Node[,] grid;
    public List<GameObject> gridObject = new List<GameObject>();
    private void Awake()
    {
    }

    public bool CreateGrid(int _x, int _y)
    {
        gridWorldSize.x = _x;
        gridWorldSize.y = _y;
        grid = new Node[(int)gridWorldSize.x, (int)gridWorldSize.y];
        Vector3 worldBottomLeft = Vector3.zero - Vector3.right * gridWorldSize.x / 2f - Vector3.forward * gridWorldSize.y / 2f;
        //홀수 / 짝수
        float tempX = ((gridWorldSize.x + 1) / 2) % 2 == 1 ? 0.25f : -0.25f;
        float tempY = ((gridWorldSize.y + 1) / 2) % 2 == 1 ? 0.25f : -0.25f;
        for (int x = 0; x < (int)gridWorldSize.x; x++)
        {
            for (int y = 0; y < (int)gridWorldSize.y; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x + 0.5f) + Vector3.forward * (y + 0.5f);
                GameObject obj = Instantiate(groundPrefab, new Vector3((0f - gridWorldSize.x / 4f) + (float)x / 2 + tempX, 0.1f, (0f - gridWorldSize.y / 4f) + (float)y / 2 + tempY), Quaternion.identity);
                grid[x, y] = new Node(obj, true, x, y);
                gridObject.Add(obj);
            }
        }
        return true;
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        int[,] temp = { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };
        bool[] walkableUDLR = new bool[4];
        for (int i = 0; i < 4; i++)
        {
            int checkX = node.gridX + temp[i, 0];
            int checkY = node.gridY + temp[i, 1];
            if (checkX >= 0 && checkX < (int)gridWorldSize.x && checkY >= 0 && checkY < (int)gridWorldSize.y)
            {
                if (grid[checkX, checkY].walkable)
                    walkableUDLR[i] = true;
                neighbours.Add(grid[checkX, checkY]);
            }
        }
        for (int i = 0; i < 4; i++)
        {
            if (walkableUDLR[i] || walkableUDLR[(i + 1) % 4])
            {
                int checkX = node.gridX + temp[i, 0] + temp[(i + 1) % 4, 0];
                int checkY = node.gridY + temp[i, 1] + temp[(i + 1) % 4, 1];
                if (checkX >= 0 && checkX < (int)gridWorldSize.x && checkY >= 0 && checkY < (int)gridWorldSize.y)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Node StartNode
    {
        get
        {
            grid[0, 0].start = true;
            grid[0, 0].ChangeColor = Color.Lerp(Color.blue, Color.white, 0.2f);
            return grid[0, 0];
        }
    }

    public Node EndNode
    {
        get
        {
            grid[(int)gridWorldSize.x - 1, (int)gridWorldSize.y - 1].end = true;
            grid[(int)gridWorldSize.x - 1, (int)gridWorldSize.y - 1].ChangeColor = Color.Lerp(Color.red, Color.white, 0.2f);
            return grid[(int)gridWorldSize.x - 1, (int)gridWorldSize.y - 1];
        }
    }
}
