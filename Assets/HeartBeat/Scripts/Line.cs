using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Line : MonoBehaviour
{
    private Heartbeat Heartbeat;
    private GameCore gameCore;
    public float speed = 5f; 
    private bool getPoint = false;
    private bool hasExecuted = false;
    private bool InThePerfectPoint = false;
    private bool InTheGoodPoint = false;

    public SpriteRenderer childSpriteRenderer;
    public Animator myAnimator;

    void Start()
    {
        Heartbeat = GameObject.Find("Chart").GetComponent<Heartbeat>();
        gameCore = GameObject.Find("GameCore").GetComponent<GameCore>();

        myAnimator = gameObject.GetComponent<Animator>();

        childSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
        InputSpace();

        if (transform.position.x < Heartbeat.checkIfShouldChangeColor.transform.position.x)
        {
            if (getPoint != true && hasExecuted == false)
            {
                //Debug.Log("a hint from Line.cs, the function triggered currecrt");
                gameCore.damagedHintFunc();
                Heartbeat.InjuryAndSpawnANote();

                //SpriteRenderer childSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
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
            }
        }

        if (transform.position.x < Heartbeat.border.transform.position.x)
        {
                Destroy(gameObject);
        }

    }        

    void InputSpace()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // 當按下 Space
        {
            CheckInput(); 
        }
    }

    bool ckptClog = false;
    void CheckInput()
    {
            if(InThePerfectPoint)
            {
                Debug.Log("完美");
                getPoint = true;
                gameCore.heartbeatHit();
                gameCore.perfectHintFunc();
                gameCore.showHintText(1);


                childSpriteRenderer.color = new Color(0, 1, 0, 1);
                //Play Animation
                myAnimator.SetTrigger("makeItClear");
                Debug.Log("CClear");
            }
            else if(InTheGoodPoint)
            {
                Debug.Log("很好");
                getPoint = true;
                gameCore.heartbeatHit();
                gameCore.perfectHintFunc();
                gameCore.showHintText(2);


                childSpriteRenderer.color = new Color(0, 1, 0, 1);
                //Play Animation
                
                myAnimator.SetTrigger("makeItClear");
                Debug.Log("CClear");
            }
            
        }
        
        
    
    void OnTriggerEnter2D(Collider2D collision)
    {    
        
        if (collision.gameObject.tag == "Good")
        {
            InTheGoodPoint = true;
        }
        if (collision.gameObject.tag == "Perfect")
        {
            InThePerfectPoint = true;
            //gameCore.perfectHintFunc();
        }

        
    }
    void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Good")
        {
            InTheGoodPoint = false;
        }
        if (collision.gameObject.tag == "Perfect")
        {
            InThePerfectPoint = false;
        }
    }
            

        
}
