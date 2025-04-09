using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameCore : MonoBehaviour
{
    [Header("遊玩模式")]
    public int GAME_MODE = 0;//0= note模式, 1= 加速模式

    [Header("地圖，修改在這裡")]
    public MapGraph myMapGraph;

    [Header("基本的GameObject Declare, 別碰")]
    public GameObject AmbulanceObject;
    public GameObject GameStartPanel;
    public GameObject GameEndPanel;

    [Header("救護車相關")]
    public float ambulanceSpeed = 7f;
    public int ambulanceMovingFromPoint;
    public int ambulanceMovingToPoint;
    public Vector2 theAmbulanceDirectionVector;

    [Header("遊戲進行相關")]
    public bool gameRunning = false;

    [Header("hp/O2系統")]
    public float maxHp = 100f;
    public float hp = 100f;//100~0 
    public int hpStatement = 4; // 0=���`�A
    public Sprite[] uiSprite = new Sprite[5];
    public SpriteRenderer heartbeatChart;
    public Image hpShowRate;

    [Header("血包相關")]
    public float bloodLooseRate = 1f;
    public float bloodMax = 40f; //���P�����
    public float bloodNow = 40f; //�{�b�Ѿl��q 
    public Image bloodPackImage;
    public float bloodLooseingCount;

    [Header("Enemy系統")]
    public GameObject enemy;
    public GameObject referencePointMobMovingRangeA;
    public GameObject referencePointMobMovingRangeB;
    public GameObject CthulhuObject;
    public GameObject Goat;
    private float[] AxPositions = { -0.1f, 0f,-0.2f };
    private float[] BxPositions = { 0f, 0.1f };
    private float[] YPositions = { 0f, 0.5f };
    private int currentInsAPos = 0;
    private int currentInsBPos = 0;
    private int currentInsYPos = 0;

    [Header("我也不知道為什麼Heartbeat會宣告在這")]
    public Heartbeat heartbeat;

    [Header("Animator們")]
    public Animator damagedTipAnimator;
    public Animator bloodPackTipAnimator;
    public Animator UIAnimator;
    public Animator gameEndPenalAnimator;
    public Animator BulletLight;
    public Animator BloodBoxLight;
    public Animator Road;

    [Header("gameEnd")]
    public Animator gameEndBGLoadinAnimator;
    public GameObject GameEndBGLoadinPanel;


    [Header("這些是Sprite和Image大家庭")]
    public Sprite eventSprite_enemySpawn;
    public Sprite eventSprite_roadRock;
    public Sprite eventSprite_cthulhu;
    public Sprite eventSprite_empty;

    public Image[] starImages = new Image[3];
    public Sprite emptyStar;
    public Sprite star;

    public Image[] endBG = new Image[3];
    

    [Header("Note相關呦")]
    public Image hintImage;
    public Sprite Perfect;
    public Sprite Great;
    public Sprite Good;
    public Sprite Miss;
    public Sprite Fail;
    public float hintImageAlpha = 1;
    public float scaleFactor = 10000f; // 放大的比例
    public float scaleDuration = 0.002f; // 放大的時間（秒）

    private Vector3 originalScale;
    

    [Header("莫名其妙的參數 我也忘記幹嘛用了")]
    public int gameRate = 0; // -1~2, -1�N�����d����, 0~2�N��1~3�P
    public int starNumber = 0;

    public bool fail = false;

    [Header("Combo計數器 超過5會開始回生命值")]
    public int comboCount = 0;

    // 1�P-�q�L���d 2�P-50%�H�W�ɶ��f�H�wí���A 3�P-85%�H�W�ɶ��f�H�wí���A
    //�wí���A = �f�H�ͩR��H���A
    [Header("和關卡星數有關")]
    public float[] gameStatementRate = new float[5];
    public float finalGameRateResult = 0;

    [Header("TextMesh的東西")]
    public TextMeshProUGUI O2TextMesh;
    public TextMeshProUGUI gameEndText;

    [Header("BPM, 對")]
    public int bpm = 120;
    public float playSpeed =1f;
    public Animator heartPumpingAnimator;

    [Header("shake 系統")]
    public GameObject allObjectFather;
    public float shakeStrength = 5f;
    public float shakeInterval = 0.1f;
    Coroutine theShakeCoroutine;

    [Header("Hocus system")]
    public int maxHocusCount = 3;
    public int nowHocusCount = 3;
    public Animator HocusAnimator;

    public float hocusSlowRate = 0.4f;
    public float hocusTime = 4f;

    [Header("who'd fuck are you?")]
    public int theBPM
    {
        get { return bpm; }
        set { bpm = value; heartbeat.BPMChnage(bpm); }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        allObjectFather = GameObject.Find("AllCarObject");

        RanderPoint();
        resetAmbulancePosition();
        ambulanceMovingFromPoint = 0;// set the start point

        driverText2.text = "";

        //Swap
        ambulanceMovingToPoint = 1;
        originalScale = hintImage.transform.localScale;

        if (GAME_MODE == 0)
        {
            
        }
        else if (GAME_MODE == 1)
        {
            //
        }

        Invoke(nameof(gameStart), 1f);
        //gameStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            //driver_speak("h20純水真的是非常非常非常非常非常非常的好吃");
        }
        HintImageAutoFade(); 
        if (gameRunning)
        {
            AmbulanceMoving();
            //swapAmbulanceMoving();
            HpStatementSync();
            BloodLoose();

            hintTextAutoFade();
            

            //�o���u���u�Ʊ�
            O2Sync();

            /*
            int randomShit = Random.Range(1,11451);
            if(randomShit == 50)
            {
                InsEnemy();
            }*/

            if (Input.GetKeyDown(KeyCode.T))
            {
                carShake(3f, 0.15f, 0.15f,false);
            }

            timeControl(playSpeed);

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

                lineRenderer.endColor = new Color(0, 0, 0, 0);
                lineRenderer.startColor = new Color(0, 0, 0, 0);

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

                //Debug.Log(myMapGraph.points[ambulanceMovingFromPoint].theEventPoints[j].myEventType + "載入");
                switch (myMapGraph.points[ambulanceMovingFromPoint].theEventPoints[j].myEventType)
                {
                    case (EventPoint.eventType.enemySpawn):
                        sr.sprite = eventSprite_enemySpawn;
                        break;
                    case (EventPoint.eventType.roadRock):
                        sr.sprite = eventSprite_roadRock;
                        break;
                    case (EventPoint.eventType.enemySpawnHint):
                        sr.sprite = eventSprite_empty;
                        break;
                    case (EventPoint.eventType.roadRockHint):
                        sr.sprite = eventSprite_empty;
                        break;
                    case (EventPoint.eventType.cthulhu):
                        sr.sprite = eventSprite_empty;
                        break;
                    case (EventPoint.eventType.goatScream):
                        sr.sprite = eventSprite_empty;
                        break;

                    default:
                        //Debug.Log("未知事件");
                        break;
                }
                sr.color = new Color(0, 0, 0, 0);
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
            BloodBoxLight.SetBool("BloodBoxLightUp",true);
            PlaySoundEffect11Once();
        }
        else
        {
            bloodPackTipAnimator.SetBool("Warning",false);
            BloodBoxLight.SetBool("BloodBoxLightUp",false);
        }
    }
    private bool isSound11Playing = false;

    public void PlaySoundEffect11Once()
    {
        if (!isSound11Playing)
        {
            isSound11Playing = true;

            AudioClip clip = SoundEffects[11];
            PlaySoundEffect(clip);
            StartCoroutine(WaitUntilSound11Finish(clip.length));
        }
    }

    private IEnumerator WaitUntilSound11Finish(float duration)
    {
        yield return new WaitForSeconds(duration);
        isSound11Playing = false;
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
        swapMusicPlayer();
    }
    public void gameEnd()
    {
        gameRunning = false;
        GameEndPanel.SetActive(true);
        gameEndPenalAnimator = GameObject.Find("GameEndPanelAnimation").GetComponent<Animator>();
        gameEndPenalAnimator.SetBool("active",true);
        

        finalGameRateResultCal();
        finalGameExecute();
        stopSwapMusicPLayer();
    }
    public void gameEndPanel()
    {
        
        gameEndBackgroundWall = GameObject.Find("geBG_Wall");
        gameEndBackgroundPatient = GameObject.Find("geBG_Patient");
        gameEndBackgroundAmbulance = GameObject.Find("geBG_Ambulance");

        gameEndBackgroundWall.GetComponent<Image>().enabled = true;
        gameEndBackgroundPatient.GetComponent<Image>().enabled = true;
        gameEndBackgroundAmbulance.GetComponent<Image>().enabled = true;

        gameEndBGLoadinAnimator.SetBool("Loadin",true);
    }

    public void LoadNextGame()
    {
        if (SceneManager.sceneCount < SceneManager.GetActiveScene().buildIndex +1)
        {
            StartCoroutine(LoadNextGameAnimation());
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
        StartCoroutine(LoadAnimation(0));
    }
    public IEnumerator LoadNextGameAnimation()
    {
        UIAnimator.SetTrigger("LoadOut");
        PlaySoundEffect(SoundEffects[5]);
        yield return new WaitForSeconds(1.2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
    }
    public IEnumerator LoadAnimation(int LoadScene)
    {
        UIAnimator.SetTrigger("LoadOut");
        PlaySoundEffect(SoundEffects[5]);
        yield return new WaitForSeconds(1.2f);
        SceneManager.LoadScene(LoadScene);
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
            //GameObject.Find("process_dur").GetComponent<Text>().text = "process: "+percent;
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
                            Debug.Log("The Call is from enemy spawn event: " + myMapGraph.points[ambulanceMovingFromPoint].theEventPoints[i].myEventType);
                            Event_enemySpawn();
                            break;
                        case (EventPoint.eventType.roadRock):
                            Event_roadRock();
                            break;
                        case (EventPoint.eventType.cthulhu):
                            Event_cthulhu();
                            break;
                        case (EventPoint.eventType.goatScream):
                            Event_goatScream();
                            break;
                        case (EventPoint.eventType.speedUp):
                            Debug.Log("The Call is from speed up event: " + myMapGraph.points[ambulanceMovingFromPoint].theEventPoints[i].myEventType);
                            Debug.Log("The Call is from object: " + gameObject);
                            Event_speedUp();
                            break;
                        case (EventPoint.eventType.driverSpeak):
                            Event_driverSpeak(myMapGraph.points[ambulanceMovingFromPoint].theEventPoints[i].argument);
                            break;
                        case (EventPoint.eventType.driverSpeakSystem):
                            Event_driverSpeakSystem(myMapGraph.points[ambulanceMovingFromPoint].theEventPoints[i].argument);
                            break;

                        case (EventPoint.eventType.spec_allowUseSryinge):
                            SpecEvent_allowUseSryinge();
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
        hpShowRate.fillAmount = hpRate;
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
    [Header("你知道哈密瓜很好吃嗎")]
    public GameObject gameEndBackgroundWall;
    public GameObject gameEndBackgroundAmbulance;
    public GameObject gameEndBackgroundPatient;
    public Sprite[] gameEndBackgroundSprite = new Sprite[3];
    public GameObject GameEndButtonLoadNextLevel;

    public void finalGameExecute()
    {
        if (fail)
        {
            GameEndButtonLoadNextLevel = GameObject.Find("GameEndButtonLoadNextLevel");
            GameEndButtonLoadNextLevel.GetComponent<Button>().interactable = false; 

            gameEndBackgroundWall = GameObject.Find("geBG_Wall");
            gameEndBackgroundPatient = GameObject.Find("geBG_Patient");
            gameEndBackgroundAmbulance = GameObject.Find("geBG_Ambulance");

            gameEndBackgroundWall.GetComponent<Image>().sprite = gameEndBackgroundSprite[0];
            gameEndBackgroundPatient.GetComponent<Image>().sprite = gameEndBackgroundSprite[1];
            gameEndBackgroundAmbulance.GetComponent<Image>().sprite = gameEndBackgroundSprite[2];


            starNumber = 0;
            gameEndText.text = "病人半路中道崩殂\n在大廳試試簡單模式?";
            //跳到這裡
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


            //Judge是否開關卡
            //取得關卡序列 並比對關卡是否通過
            int myGameSort = (SceneManager.GetActiveScene().buildIndex) -1;
            if (Lobby_Core.s_GSF.gamePassed[myGameSort] == false)
            {
                Lobby_Core.s_GSF.gamePassed[myGameSort] = true;
                Lobby_Core.s_GSF.mexUnlockGame += 1;
                Lobby_Core.SaveGameFile();
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

    public void perfectHintFunc()
    {
        //damagedTipAnimator.gameObject.GetComponent<Image>().color = Color.green;
        //damagedTipAnimator.SetTrigger("damaged");
        GameObject textGameObect = new GameObject("niceHint");
        
    }
    public void damagedHintFunc()
    {
        damagedTipAnimator.gameObject.GetComponent<Image>().color = Color.red;
        damagedTipAnimator.SetTrigger("damaged");
        Debug.Log("damaged");
    }

    public void heartbeatMiss()
    {
        //Debug.Log("from GameCore.cs, the message called");
        if(gameRunning)
        {
            hp -= 5f;
            comboCount = 0;
            showHintText(3);
            ShowHintImage(3);

            if (hp < 0)
            {
                fail = true;
                gameEnd();
            }
        }
    }

    public void BanditAttack(float damage)
    {
        //Debug.Log("from GameCore.cs, the message called");

        //carShake(4, 1.4f, 0.15f, false);
        carShake(3f, 0.13f, 0.15f, false);
        hp -= damage;
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
        heartbeat.hitJudgeReaction(1);
    }

    public void InsEnemy()
    {
        Vector2 spawnPosition;
        float selectedX;
        float selectedY;

        if (Random.Range(0, 2) == 0)
        {
            spawnPosition = referencePointMobMovingRangeA.transform.position;
            selectedX = AxPositions[currentInsAPos];
            currentInsAPos = (currentInsAPos + 1) % AxPositions.Length;
        }
        else
        {
            spawnPosition = referencePointMobMovingRangeB.transform.position;
            selectedX = BxPositions[currentInsBPos];
            currentInsBPos = (currentInsBPos + 1) % BxPositions.Length;
        }
        selectedY = YPositions[currentInsYPos];
        currentInsYPos = (currentInsYPos + 1) % YPositions.Length;
        Vector2 randomOffset = new Vector2(selectedX, selectedY);
        
        Instantiate(enemy, spawnPosition + randomOffset, Quaternion.identity);
        
    }
        public void Event_enemySpawn()
        {
        /*
        int enemyNumberRandom = Random.Range(1,3);
        for (int i = 0; i < enemyNumberRandom; i++)
        {
            InsEnemy();
        }*/
        InsEnemy();
        
        
        
        //暴力解Bug 之後有問題修掉
        //playSpeed -= 0.1f;



        //BulletLight.SetTrigger("BulletLightUp"); //子彈變亮提示
        Debug.Log("Enemy spawn from the event.");
    }
    public void Event_roadRock()
    {       
        heartbeat.pendingNote += 3;
        PlaySoundEffect(SoundEffects[0]);
        //carShake(4, 1.4f, 0.15f, false);
        carShake(3f, 0.3f, 0.15f, false);
    }
    public void InsCthulhu()
    {
        float RanX = Random.Range(-6.66f, 7.06f);
        float RanY = Random.Range(-3.68f, 3.36f);
        Instantiate(CthulhuObject, new Vector3(RanX, RanY), Quaternion.identity);

    }
    public void Event_cthulhu()
    {
        //BulletLight.SetTrigger("BulletLightUp");  //子彈變亮提示
        InsCthulhu();
    }
    public void Event_goatScream()
    {
        goatScream();
    }
    public void goatScream()
    {
        Instantiate(Goat);
    }

    [Header("BGM and SoundEffects")]
    public AudioClip bgmClip;
    GameObject theBgmPlayer;
    public AudioClip[] SoundEffects;
    public void swapMusicPlayer()
    {
        theBgmPlayer = new GameObject("BgmPlayer");
        AudioSource AS =  theBgmPlayer.AddComponent<AudioSource>();
        AS.volume = 0.2f;
        AS.clip = bgmClip;
        AS.loop = true;
        AS.Play();
    }
    public void stopSwapMusicPLayer()
    {
        Destroy(theBgmPlayer);
    }

    public void PlaySoundEffect(AudioClip AC)
    {
        GameObject newSoundEffect = new GameObject("soundEffect");
        AK_SoundObject AKS =  newSoundEffect.AddComponent<AK_SoundObject>();
        AudioSource AS = newSoundEffect.AddComponent<AudioSource>();
        AS.clip = AC;
        AS.Play();
        Destroy(newSoundEffect, AKS.playTime);
    }

    [Header("輔助顯示字體")]
    public TextMeshProUGUI theHintTxt;
    public float hintTextAlpha = 1;
    public float countingRemain = 0f;
    public void showHintText(int res)
    {
        countingRemain = 1;
        hintTextAlpha = 1;
        if (res == 1)
        {
            //means plater hit perfect
            theHintTxt.text = "Perfect";
            theHintTxt.color = Color.green;
        }
        else if (res == 2)
        {
            theHintTxt.text = "Good";
            theHintTxt.color = new Color(0.3f, 0.75f, 0, hintTextAlpha);
        }
        else if (res == 3)
        {
            theHintTxt.text = "Miss";
            theHintTxt.color = new Color(0.2f, 0.2f, 0.2f, hintTextAlpha);
        }
        else if (res == 4)
        {
            theHintTxt.text = "Great";
            theHintTxt.color = new Color(0.3f, 0.75f, 0, hintTextAlpha);
        }
        else
        {
            theHintTxt.text = "Wrong";
            theHintTxt.color = Color.red;
        }
    }
    public void hintTextAutoFade()
    {
        countingRemain += Time.deltaTime;
        if (countingRemain > 0.2)
        {
            hintTextAlpha = hintTextAlpha - Time.deltaTime*1.2f;
            theHintTxt.color = new Color(theHintTxt.color.r, theHintTxt.color.g, theHintTxt.color.b,hintTextAlpha);
        }
    }
    public void ShowHintImage(int res)
    {
        countingRemain = 1;
        hintImageAlpha = 1;

        if (res == 1)
        {
            hintImage.sprite = Perfect;
        }
        else if (res == 2)
        {
            hintImage.sprite = Good;
        }
        else if (res == 3)
        {
            hintImage.sprite = Miss;
        }
        else if (res == 4)
        {
            hintImage.sprite = Great;
        }
        else
        {
            hintImage.sprite = Fail;
        }
        hintImage.color = new Color(hintImage.color.r, hintImage.color.g, hintImage.color.b, hintImageAlpha);

        StartCoroutine(ScaleImage());
    }


    public void HintImageAutoFade()
    {
        countingRemain += Time.deltaTime;
        if (countingRemain > 0.2f)
        {
            hintImageAlpha = hintImageAlpha - Time.unscaledDeltaTime * 1.2f;
            hintImage.color = new Color(hintImage.color.r, hintImage.color.g, hintImage.color.b, hintImageAlpha);
        }
        
    }
    private IEnumerator ScaleImage()
    {
        // 放大動畫
        Vector3 targetScale = originalScale * scaleFactor;
        float elapsedTime = 0f;

        while (elapsedTime < scaleDuration)
        {
            hintImage.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        hintImage.transform.localScale = targetScale;

        // 恢復原來尺寸
        elapsedTime = 0f;
        while (elapsedTime < scaleDuration)
        {
            hintImage.transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        hintImage.transform.localScale = originalScale; // 確保最後的尺寸是原來的尺寸
    }

    public void carShake()
    {
        if (theShakeCoroutine != null)
        {
            StopCoroutine(theShakeCoroutine);
        }
        theShakeCoroutine = StartCoroutine(shakeCoroutine());
    }

    IEnumerator shakeCoroutine()
    {
        float swapX = 0;
        float swapY = 0;

        //float defultVelocity = 1f;
        float defultTime = 1f;

        float timePast =0;

        WaitForSeconds sec = new WaitForSeconds(shakeInterval);

        while (defultTime >0)
        {
            timePast += Time.deltaTime;

            swapX = Mathf.Sin(timePast) * Random.Range(-1f, 1f) * shakeStrength;
            swapY = Mathf.Cos(timePast) * Random.Range(-1f, 1f) * shakeStrength;

            allObjectFather.transform.position = new Vector3(swapX, swapY, 0);

            defultTime -= Time.deltaTime;
            yield return sec;
        }

        yield return null;
        allObjectFather.transform.position = new Vector3(0,0,0);
    }


    public void carShake(float dur, float str, float interval, bool isFadeOut)
    {
        if (theShakeCoroutine != null)
        {
            StopCoroutine(theShakeCoroutine);
        }
        theShakeCoroutine = StartCoroutine(shakeCoroutine(dur, str, interval, isFadeOut));
    }

    IEnumerator shakeCoroutine(float dur, float str, float interval, bool isFadeOut)
    {
        float swapX = 0;
        float swapY = 0;

        float oriStr = str;

        //float defultVelocity = 1f;
        float defultTime = dur;

        float timePast = 0;
        int count = 0;
        int maxCount = (int)(dur / interval);

        WaitForSeconds sec = new WaitForSeconds(interval);

        while (defultTime > 0)
        {
            timePast += Time.deltaTime;

            swapX = Mathf.Sin(timePast) * Random.Range(-1f, 1f) * str;
            swapY = Mathf.Cos(timePast) * Random.Range(-1f, 1f) * str;

            allObjectFather.transform.position = new Vector3(swapX, swapY, 0);

            if (isFadeOut)
            {
                str = oriStr * ((maxCount - count) / maxCount);
            }

            defultTime -= interval;
            count++;
            yield return interval;
        }

        yield return null;
        allObjectFather.transform.position = new Vector3(0, 0, 0);
    }

    public void timeControl(float theFuq)
    {
        AudioSource AS = theBgmPlayer.GetComponent<AudioSource>();
        AS.pitch = theFuq;
        heartbeat.BPMChange(theFuq);
    }

    public void Event_speedUp()
    {
        playSpeed += 0.1f;
        Road.speed += 0.1f;

        Debug.Log("Speed up from script event, now speed: " + playSpeed);
    }

    public void Event_driverSpeak(string[] args)
    {
        driver_speak(args);
    }
    public void Event_driverSpeakSystem(string[] args)
    {
        driver_speak_system(args);
    }
    [Header("特別事件變數")]
    public GameObject specEvent_objA;
    public GameObject specEvent_objB;
    public GameObject specEvent_objC;
    public void SpecEvent_allowUseSryinge()
    {
        specEvent_objA.SetActive(true);
        specEvent_objB.SetActive(true);
        specEvent_objC.SetActive(false);
    }
    public void Hocus()
    {
        StartCoroutine(hocus(hocusTime));
    }
    public void Hocus(float sec)
    {
        StartCoroutine(hocus(sec));
    }
    IEnumerator hocus(float sec)
    {
        Time.timeScale = hocusSlowRate;
        HocusAnimator.SetBool("hocusing",true);
        yield return new WaitForSecondsRealtime(sec);
        Time.timeScale = 1f;
        HocusAnimator.SetBool("hocusing", false);
    }

    public void con_HeartPumping(float speedRate)
    {
        heartPumpingAnimator.speed = speedRate;
    }

    [Header("Driver System")]
    public SpriteRenderer DSSR;
    public Coroutine driverCoroutine;
    public TextMeshProUGUI driverText;
    public Text driverText2;
    public GameObject dialogTextbox;
    public Animator textAnimator;
    public Animator driverAnimator;
    public bool isReadReady = false;
    public GameObject CastButton;
    public bool onSpeaking = false;
    public bool breakTrigger = false;

    public GameObject driver_mouth;
    public GameObject driver_mouthPosA;
    public GameObject driver_mouthPosB;
    public void CastButtonEvent()
    {
       if (onSpeaking)
        {
            Debug.Log("break the trigger call");
            breakTrigger = true;
        }
       else
        {
            Debug.Log("Finish reading and skip to the next line of text, call");
            isReadReady = true;
        }
        //Debug.Log("time stop released");
    }
    public void driver_system(string str)
    {
    }
    public void driver_speak(string[] strs)
    {
        if (strs != null)
        {
            if (driverCoroutine != null)
            {
                StopCoroutine(driverCoroutine);
            }
            driverCoroutine =StartCoroutine(driver_SpeakCoroutine(strs[0]));
        }
        else
        {
            Debug.Log("AkBug: event is empty");
        }
        
    }
    IEnumerator driver_SpeakCoroutine(string str)
    {
        string swapStr = "";
        //driverAnimator.SetBool("onSpeaking",true);
        //textAnimator.SetBool("onSpeaking", true);
        dialogTextbox.SetActive(true);
        //yield return new WaitForSecondsRealtime(0.7f);
        for (int i = 0; i < str.Length; i++)
        {
            swapStr += str[i];
            driverText2.text = swapStr;
            yield return new WaitForSecondsRealtime(0.08f);
        }
        yield return new WaitForSecondsRealtime(1.2f + (str.Length/12));
        driverText2.text = "";
        //driverAnimator.SetBool("onSpeaking", false);
        //textAnimator.SetBool("onSpeaking", false);
        dialogTextbox.SetActive(false);
    }
    public void driver_speak_system(string[] strs)
    {
        if (driverCoroutine != null)
        {
            StopCoroutine(driverCoroutine);
        }
        driverCoroutine = StartCoroutine(driver_SpeakCoroutine_system(strs));
    }
    IEnumerator driver_SpeakCoroutine_system(string[] strs)
    {
        Debug.Log("Driver Speak System Triggered");
        string swapStr = "";
        //driverAnimator.SetBool("onSpeaking", true);
        //textAnimator.SetBool("onSpeaking", true);

        //yield return new WaitForSecondsRealtime(0.7f);

        for (int j = 0; j < strs.Length; j++)
        {
            dialogTextbox.SetActive(true);
            Time.timeScale = 0f;
            isReadReady = false;
            yield return new WaitForSecondsRealtime(0.01f);
            CastButton.SetActive(true);
            swapStr = "";



            if (strs[j] == "SpecEvent_allowUseSryinge")
            {
                SpecEvent_allowUseSryinge();
                onSpeaking = false;
                breakTrigger = false;
                Debug.Log("SpecEvent_allowUseSryinge triggered by dialog system");
            }
            else if (strs[j] == "Spec_gameEnd")
            {
                //Judge是否開關卡
                //取得關卡序列 並比對關卡是否通過
                int myGameSort = (SceneManager.GetActiveScene().buildIndex) - 1;
                if (Lobby_Core.s_GSF.gamePassed[myGameSort] == false)
                {
                    Lobby_Core.s_GSF.gamePassed[myGameSort] = true;
                    Lobby_Core.s_GSF.mexUnlockGame += 1;
                    Lobby_Core.SaveGameFile();
                }

                SceneManager.LoadScene(8);

            }
            else
            {
                for (int i = 0; i < strs[j].Length; i++)
                {

                    onSpeaking = true;

                    //break condition
                    if (breakTrigger)
                    {
                        //break the for loop
                        driverText2.text = strs[j];
                        onSpeaking = false;
                        breakTrigger = false;
                        //Debug.Log("break trigger skip out");
                        break;
                    }

                    swapStr += strs[j][i];
                    driverText2.text = swapStr;
                    //Debug.Log("Driver Count" + i);

                    int ran = Random.Range(0, 2);
                    if (ran == 0)
                    {
                        driver_mouth.transform.position = driver_mouthPosA.transform.position;
                    }
                    else
                    {
                        driver_mouth.transform.position = driver_mouthPosB.transform.position;
                    }
                    yield return new WaitForSecondsRealtime(0.08f);
                }
                onSpeaking = false;
                // Debug.Log("starting wait the reading cast");
                yield return new WaitUntil(() => isReadReady);

                Time.timeScale = 1f;

                driverText2.text = "";
            }
        }
        //driverAnimator.SetBool("onSpeaking", false);
        //textAnimator.SetBool("onSpeaking", false);
        dialogTextbox.SetActive(false);
        CastButton.SetActive(false);
        driver_mouth.transform.position = driver_mouthPosA.transform.position;
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
    public enum eventType { enemySpawn, roadRock, enemySpawnHint, roadRockHint, cthulhu , goatScream, speedUp, driverSpeak, driverSpeakSystem, spec_allowUseSryinge}
    public eventType myEventType;
    public string[] argument = new string[0];
    [Range(0,1f)]public float atPos;//0 to 1, to control the position it spawn on the line.
    public bool triggered = false;
}