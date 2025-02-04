using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Line : MonoBehaviour
{
     [Header("預製體")]
    public GameObject DecisionLine; 

    [Header("時間限制")]
    private float duration;

    private float distance;  
    public float speed;            
    private float elapsedTime = 0f;

    [Header("邊界參考物")]
    public GameObject borderCheckpoint;

    bool callClug = false;
    
    void Start()
    {
        GameObject checkPoint = GameObject.FindWithTag("CheckPoint");
        borderCheckpoint = GameObject.Find("Border");
        HeartBeat heartBeat = checkPoint.GetComponent<HeartBeat>();
        duration = heartBeat.duration;

        distance = Vector3.Distance(checkPoint.transform.position, DecisionLine.transform.position);

        speed = distance / duration;
    }


    void FixedUpdate()
    {
        GameObject checkPoint = GameObject.FindWithTag("CheckPoint");
        HeartBeat heartBeat = checkPoint.GetComponent<HeartBeat>();

        gameObject.transform.localPosition = new Vector3(transform.position.x - (speed * Time.deltaTime), transform.position.y, transform.position.z);

        if (elapsedTime < duration)
        {
            float moveDistance = speed * Time.deltaTime; 
            //DecisionLine.transform.position = Vector3.MoveTowards(DecisionLine.transform.position, checkPoint.transform.position, moveDistance);
            //gameObject.transform.localPosition = new Vector3(transform.position.x - (speed * Time.deltaTime), transform.position.y, transform.position.z);
            elapsedTime += Time.deltaTime;
        }
        else
        {
            if (callClug == false)
            {
                if (heartBeat.safe == false)
                {
                    Debug.Log("一代一代");
                }

                heartBeat.fixedGenAllow = true;
                callClug = true;
            }
            //Destroy(gameObject);
        }
        if (gameObject.transform.position.x < borderCheckpoint.transform.position.x)
        {
            Destroy(gameObject);
        }
    }
}
