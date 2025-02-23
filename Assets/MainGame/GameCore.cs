using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using TMPro;
using Unity.Android.Gradle;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.VFX;

public class GameCore : MonoBehaviour
{
    public MapGraph myMapGraph;

    public GameObject AmbulanceObject;
    public GameObject GameStartPanel;
    public GameObject GameEndPanel;

    public float ambulanceSpeed = 7f;
    public int ambulanceMovingFromPoint;
    public int ambulanceMovingToPoint;
    public Vector2 theAmbulanceDirectionVector;

    public bool gameRunning = false;

    public float maxHp = 100f;
    public float hp = 100f;//100~0 
    public int hpStatement = 4; // 0=���`�A
    public Sprite[] uiSprite = new Sprite[5];
    public SpriteRenderer heartbeatChart;

    public float bloodLooseRate = 1f;
    public float bloodMax = 40f; //���P�����
    public float bloodNow = 40f; //�{�b�Ѿl��q 
    public Image bloodPackImage;
    public float bloodLooseingCount;

    public GameObject enemy;
    public GameObject referencePointMobMovingRangeA;
    public GameObject referencePointMobMovingRangeB;

    public Heartbeat heartbeat;

    public Animator damagedTipAnimator;
    public Animator bloodPackTipAnimator;

    public Sprite eventSprite_enemySpawn;
    public Sprite eventSprite_roadRock;

    public Image[] starImages = new Image[3];
    public Sprite emptyStar;
    public Sprite star;
    public int gameRate = 0; // -1~2, -1�N�����d����, 0~2�N��1~3�P
    public int starNumber = 0;

    public bool fail = false;

    public int comboCount = 0;

    // 1�P-�q�L���d 2�P-50%�H�W�ɶ��f�H�wí���A 3�P-85%�H�W�ɶ��f�H�wí���A
    //�wí���A = �f�H�ͩR��H���A
    public float[] gameStatementRate = new float[5];
    public float finalGameRateResult = 0;

    public TextMeshProUGUI O2TextMesh;
    public TextMeshProUGUI gameEndText;

    public int bpm = 120;
    
    public int theBPM
    {
        get { return bpm; }
        set { bpm = value; heartbeat.BPMChnage(bpm); }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RanderPoint();
        resetAmbulancePosition();
        ambulanceMovingFromPoint = 0;// set the start point

        //Swap
        ambulanceMovingToPoint = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameRunning)
        {
            AmbulanceMoving();
            //swapAmbulanceMoving();
            HpStatementSync();
            BloodLoose();
            
            
            //�o���u���u�Ʊ�
            O2Sync();

            /*
            int randomShit = Random.Range(1,11451);
            if(randomShit == 50)
            {
                InsEnemy();
            }*/

            if (Input.GetKeyDown(KeyCode.U))
            {
                swapTestHpMinus5();
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                damagedFunc();  
            }

            gameStatementRate[hpStatement] += Time.deltaTime;
        }
    }

    public void RanderPoint()
    {
        if (myMapGraph == null || myMapGraph.points.Count == 0)
        {
            Debug.LogWarning("AkWarning - MapGraph �S���I�i�H��V�I");
            return;
        }

        // �Ыؤ@�ӪŪ���Ӧs��Ҧ� LineRenderer
        GameObject lineParent = new GameObject("LineRenderers");

        // �M�� MapGraph �̪��Ҧ��I
        for (int i = 0; i < myMapGraph.points.Count; i++)
        {
            Point currentPoint = myMapGraph.points[i];

            // �M�����e�I���s���I
            foreach (int linkedIndex in currentPoint.linkingPointSort)
            {
                if (linkedIndex < 0 || linkedIndex >= myMapGraph.points.Count)
                {
                    Debug.LogWarning($"AkWarning - ���� {linkedIndex} �W�X�d��A�L�k�s���I");
                    continue;
                }

                Point linkedPoint = myMapGraph.points[linkedIndex];

                // �ʺA�Ы� LineRenderer ����
                GameObject lineObject = new GameObject($"Line_{i}_to_{linkedIndex}");
                lineObject.transform.parent = lineParent.transform; // �]�m������

                LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

                // �]�w LineRenderer �ݩ�
                lineRenderer.startWidth = 0.1f;
                lineRenderer.endWidth = 0.1f;
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                lineRenderer.startColor = Color.black;
                lineRenderer.endColor = Color.black;
                lineRenderer.useWorldSpace = true;
                lineRenderer.positionCount = 2; // ����I

                // �]�m�u��������I
                Vector3 startPos = new Vector3(currentPoint.x, currentPoint.y, 0);
                Vector3 endPos = new Vector3(linkedPoint.x, linkedPoint.y, 0);

                lineRenderer.SetPosition(0, startPos);
                lineRenderer.SetPosition(1, endPos);
            }
            int j = 0;
            //Render the liner event
            foreach (EventPoint eventPoint in currentPoint.theEventPoints)
            {
                Vector2 vec = new Vector2(myMapGraph.points[myMapGraph.points[i].linkingPointSort[0]].x - myMapGraph.points[i].x, myMapGraph.points[myMapGraph.points[i].linkingPointSort[0]].y - myMapGraph.points[i].y);
                vec = vec * eventPoint.atPos;
                GameObject gObj = new GameObject("Pointer Object From Point" + i + eventPoint.myEventType);
                gObj.transform.position = new Vector2( myMapGraph.points[i].x + vec.x, myMapGraph.points[i].y + vec.y);
                SpriteRenderer sr =  gObj.AddComponent<SpriteRenderer>();
                sr.sortingOrder = 10;

                Debug.Log(myMapGraph.points[ambulanceMovingFromPoint].theEventPoints[j].myEventType + "載入");
                switch (myMapGraph.points[ambulanceMovingFromPoint].theEventPoints[j].myEventType)
                {
                    case (EventPoint.eventType.enemySpawn):
                        sr.sprite = eventSprite_enemySpawn;
                        break;
                    case (EventPoint.eventType.roadRock):
                        sr.sprite = eventSprite_roadRock;
                        break;
                    default:
                        Debug.Log("未知事件");
                        break;
                }
                j++;
            }
        }

    }

    
    public void BloodLoose()
    {
        bloodNow -= Time.deltaTime * bloodLooseRate;
        bloodPackImage.fillAmount = bloodNow / bloodMax;

        if (bloodPackImage.fillAmount == 0)
        {
            bloodLooseingCount += Time.deltaTime;

            if (bloodLooseingCount - (bloodLooseingCount % 1) > bloodClug)
            {
                bloodClug++;
                BloodInjury();
            }
        }
        else
        {
            bloodLooseingCount = 0;
            bloodClug = 2;
        }

        if (bloodNow / bloodMax < 0.2f)
        {
            bloodPackTipAnimator.SetBool("Warning", true);
        }
        else
        {
            bloodPackTipAnimator.SetBool("Warning",false);
        }
    }

    int bloodClug = 0;
    public void BloodLooseJudgement()
    {
        //�P�w����P�W�[note
        if (bloodLooseingCount - (bloodLooseingCount % 1) > bloodClug)
        {
            bloodClug++;
        }
    }

    public void ChangeBloodPack()
    {
        bloodNow = bloodMax; 
        bloodPackImage.fillAmount = 1f;
    }

    public void BloodInjury()
    {
        Debug.Log("Loose Blood test message");
        hp -= 10;
    }
      public void BloodInjury(int hpHurt)
    {
        hp -= hpHurt;
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
            //�S���U�@���F �T����s�Q���U ���� ��ı�o�o�ӧP�_�i�H���@�I���� ����F�ղ{�b���o�˼g�N�n�ڦn�i�k��
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
    public void AmbulanceMoving()
    {
        theAmbulanceDirectionVector = new Vector2(myMapGraph.points[ambulanceMovingToPoint].x - myMapGraph.points[ambulanceMovingFromPoint].x, myMapGraph.points[ambulanceMovingToPoint].y- myMapGraph.points[ambulanceMovingFromPoint].y).normalized;
        float vectorX = theAmbulanceDirectionVector.x/10f;
        float vectorY = theAmbulanceDirectionVector.y/10f;
        //Debug.Log("dirX:" + vectorX + " DirY:" + vectorY);
        AmbulanceObject.transform.localPosition = (AmbulanceObject.transform.localPosition + new Vector3(vectorX * ambulanceSpeed * Time.deltaTime, vectorY * ambulanceSpeed * Time.deltaTime, 0));
        
        AmbulanceReachPointJudge();
        AmbulancePointEventJudge();
    }
    public void swapAmbulanceMoving()
    {
        AmbulanceObject.transform.localPosition = (AmbulanceObject.transform.localPosition + new Vector3(0.01f * ambulanceSpeed * Time.deltaTime,0,0));
        swapAmbulanceJudgement();
    }
    public void AmbulancePointEventJudge()
    {
        if (myMapGraph.points[ambulanceMovingFromPoint].theEventPoints.Length != 0)
        {
            Vector2 journey = new Vector2(myMapGraph.points[ambulanceMovingToPoint].x - myMapGraph.points[ambulanceMovingFromPoint].x, myMapGraph.points[ambulanceMovingToPoint].y - myMapGraph.points[ambulanceMovingFromPoint].y);
            float journeyLength = journey.magnitude; // 旅程的總長度
            Vector2 ambulanceAtPos = new Vector2(AmbulanceObject.transform.position.x, AmbulanceObject.transform.position.y);
            float distanceTravelled = (ambulanceAtPos - new Vector2(myMapGraph.points[ambulanceMovingFromPoint].x, myMapGraph.points[ambulanceMovingFromPoint].y)).magnitude; // 已移動距離
            float percent = distanceTravelled / journeyLength; // 計算進度百分比
            //float percent = Mathf.Sqrt(Mathf.Pow((journey.x - ambulanceAtPos.x),2) - Mathf.Pow((journey.y - ambulanceAtPos.y),2))

            for (int i = 0, times = myMapGraph.points[ambulanceMovingFromPoint].theEventPoints.Length; i< times; i++)
            {
                //check if reachPoint
                if (myMapGraph.points[ambulanceMovingFromPoint].theEventPoints[i].atPos < percent && myMapGraph.points[ambulanceMovingFromPoint].theEventPoints[i].triggered == false)
                {
                    myMapGraph.points[ambulanceMovingFromPoint].theEventPoints[i].triggered = true;
                    switch (myMapGraph.points[ambulanceMovingFromPoint].theEventPoints[i].myEventType)
                    {
                        case (EventPoint.eventType.enemySpawn):
                            Event_enemySpawn();
                            break;
                        case (EventPoint.eventType.roadRock):
                            Event_roadRock();
                            break;
                        default:
                            Debug.Log("未知事件");
                            break;
                    }
                }
            }
        }
    }
    public void AmbulanceReachPointJudge()
    {
        //設定方向性
        bool reachX = false;
        bool reachY = false;
        if (myMapGraph.points[ambulanceMovingFromPoint].x < myMapGraph.points[ambulanceMovingToPoint].x)
        {
            if (AmbulanceObject.transform.position.x > myMapGraph.points[ambulanceMovingToPoint].x)
            {
                reachX = true;
            }
        }
        else
        {
            if (AmbulanceObject.transform.position.x <= myMapGraph.points[ambulanceMovingToPoint].x)
            {
                reachX = true;
            }
        }

        if (myMapGraph.points[ambulanceMovingFromPoint].y < myMapGraph.points[ambulanceMovingToPoint].y)
        {
            if (AmbulanceObject.transform.position.y > myMapGraph.points[ambulanceMovingToPoint].y)
            {
                reachY = true;
            }
        }
        else
        {
            if (AmbulanceObject.transform.position.y <= myMapGraph.points[ambulanceMovingToPoint].y)
            {
                reachY = true;
            }
        }

        if (reachX == true && reachY == true)
        {
            //ReachPoint, trigger point funciotn
            switch (myMapGraph.points[ambulanceMovingToPoint].pointType)
            {
                case 0:
                    Debug.LogWarning("AK_error : You shouldn't do this, the start point can't be the reach point bro, fix your game.");
                    break;
                case (1):// the end;
                    gameEnd();
                    break;
            }
        }
        else
        {
            //Debug.Log("X reach:" +reachX +" Y reach:" + reachY);
        }
    }
    public void AmbulanceSetInitialization()
    {
        ambulanceMovingFromPoint = 0;
        ambulanceMovingToPoint = 1;
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

        //�W�[��ı�ĪG
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
        if (fail)
        {
            starNumber = 0;
            gameEndText.text = "病人半路中道崩殂";
        }
        else
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

    public void damagedFunc()
    {
        damagedTipAnimator.SetTrigger("damaged");
        Debug.Log("damaged");
    }

    public void heartbeatMiss()
    {
        //Debug.Log("from GameCore.cs, the message called");
        hp -= 15f;
        comboCount = 0;

        if (hp < 0)
        {
            fail = true;
            gameEnd();
        }
    }

    public void heartbeatHit()
    {
        comboCount++;
        if (comboCount > 5)
        {
            if (hp < 100)
            {
                hp += 5;
            }
            else
            {
                hp = 100;
            }
        }
    }

    public void InsEnemy()
    {
        Instantiate(enemy,new Vector3(Random.Range(referencePointMobMovingRangeA.transform.position.x,referencePointMobMovingRangeB.transform.position.x),Random.Range(referencePointMobMovingRangeA.transform.position.y,referencePointMobMovingRangeB.transform.position.y)),Quaternion.identity);
    }
    public void Event_enemySpawn()
    {
        int enemyNumberRandom = Random.Range(1,3);
        for (int i = 0; i < enemyNumberRandom; i++)
        {
            InsEnemy();
        }
    }
    public void Event_roadRock()
    {
        int roadRockRandom = Random.Range(3,6);
        heartbeat.pendingNote += roadRockRandom;
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
    /// 0= start point
    /// 1= end point
    /// 2= the node point
    /// </summary>
    public int pointType;
    public List<int> linkingPointSort = new List<int>();
    public float x;
    public float y;

    public EventPoint[] theEventPoints;
    public bool[] alreadyReachTheEventPoints;
    public void AutoSet()
    {
        if (theEventPoints.Length != alreadyReachTheEventPoints.Length)
        {
            bool[] theSwapBoolArray = new bool[theEventPoints.Length];
            alreadyReachTheEventPoints = theSwapBoolArray;
        }
    }
}

[System.Serializable]
public class EventPoint
{
    public enum eventType { enemySpawn, roadRock}
    public eventType myEventType;
    [Range(0,1f)]public float atPos;//0 to 1, to control the position it spawn on the line.
    public bool triggered = false;
}