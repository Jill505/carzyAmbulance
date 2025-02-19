using System.Collections;
using UnityEditor.ShaderGraph;
using UnityEngine;
using TMPro;


public class Heartbeat : MonoBehaviour
{
    //public GameCore gameCore;

    [Header("預製體")]
    public GameObject DecisionLine;

    [Header("生成位置")]
    public Transform InstantiatePoint; 

    [Header("生成拍子與節奏")]
    public int BPM = 120;            
    public float duration;   
    private int currentCount = 0;
    private int pendingNote = 0;

    public TextMeshProUGUI bpmTextMesh;

    void Start()
    {
        BPMSet();
        BPMsync();
        StartCoroutine(InstantitateLine());
        /*
        if(gameCore.gameRunning)
        {
            BPMSet();
            //BPMsync();
            StartCoroutine(InstantitateLine());
            InputSpace();
        }*/
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))  //受傷增加節拍(用Q代替
        {
            InjuryAndSpawnANote();
        }
    }
    public void BPMSet()
    {
        duration = 60f/(BPM*4f);
    }
    public void BPMsync()
    {
        bpmTextMesh.text = "BPM: "+(int)BPM;
    }
/*
    IEnumerator gameGoing()
    {
        while(isGamePlaying == true)
        {
            //yield return null;// update in coroutine
            yield return new WaitForSeconds(duration);
            Debug.Log("gameGoing count");
            //judge if the position can spawn
            currentCount ++;

            if(currentCount > 3)
            {
                currentCount = 0;
                //spawn a note;
                spawnDuritionCount ++;
                spawnDuritionCount ++;
            }

            //judge and spawn
            if(spawnDuritionCount >0)
            {
                spawnDuritionCount --;
                //spawn a note
                InstantitateLine();
                Debug.Log("Ins line");
            }


        }
        yield return null;
    }
*/
    IEnumerator InstantitateLine()
    {
        while(true)
        {
            yield return new WaitForSeconds(duration);
            currentCount++;

            if(currentCount > 3)
            {
                currentCount = 0;
                pendingNote++;
            }

            if(pendingNote > 0)
            {
                Instantiate(DecisionLine, InstantiatePoint.position, InstantiatePoint.rotation);
                pendingNote--;
            }
        }
    }

    
    

    public void InjuryAndSpawnANote()
    {
        pendingNote ++;
    }

}
