using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;


public class GameCore : MonoBehaviour
{
    public int thisGameSceneSort = 3;
    public MapGraph myMapGraph;

    public GameObject AmbulanceObject;
    public float ambulanceSpeed = 7f;

    public bool gameRunning = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RanderPoint();
        resetAmbulancePosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameRunning)
        {
            swapAmbulanceMoving();
        }
    }

    public void RanderPoint()
    {
        if (myMapGraph == null || myMapGraph.points.Count == 0)
        {
            Debug.LogWarning("MapGraph 沒有點可以渲染！");
            return;
        }

        // 創建一個空物件來存放所有 LineRenderer
        GameObject lineParent = new GameObject("LineRenderers");

        // 遍歷 MapGraph 裡的所有點
        for (int i = 0; i < myMapGraph.points.Count; i++)
        {
            Point currentPoint = myMapGraph.points[i];

            // 遍歷當前點的連接點
            foreach (int linkedIndex in currentPoint.linkingPointSort)
            {
                if (linkedIndex < 0 || linkedIndex >= myMapGraph.points.Count)
                {
                    Debug.LogWarning($"索引 {linkedIndex} 超出範圍，無法連接！");
                    continue;
                }

                Point linkedPoint = myMapGraph.points[linkedIndex];

                // 動態創建 LineRenderer 物件
                GameObject lineObject = new GameObject($"Line_{i}_to_{linkedIndex}");
                lineObject.transform.parent = lineParent.transform; // 設置父物件

                LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

                // 設定 LineRenderer 屬性
                lineRenderer.startWidth = 0.1f;
                lineRenderer.endWidth = 0.1f;
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                lineRenderer.startColor = Color.black;
                lineRenderer.endColor = Color.black;
                lineRenderer.useWorldSpace = true;
                lineRenderer.positionCount = 2; // 兩個點

                // 設置線條的兩個點
                Vector3 startPos = new Vector3(currentPoint.x, currentPoint.y, 0);
                Vector3 endPos = new Vector3(linkedPoint.x, linkedPoint.y, 0);

                lineRenderer.SetPosition(0, startPos);
                lineRenderer.SetPosition(1, endPos);
            }
        }

    }

    public void gameStart()
    {
        gameRunning = true; 
    }

    public void LoadNextGame()
    {

    }
    public void ReloadGame()
    {

    }

    public void resetAmbulancePosition()
    {
        AmbulanceObject.transform.localPosition = myMapGraph.getPointXY(0);
    }
    public void swapAmbulanceMoving()
    {
        AmbulanceObject.transform.localPosition = (AmbulanceObject.transform.localPosition + new Vector3(0.01f * ambulanceSpeed * Time.deltaTime,0,0));
        swapAmbulanceJudgement();
    }
    public void swapAmbulanceJudgement()
    {
        if (AmbulanceObject.transform.position.x >= myMapGraph.getPointXY(1).x)
        {
            //Game End.
            gameRunning = false;
            //Open the end canvas and else
        }
    }
}

[System.Serializable]
public class MapGraph
{
    public List<Point> points = new List<Point>();

    public Vector2 getPointXY(int sort)
    {
        return new Vector2(points[sort].x, points[sort].y);
    }
}

[System.Serializable]
public class Point
{
    /// <summary>
    /// pointType
    /// 0=起點
    /// 1=終點
    /// 2=節點
    /// </summary>
    public int pointType;
    public List<int> linkingPointSort = new List<int>();
    public float x;
    public float y;
}