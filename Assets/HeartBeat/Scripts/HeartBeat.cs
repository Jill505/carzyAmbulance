using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class HeartBeat : MonoBehaviour
{
    [Header("預製體")]
    public GameObject DecisionLine;

    [Header("生成位置")]
    public Transform RightInstantiatePoint; 
    public Transform LeftInstantiatePoint; 

    [Header("數秒數")]
    public Text count;
    private float Count;


    private bool InThePoint = false;
    private bool canSpace = true;

    [HideInInspector]
    public bool safe = false;


    void Update()
    {
        InputSpace();
        InstantiateLine();
        ForCount();
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
            Debug.Log("你過關");
            safe = true;
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
        GameObject CheckLineExist = GameObject.FindWithTag("Line");
        if(CheckLineExist == null)
        {
            Instantiate(DecisionLine,RightInstantiatePoint.position,RightInstantiatePoint.rotation);
            Instantiate(DecisionLine,LeftInstantiatePoint.position,LeftInstantiatePoint.rotation);
            safe = false;
        }
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

    

    
}