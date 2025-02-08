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
    public Transform LeftInstantiatePoint; 

    [Header("數秒數")]
    public Text count;
    private float Count;

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
            ForCount();

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
        duration = 60f/BPM;
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
            ChangeColor("#56FF00");
        }
        else
        {
            Debug.Log("不在範圍喔");
            StartCoroutine(PunishTime());
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
        if(InstantiateCount == measureBeats)
        {
            yield return new WaitForSeconds(DelayTime);
            BPMSet();
            InstantiateCount = 0;
        }
        ChangeColor("#FF0000");
        fixedGenAllow = false;
        GameObject swapLine = Instantiate(DecisionLine,RightInstantiatePoint.position,RightInstantiatePoint.rotation);
        //Instantiate(DecisionLine,LeftInstantiatePoint.position,LeftInstantiatePoint.rotation);
        //swapLine.GetComponent<Line>().speed = 
        safe = false;
        Generating = false;
        InstantiateCount++; 
    }

    void ForCount()
    {
        Count += Time.deltaTime;
        string countText = Count.ToString("F2"); // 保留兩位小數
        count.text =  countText;
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

    void ChangeColor(string hexColor)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(hexColor, out color))
        {
            this.GetComponent<Renderer>().material.color = color;
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