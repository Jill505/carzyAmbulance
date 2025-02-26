using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Line : MonoBehaviour
{
    private Heartbeat Heartbeat;
    private GameCore gameCore;
    public float speed = 5f; 
    private bool InThePerfectPoint = false;
    private bool InTheGoodPoint = false;
    private bool getPoint = false;
    private bool hasExecuted = false;

    void Start()
    {
        Heartbeat = GameObject.Find("Chart").GetComponent<Heartbeat>();
        gameCore = GameObject.Find("GameCore").GetComponent<GameCore>();   
       
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
                SpriteRenderer childSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
                if (childSpriteRenderer != null)
                {
                    childSpriteRenderer.color = Color.red;
                }
                hasExecuted = true;
            }
        }

        if (transform.position.x < Heartbeat.border.transform.position.x)
        {
            if (getPoint != true)
            {
                Destroy(gameObject);
            }
            
        }

    }        

    void InputSpace()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            CheckInput();
        }
    }
    void CheckInput()
    {
        if(InThePerfectPoint)
        {
            Debug.Log("完美");
            getPoint = true;
            gameCore.heartbeatHit();
            gameCore.perfectHintFunc();
            gameCore.showHintText(1);
        }
        else if(InTheGoodPoint && !InThePerfectPoint)
        {
            Debug.Log("很好");
            getPoint = true;
            gameCore.heartbeatHit();
            gameCore.perfectHintFunc();
            gameCore.showHintText(2);
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Good")
        {
            InTheGoodPoint = true;
            //gameCore.perfectHintFunc();
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
