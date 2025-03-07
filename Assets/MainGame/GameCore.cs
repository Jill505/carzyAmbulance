using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Threading;
using TMPro;
using UnityEngine;
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

    [Header("我也不知道為什麼Heartbeat會宣告在這")]
    public Heartbeat heartbeat;

    [Header("Animator們")]
    public Animator damagedTipAnimator;
    public Animator bloodPackTipAnimator;
    public Animator UIAnimator;
    public Animator gameEndPenalAnimator;


    [Header("這些是Sprite和Image大家庭")]
    public Sprite eventSprite_enemySpawn;
    public Sprite eventSprite_roadRock;
    public Sprite eventSprite_cthulhu;
    public Sprite eventSprite_empty;

    public Image[] starImages = new Image[3];
    public Sprite emptyStar;
    public Sprite star;

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

    [Header("shake 系統")]
    public GameObject allObjectFather;
    public float shakeStrength = 5f;
    public float shakeInterval = 0.1f;
    Coroutine theShakeCoroutine;

    [Header("Hocus system")]
    public int maxHocusCount = 3;
    public int nowHocusCount = 3;

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

        //Swap
        ambulanceMovingToPoint = 1;

        if (GAME_MODE == 0)
        {
            
        }
        else if (GAME_MODE == 1)
        {
            //
        }
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

                Debug.Log(myMapGraph.points[ambulanceMovingFromPoint].theEventPoints[j].myEventType + "載入");
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
                        Debug.Log("未知事件");
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
        swapMusicPlayer();
    }
    public void gameEnd()
    {
        gameRunning = false;
        GameEndPanel.SetActive(true);

        gameEndPenalAnimator.SetBool("active",true);

        finalGameRateResultCal();
        finalGameExecute();
        stopSwapMusicPLayer();
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
        StartCoroutine(LoadAnimation(0));
    }
    public IEnumerator LoadAnimation(int LoadScene)
    {
        UIAnimator.SetTrigger("LoadOut");
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
                        case (EventPoint.eventType.cthulhu):
                            Event_cthulhu();
                            break;
                        case (EventPoint.eventType.goatScream):
                            Event_goatScream();
                            break;
                        case (EventPoint.eventType.speedUp):
                            Event_speedUp();
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
        hp -= 5f;
        comboCount = 0;
        showHintText(3);

        if (hp < 0)
        {
            fail = true;
            gameEnd();
        }
    }

    public void BanditAttack()
    {
        //Debug.Log("from GameCore.cs, the message called");

        carShake(4, 1.4f, 0.15f, false);
        hp -= 5f;
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

        if (Random.Range(0, 2) == 0)
        {
            spawnPosition = referencePointMobMovingRangeA.transform.position;
        }
        else
        {
            spawnPosition = referencePointMobMovingRangeB.transform.position;
        }
        Instantiate(enemy, spawnPosition, Quaternion.identity);
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
        Debug.Log("Enemy spawn from the event.");
    }
    public void Event_roadRock()
    {       
        heartbeat.pendingNote ++;
        PlaySoundEffect(SoundEffects[0]);
        carShake(4, 1.4f, 0.15f, false);
    }
    public void InsCthulhu()
    {
        float RanX = Random.Range(-6.66f, 7.06f);
        float RanY = Random.Range(-3.68f, 3.36f);
        Instantiate(CthulhuObject, new Vector3(RanX, RanY), Quaternion.identity);

    }
    public void Event_cthulhu()
    {
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
        yield return new WaitForSecondsRealtime(sec);
        Time.timeScale = 1f;
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
    public enum eventType { enemySpawn, roadRock, enemySpawnHint, roadRockHint, cthulhu , goatScream, speedUp}
    public eventType myEventType;
    [Range(0,1f)]public float atPos;//0 to 1, to control the position it spawn on the line.
    public bool triggered = false;
}