using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class HeartBeat : MonoBehaviour
{
    public GameCore gameCore;

    [Header("預製體")]
    public GameObject DecisionLine;

    [Header("生成位置")]
    public Transform RightInstantiatePoint; 

    [Header("生成拍子與節奏")]
    public int BPM = 120;            
    public float duration;   
    public float DelayTime = 0f;     //進入下一個小節的延遲時間
    public float measureBeats = 4f;  //每小節的拍數
    public bool fixedGenAllow = true;

    private bool InThePoint = false;
    private bool canSpace = true;
    private bool Generating = false;
    private float InstantiateCount = 0;

    public TextMeshProUGUI bpmTextMesh;

    [HideInInspector]
    public bool safe = false;


    void Start()
    {
        BPMSet();
        fixedGenAllow = true;
    }
    void Update()
    {
        if (gameCore.gameRunning)
        {
            InputSpace();
            InstantiateLine();

            //這邊優先優化掉
            BPMsync();

            //測試用功能 按下Enter手動生成一個note
            if (Input.GetKeyDown(KeyCode.O))
            {
                testGenNote();
            }
        }
    }

    public void BPMSet()
    {
        duration = 60f/(BPM*4f);
    }

    void InputSpace()
    {
        if(Input.GetKeyDown(KeyCode.Space) && canSpace)
        {
            CheckInput();
        }
    }
    void CheckInput()
    {
        if(InThePoint)
        {
            Debug.Log("在範圍內成功檢定");
            safe = true;
            gameCore.heartbeatHit();
        }
        else
        {
            Debug.Log("不在範圍喔");
            StartCoroutine(PunishTime());
            gameCore.heartbeatMiss();
        }
    }

    IEnumerator PunishTime()
    {
        canSpace = false;
        yield return new WaitForSeconds(0.5f);
        canSpace = true;
    }

    void InstantiateLine()
    {
        /*GameObject CheckLineExist = GameObject.FindWithTag("Line");
        if(CheckLineExist == null && !Generating)
        {
            StartCoroutine(InstantitateDelay());
        }*/

        if (fixedGenAllow == true && !Generating)
        {
            StartCoroutine(InstantitateDelay());
        }

    }
    IEnumerator InstantitateDelay()
    {
        Generating = true;
        if (InstantiateCount == measureBeats)
        {
            yield return new WaitForSeconds(DelayTime);
            BPMSet();
            InstantiateCount = 0;
        }
        fixedGenAllow = false;

        float sixteenthNoteDuration = 60f / (BPM * 4);
        yield return new WaitForSeconds(sixteenthNoteDuration * 4); 

        GameObject swapLine = Instantiate(DecisionLine, RightInstantiatePoint.position, RightInstantiatePoint.rotation);
        safe = false;
        Generating = false;
        InstantiateCount++; 
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Line")
        {
            InThePoint = true;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Line")
        {
            InThePoint = false;
        }
    }

    public void BPMsync()
    {
        bpmTextMesh.text = "BPM: "+(int)BPM;
    }

    public void testGenNote()
    {
        fixedGenAllow = true;
    }
}

