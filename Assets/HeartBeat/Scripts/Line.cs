using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Line : MonoBehaviour
{
    private Heartbeat Heartbeat;
    private GameCore gameCore;
    public float speed = 5f; 
    private bool CheckTime = false;
    private bool getPoint = false;
    private bool hasExecuted = false;
    private float spacetime;

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
            spacetime = 0f; // 重置時間
        }

        if (Input.GetKey(KeyCode.Space)) // 當持續按住 Space
        {   
            spacetime += Time.deltaTime; // 記錄長按的時間
        }

        if (Input.GetKeyUp(KeyCode.Space)) // 當放開 Space
        {   
            Debug.Log("Space 按住時間：" + spacetime + " 秒");
            CheckInput(); 
        }
    }

    bool ckptClog = false;
    void CheckInput()
    {
        if(CheckTime)
        {
            if(spacetime >= 0.1f && spacetime < 0.2f)
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
            else if(spacetime >= 0.05f && spacetime < 0.1f || spacetime >= 0.2f && spacetime < 0.3f)
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
        
        
    }
    void OnTriggerEnter2D(Collider2D collision)
    {    
        if (collision.gameObject.tag == "CheckPoint")
        {
            CheckTime = true;
            //gameCore.perfectHintFunc();
        }
        
    }
    void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "CheckPoint")
        {
            CheckTime = false;
        }
    }
            

        
}
