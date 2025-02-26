using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Line : MonoBehaviour
{
    private Heartbeat Heartbeat;
    private GameCore gameCore;
    public float speed = 5f; 
    private float liveTime = 0f;
    private bool InThePerfectPoint = false;
    private bool InTheGoodPoint = false;
    private bool getPoint = false;

    void Start()
    {
        Heartbeat = GameObject.Find("Chart").GetComponent<Heartbeat>();
        gameCore = GameObject.Find("GameCore").GetComponent<GameCore>();   
    }

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
        InputSpace();

        if (transform.position.x < Heartbeat.border.transform.position.x)
        {
            if (getPoint != true)
            {
                //Debug.Log("a hint from Line.cs, the function triggered currecrt");
                Heartbeat.InjuryAndSpawnANote();
                gameCore.damagedHintFunc();
            }
            Destroy(gameObject);
        }

        /*
        liveTime += Time.deltaTime;
        if (liveTime >= 1.5f)
        {
            if(getPoint != true)
            {
                //Debug.Log("a hint from Line.cs, the function triggered currecrt");
               Heartbeat.InjuryAndSpawnANote();
            }
            Destroy(gameObject);
        }*/
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
