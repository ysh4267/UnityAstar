using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public GameObject line;
    LineRenderer lineRander;

    public GridGenerater gridGen;
    public Node start, end;

    public bool finding;

    public bool wallDetect;

    private void Start()
    {
        lineRander = line.GetComponent<LineRenderer>();
    }

    public void ResetGrid() {
        lineRander.positionCount = 0;
        foreach (var item in gridGen.gridObject)
        {
            Destroy(item);
        }
        gridGen.grid = null;
    }

    public void StartGrid(int x, int y)
    {
        StopCoroutine("FindPath");
        line.SetActive(false);
        finding = false;

        bool success = gridGen.CreateGrid(x, y);

        Debug.Log(success);
        if (success)
        {
            start = gridGen.StartNode;
            end = gridGen.EndNode;
        }
        StartCoroutine(UpdateWallInfo());
        StartCoroutine(UpdateWallInfo());
    }

    IEnumerator UpdateWallInfo() {
        yield return null;
        foreach (var item in gridGen.grid)
        {
            if (item.ground.GetComponent<NodeObject>().isWall) {
                item.ChangeColor = Color.Lerp(Color.black, Color.white, 0.2f);
                item.walkable = false;
            }
        }
        yield return null;
    }

    void Update() {

    }

    // public void UpdateWallInfo() {
    //     foreach (var item in gridGen.grid)
    //     {
    //         if (item.ground.GetComponent<NodeObject>().isWall) {
    //             item.ChangeColor = Color.Lerp(Color.black, Color.white, 0.2f);
    //             item.walkable = false;
    //         }
    //     }
    // }

    public void StartFinding(bool search)
    {
        StopCoroutine("FindPath");
        line.SetActive(false);
        finding = false;
        if(search) StartCoroutine("FindPath");        
    }

    IEnumerator FindPath()
    {
        //길을 찾는중인가.
        finding = true;
        //길을 찾았는가.
        bool pathSuccess = false;

        //직후 탐색 대상 리스트
        List<Node> openSet = new List<Node>();
        //탐색완료 해쉬 리스트
        HashSet<Node> closedSet = new HashSet<Node>();
        //시작지점을 탐색 대상에 추가
        openSet.Add(start);

        //탐색대상이 없을때 까지
        while (openSet.Count > 0)
        {
            //탐색대상중 가장 오래된 대상을 가리킴
            Node currentNode = openSet[0];
            //탐색대상 리스트를 한번 순회 하면서
            for(int i = 1; i<openSet.Count; i++)
            {
                //탐색대상 리스트의 노드의 F값(G + H)이 짧거나 같다면 / 휴리스틱값이 더 작다면
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    //가리키고 있는 노드 변경
                    currentNode = openSet[i];
                }
            }

            //가장 A*의 조건에 가까운 노드를 탐색대상에서 지우고
            //탐색완료 대상에 추가
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            //도착했는가
            if (currentNode == end)
            {
                pathSuccess = true;
                break;
            }

            //finding이 꺼졌는지 파악(일시정지용)
            //시각화용 딜레이
            //yield return new WaitForSeconds(0.01f);

            //탐색한 노드의 색 변경
            if (currentNode != start)
                currentNode.ChangeColor = Color.Lerp(Color.cyan, Color.white, 0.2f);

            //직전에 탐색한 노드의 이웃한 노드들
            foreach (Node neighbour in gridGen.GetNeighbours(currentNode))
            {
                //이웃노드가 벽이거나 / 이미 탐색을 마친곳이면 스킵
                if (!neighbour.walkable  || closedSet.Contains(neighbour))
                {
                    continue;
                }

                //이웃한 노드들에 새 G코스트를 업데이트 하기위한 계산값
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                //새 G값이 기존의 것보다 작거나 아예 기존값이 없으면 업데이트
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    //G값
                    neighbour.gCost = newMovementCostToNeighbour;
                    //H값
                    neighbour.hCost = GetDistance(neighbour, end);
                    //해당 노드까지가는 경로중 가장 효율적이었던 노드가 부모노드
                    neighbour.parent = currentNode;

                    //기존에 탐색리스트에 들어있지 않았다면
                    if (!openSet.Contains(neighbour))
                    {
                        //탐색리스트에 추가
                        openSet.Add(neighbour);
                        //벽이 아니고 도착지점도 아니라면 녹색으로
                        if (neighbour.walkable && !neighbour.end)
                            neighbour.ChangeColor = Color.Lerp(Color.green, Color.white, 0.2f);
                    }
                }
            }
        }
            yield return new WaitUntil(() => finding);

        //도착했다면
        if (pathSuccess)
        {
            //각 노드들의 위치값을 배열로 넘겨 선을 그림
            DrawingLine(RetracePath(start, end));
        }
        finding = false;
    }

    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i - 1].ground.transform.position + Vector3.up * 0.1f);
            }
            directionOld = directionNew;
        }
        waypoints.Add(start.ground.transform.position + Vector3.up * 0.1f);
        return waypoints.ToArray();
    }

    public void DrawingLine(Vector3[] waypoints)
    {
        line.SetActive(true);
        lineRander.positionCount = waypoints.Length;
        for (int i = 0; i < waypoints.Length; i++)
        {
            lineRander.SetPosition(i, waypoints[i]);
        }
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
