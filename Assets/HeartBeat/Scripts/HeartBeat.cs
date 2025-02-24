using System.Collections;
using UnityEditor.ShaderGraph;
using UnityEngine;
using TMPro;


public class Heartbeat : MonoBehaviour
{
    private GameCore gameCore;

    [Header("預製體")]
    public GameObject DecisionLine;

    [Header("生成位置")]
    public Transform InstantiatePoint; 

    [Header("生成拍子與節奏")]
    public int BPM = 120;            
    public float duration;   
    private int currentCount = 0;
    public int pendingNote = 0;

    public float preSpawnSoundEffectTime = 0.2f;
    public GameObject heartbeatSoundEffect_normal;
    public GameObject heartbeatSoundEffect_inDanger;
    public GameObject heartbeatSoundEffect_moreDanger;

    public TextMeshProUGUI bpmTextMesh;

    void Start()
    {
        gameCore = GameObject.Find("GameCore").GetComponent<GameCore>();
        syncBPMFromGameCore();
        BPMSet();
        BPMsyncToText();
        StartCoroutine(InstantitateLine());   
    }

    public void BPMChnage(int newBPM)
    {
        BPM = newBPM;
        syncBPMFromGameCore();
        BPMSet();
        BPMsyncToText();
    }


    public void BPMSet()
    {
        duration = 60f/(BPM*4f);
    }
    public void BPMsyncToText()
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
        yield return new WaitUntil(() => gameCore.gameRunning);
        while(gameCore.gameRunning)
        {
            yield return new WaitForSeconds(duration);
            currentCount++;

            if (pendingNote > 0)
            {
                Instantiate(DecisionLine, InstantiatePoint.position, InstantiatePoint.rotation);
                insSoundEffect();
                pendingNote--;
            }

            if (currentCount > 3)
            {
                currentCount = 0;
                pendingNote++;
            }
        }
    }
    
    

    public void InjuryAndSpawnANote()
    {
        //Debug.Log("from Heartbeat.cs. the message called currectly");
        pendingNote ++;
        gameCore.heartbeatMiss();
    }

    public void insSoundEffect()
    {
        if (gameCore.hp < 10)
        {
            Instantiate(heartbeatSoundEffect_moreDanger);
        }
        else if(gameCore.hp < 50)
        {
            Instantiate(heartbeatSoundEffect_inDanger);
        }
        else
        {
            Instantiate(heartbeatSoundEffect_normal);
        }
    }

    public void syncBPMFromGameCore()
    {
        BPM = gameCore.theBPM;
    }

    public void AssistSystem()
    {

    }
}
