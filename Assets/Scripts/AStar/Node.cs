using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public GameObject ground;
    public bool walkable;
    public int gridX;
    public int gridY;

    public bool start;
    public bool end;

    public int gCost;
    public int hCost;
    public Node parent;

    public Node(GameObject _ground, bool _walkable, int _gridX, int _gridY)
    {
        ground = _ground;
        walkable = _walkable;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int fCost
    {
        get{ return gCost + hCost; }
    }
    
    public Color ChangeColor 
    {
        set{ground.GetComponent<MeshRenderer>().material.color = value;}
    }
}
