using System.Collections;
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
    public int hpStatement = 4; // 0=���`�A
    public Sprite[] uiSprite = new Sprite[5];
    public SpriteRenderer heartbeatChart;

    public float bloodLooseRate = 1f;
    public float bloodMax = 40f; //���P�����
    public float bloodNow = 40f; //�{�b�Ѿl��q
    public Image bloodPackImage;

    public Animator damagedTipAnimator;

    public Image[] starImages = new Image[3];
    public Sprite emptyStar;
    public Sprite star;
    public int gameRate = 0; // -1~2, -1�N�����d����, 0~2�N��1~3�P
    public int starNumber = 0;
    // 1�P-�q�L���d 2�P-50%�H�W�ɶ��f�H�wí���A 3�P-85%�H�W�ɶ��f�H�wí���A
    //�wí���A = �f�H�ͩR��H���A
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

            //�o���u���u�Ʊ�
            O2Sync();


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
        }

    }

    
    public void BloodLoose()
    {
        
        bloodNow -= Time.deltaTime * bloodLooseRate;
        bloodPackImage.fillAmount = bloodNow / bloodMax;
    }

    public void BloodLooseJudgement()
    {
        //�P�w����P�W�[note
    }

    public void ChangeBloodPack()
    {
        bloodNow = bloodMax; //�������@�w�ɶ��A���n���W�N�}�l�y��A��O�@�I�p�L�ĮĪG
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

    public void damagedFunc()
    {
        damagedTipAnimator.SetTrigger("damaged");
        Debug.Log("damaged");
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
    /// 0=�_�I
    /// 1=���I
    /// 2=�`�I
    /// </summary>
    public int pointType;
    public List<int> linkingPointSort = new List<int>();
    public float x;
    public float y;
}