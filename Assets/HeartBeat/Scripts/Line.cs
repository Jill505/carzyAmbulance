using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Line : MonoBehaviour
{
    private Heartbeat Heartbeat;
    private GameCore gameCore;
    public float speed = 5f; 
    public float punishTime = 0.2f;
    public bool punish = false;
    private bool getPoint = false;
    private bool hasExecuted = false;
    public bool InThePerfectPoint = false;
    public bool InTheGreatPoint = false;
    public bool InTheGoodPoint = false;
    public bool InThePunishPoint = false;

    public SpriteRenderer childSpriteRenderer;
    public Animator myAnimator;
    private int thisLineNumber;
    private bool isChecked = false;
    


    void Start()
    {
        Heartbeat = GameObject.Find("Chart").GetComponent<Heartbeat>();
        gameCore = GameObject.Find("GameCore").GetComponent<GameCore>();

        myAnimator = gameObject.GetComponent<Animator>();

        childSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        thisLineNumber = Heartbeat.GetNextLineNumber();
        Heartbeat.RegisterLine(this, thisLineNumber);
    }

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (transform.position.x < Heartbeat.checkIfShouldChangeColor.transform.position.x)
        {
            if (getPoint != true && hasExecuted == false)
            {
                //Debug.Log("a hint from Line.cs, the function triggered currecrt");
                gameCore.damagedHintFunc();
                gameCore.heartbeatMiss();
                Heartbeat.nextLineToCheck++;
                Heartbeat.RemoveLine(thisLineNumber); 
                if (childSpriteRenderer != null)
                {
                    childSpriteRenderer.color = Color.red;
                }
                else
                {
                    childSpriteRenderer.color = new Color(0,1,0,1);
                    //Play Animation
                    myAnimator.SetTrigger("makeItClear");
                }
                hasExecuted = true;
                isChecked = true; 
            }
        }

        if (transform.position.x < Heartbeat.border.transform.position.x)
        {
            Destroy(gameObject);
        }

    }        


    //bool ckptClog = false;
    public void CheckInput()
    {
        if (isChecked) return;
        if(InThePunishPoint == true)
        {
            Debug.Log("懲罰");
            gameCore.showHintText(5);
            gameCore.ShowHintImage(5);
            StartCoroutine(PunishTime());
        }
        if(InThePerfectPoint)
        {
            Debug.Log("完美");
            getPoint = true;
            gameCore.heartbeatHit();
            gameCore.perfectHintFunc();
            gameCore.showHintText(1);
            gameCore.ShowHintImage(1);


            childSpriteRenderer.color = new Color(0, 1, 0, 1);
            //Play Animation
            myAnimator.SetTrigger("makeItClear");
            Debug.Log("CClear");
            isChecked = true;
            Heartbeat.RemoveLine(thisLineNumber); // 檢查後移除該Line

        }
        else if(InTheGreatPoint)
        {
            Debug.Log("很棒");
            getPoint = true;
            gameCore.heartbeatHit();
            gameCore.perfectHintFunc();
            gameCore.showHintText(4);
            gameCore.ShowHintImage(4);


            childSpriteRenderer.color = new Color(0, 1, 0, 1);
            //Play Animation
                
            myAnimator.SetTrigger("makeItClear");
            Debug.Log("CClear");
            isChecked = true;
            Heartbeat.RemoveLine(thisLineNumber); // 檢查後移除該Line
        }
        else if(InTheGoodPoint)
        {
            Debug.Log("很好");
            getPoint = true;
            gameCore.heartbeatHit();
            gameCore.perfectHintFunc();
            gameCore.showHintText(2);
            gameCore.ShowHintImage(2);


            childSpriteRenderer.color = new Color(0, 1, 0, 1);
            //Play Animation
                
            myAnimator.SetTrigger("makeItClear");
            Debug.Log("CClear");
            isChecked = true;
            Heartbeat.RemoveLine(thisLineNumber); // 檢查後移除該Line
        }
        
            
    }
    public bool IsChecked()
    {
        return isChecked;
    }
        
        
    
    void OnTriggerEnter2D(Collider2D collision)
    {    
        
        if (collision.gameObject.tag == "Good")
        {
            InTheGoodPoint = true;
            childSpriteRenderer.color = new Color(1, 1, 1, 0.7f);
        }
        if (collision.gameObject.tag == "Great")
        {
            InTheGreatPoint = true;
            childSpriteRenderer.color = new Color(1, 1, 1, 0.8f);
        }
        if (collision.gameObject.tag == "Perfect")
        {
            InThePerfectPoint = true;
            childSpriteRenderer.color = new Color(1, 1, 1, 0.9f);
            //gameCore.perfectHintFunc();
        }
        if (collision.gameObject.tag == "PunishPoint")
        {
            InThePunishPoint = true;
        }

        
    }
    void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Good")
        {
            InTheGoodPoint = false;
        }
        if (collision.gameObject.tag == "Great")
        {
            InTheGreatPoint = false;
        }
        if (collision.gameObject.tag == "Perfect")
        {
            InThePerfectPoint = false;
        }
        if (collision.gameObject.tag == "PunishPoint")
        {
            InThePunishPoint = false;
        }
    }
    IEnumerator PunishTime()
    {
        punish = true;
        yield return new WaitForSeconds(punishTime);
        punish = false;
    }
            

        
}
