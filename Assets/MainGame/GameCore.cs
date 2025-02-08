using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameCore : MonoBehaviour
{
    public MapGraph myMapGraph;

    public GameObject AmbulanceObject;
    public GameObject GameStartPanel;
    public GameObject GameEndPanel;

    public float ambulanceSpeed = 7f;

    public bool gameRunning = false;

    public float maxHp = 100f;
    public float hp = 100f;//100~0 
    public int hpStatement = 4; // 0=死亡，
    public Sprite[] uiSprite = new Sprite[5];
    public SpriteRenderer heartbeatChart;

    public float bloodLooseRate = 1f;
    public float bloodMax = 40f; //等同於秒數
    public float bloodNow = 40f; //現在剩餘血量
    public Image bloodPackImage;

    public Image[] starImages = new Image[3];
    public Sprite emptyStar;
    public Sprite star;
    public int gameRate = 0; // -1~2, -1代表關卡失敗, 0~2代表1~3星
    public int starNumber = 0;
    // 1星-通過關卡 2星-50%以上時間病人安穩狀態 3星-85%以上時間病人安穩狀態
    //安穩狀態 = 病人生命跡象狀態
    public float[] gameStatementRate = new float[5];
    public float finalGameRateResult = 0;

    public TextMeshProUGUI O2TextMesh;

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
            HpStatementSync();

            BloodLoose();

            //這邊優先優化掉
            O2Sync();


            if (Input.GetKeyDown(KeyCode.U))
            {
                swapTestHpMinus5();
            }

            gameStatementRate[hpStatement] += Time.deltaTime;
        }
    }

    public void RanderPoint()
    {
        if (myMapGraph == null || myMapGraph.points.Count == 0)
        {
            Debug.LogWarning("AkWarning - MapGraph 沒有點可以渲染！");
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
                    Debug.LogWarning($"AkWarning - 索引 {linkedIndex} 超出範圍，無法連接！");
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

    public void BloodLoose()
    {
        bloodNow -= Time.deltaTime * bloodLooseRate;
        bloodPackImage.fillAmount = bloodNow / bloodMax;
    }

    public void BloodLooseJudgement()
    {
        //判定失血與增加note
    }

    public void ChangeBloodPack()
    {
        bloodNow = bloodMax; //滿血持續一定時間，不要馬上就開始流血，算是一點小無敵效果
    }

    public void gameStart()
    {
        gameRunning = true;
        GameStartPanel.SetActive(false);
    }
    public void gameEnd()
    {
        gameRunning = false;
        GameEndPanel.SetActive(true);
        finalGameRateResultCal();
        finalGameExecute();
    }

    public void LoadNextGame()
    {
        if (SceneManager.sceneCount < SceneManager.GetActiveScene().buildIndex +1)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
        }
        else
        {
            //沒有下一關了 禁止按鈕被按下 諤諤諤諤 我覺得這個判斷可以早一點執行 但算了啦現在先這樣寫就好我好懶惰喔
        }
    }
    public void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void LoadLobby()
    {
        SceneManager.LoadScene(0);
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
            gameEnd();
            //Open the end canvas and else
        }
    }

    public void HpStatementSync()
    {
        float hpRate = hp / maxHp;
        if (hpRate > 0.85f)
        {
            hpStatement = 4;
        }
        else if (hpRate > 0.65f)
        {
            hpStatement = 3;
        }
        else if (hpRate > 0.4f)
        {
            hpStatement = 2;
        }
        else if (hpRate > 0f)
        {
            hpStatement = 1;
        }
        else
        {
            hpStatement = 0;
        }
        heartbeatChart.sprite = uiSprite[hpStatement];

        //增加視覺效果
    }

    public void swapTestHpMinus5()
    {
        hp -= 10f;
    }

    public void finalGameRateResultCal()
    {
        float swapBaseNum =0;
        for (int i = 0; i< gameStatementRate.Length; i++)
        {
            swapBaseNum += gameStatementRate[i];
        }
        float swapBaseNum2 = gameStatementRate[3] + gameStatementRate[4];

        finalGameRateResult = swapBaseNum2 / swapBaseNum;
    }
    public void finalGameExecute()
    {
        
        if (finalGameRateResult > 0.8f)
        {
            starNumber = 3;
        }
        else if (finalGameRateResult > 0.5f)
        {
            starNumber = 2;
        }
        else
        {
            starNumber = 1;
        }

        for (int i = 0; i < starNumber; i++)
        {
            starImages[i].sprite = star;
        }
    }

    public void O2Sync()
    {
        O2TextMesh.text = "O2: " + (int)hp;
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