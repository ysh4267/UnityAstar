using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MazeRenderer : MonoBehaviour
{

    [SerializeField]
    [Range(1, 100)]
    public int width = 10;

    [SerializeField]
    [Range(1, 100)]
    public int height = 10;

    [SerializeField]
    private float size = 1f;

    [SerializeField]
    private Transform wallPrefab = null;

    [SerializeField]
    private Transform floorPrefab = null;

    [SerializeField]
    public Main main = null;

    [SerializeField]
    public Slider sliderW = null;
    public Slider sliderH = null;
    List<GameObject> objectList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetWidth() {
        width = (int)sliderW.value;
    }

    public void SetHeight() {
        height = (int)sliderH.value;
    }

    public void ResetMaze()
    {
        foreach (var item in objectList)
        {
            Destroy(item);
        }
        main.ResetGrid();
    }

    public void GenerateMaze()
    {
        var maze = MazeGenerator.Generate(width, height);
        Draw(maze);
        main.StartGrid(width * 2 - 1, height * 2 - 1);
    }

    private void Draw(WallState[,] maze)
    {

        var floor = Instantiate(floorPrefab, transform);
        floor.localScale = new Vector3(width, 1, height);
        objectList.Add(floor.gameObject);
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                var cell = maze[i, j];
                var position = new Vector3(-width / 2 + i, 0, -height / 2 + j);

                if (cell.HasFlag(WallState.UP))
                {
                    var topWall = Instantiate(wallPrefab, transform) as Transform;
                    topWall.position = position + new Vector3(0, 0, size / 2);
                    topWall.localScale = new Vector3(size, topWall.localScale.y, topWall.localScale.z);
                    objectList.Add(topWall.gameObject);

                }

                if (cell.HasFlag(WallState.LEFT))
                {
                    var leftWall = Instantiate(wallPrefab, transform) as Transform;
                    leftWall.position = position + new Vector3(-size / 2, 0, 0);
                    leftWall.localScale = new Vector3(size, leftWall.localScale.y, leftWall.localScale.z);
                    leftWall.eulerAngles = new Vector3(0, 90, 0);
                    objectList.Add(leftWall.gameObject);

                }

                if (i == width - 1)
                {
                    if (cell.HasFlag(WallState.RIGHT))
                    {
                        var rightWall = Instantiate(wallPrefab, transform) as Transform;
                        rightWall.position = position + new Vector3(+size / 2, 0, 0);
                        rightWall.localScale = new Vector3(size, rightWall.localScale.y, rightWall.localScale.z);
                        rightWall.eulerAngles = new Vector3(0, 90, 0);
                        objectList.Add(rightWall.gameObject);

                    }
                }

                if (j == 0)
                {
                    if (cell.HasFlag(WallState.DOWN))
                    {
                        var bottomWall = Instantiate(wallPrefab, transform) as Transform;
                        bottomWall.position = position + new Vector3(0, 0, -size / 2);
                        bottomWall.localScale = new Vector3(size, bottomWall.localScale.y, bottomWall.localScale.z);
                        objectList.Add(bottomWall.gameObject);

                    }
                }
            }

        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
