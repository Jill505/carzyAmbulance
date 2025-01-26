using System.Collections;
using UnityEngine;

public class Line : MonoBehaviour
{
     [Header("預製體")]
    public GameObject DecisionLine; 

    [Header("時間限制")]
    public float duration = 10f;   

    private float distance;  
    private float speed;            
    private float elapsedTime = 0f; 
    
    void Start()
    {
        GameObject checkPoint = GameObject.FindWithTag("CheckPoint");

        distance = Vector3.Distance(checkPoint.transform.position, DecisionLine.transform.position);

        speed = distance / duration;
    }


    void FixedUpdate()
    {
        GameObject checkPoint = GameObject.FindWithTag("CheckPoint");
        HeartBeat heartBeat = checkPoint.GetComponent<HeartBeat>();
        if (elapsedTime < duration)
        {
            float moveDistance = speed * Time.deltaTime; 
            DecisionLine.transform.position = Vector3.MoveTowards(DecisionLine.transform.position, checkPoint.transform.position, moveDistance);
            elapsedTime += Time.deltaTime;
        }
        else
        {
            if(heartBeat.safe == false)
            {
                Debug.Log("一代一代");
            }
            Destroy(gameObject);
        }
    }
    
}
