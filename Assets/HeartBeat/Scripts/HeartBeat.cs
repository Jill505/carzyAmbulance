using System.Collections;
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

    public SpriteRenderer hintSpriteRenderer;
    public SpriteRenderer hintSpriteRenderer2;

    public float alphaRate = 1;
    public float alphaFadeRate = 1;
    public Coroutine AssistCoroutine;
    public bool tapTrigger;
    public bool TapTrigger { get { return TapTrigger; }
        set { 
            if (value == true)
            {
                if (tapTrigger == true)
                {
                    //輔助回歸A
                }
            }
        }
    }
    public GameObject border;

    void Start()
    {
        gameCore = GameObject.Find("GameCore").GetComponent<GameCore>();
        syncBPMFromGameCore();
        BPMSet();
        BPMsyncToText();
        StartCoroutine(InstantitateLine());   
    }
    void Update()
    {
        BPMSet();
        if (gameCore.GAME_MODE == 0)
        {
            if (alphaRate > 0.05f)
            {
                alphaRate -= Time.deltaTime * alphaFadeRate;
            }
            hintSpriteRenderer.color = new Color(hintSpriteRenderer.color.r, hintSpriteRenderer.color.g, hintSpriteRenderer.color.b, alphaRate);
            hintSpriteRenderer2.color = new Color(hintSpriteRenderer.color.r, hintSpriteRenderer.color.g, hintSpriteRenderer.color.b, alphaRate);
        }
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

        if (gameCore.GAME_MODE == 0)
        {
            yield return new WaitUntil(() => gameCore.gameRunning);
            while (gameCore.gameRunning)
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

                if (pendingNote > 0)
                {
                    //執行生成提示
                    if (AssistCoroutine != null)
                    {
                        StopCoroutine(AssistCoroutine);
                    }
                    AssistCoroutine = StartCoroutine(AssistHint());
                }
                else
                {

                }
            }
        }
    }
    
    public IEnumerator AssistHint()
    {
        float alpha = 1;
        yield return new WaitUntil(() => true);
        yield return null;
    }

    public void InjuryAndSpawnANote()
    {
        //Debug.Log("from Heartbeat.cs. the message called currectly");
        pendingNote ++;
        gameCore.heartbeatMiss();
    }

    public void insSoundEffect()
    {
        if (gameCore.hp < 35)
        {
            Instantiate(heartbeatSoundEffect_moreDanger);
        }
        else if(gameCore.hp < 80)
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

    public void hitJudgeReaction(int state)
    {
        if (state == 0)//perfect
        {
            alphaRate = 1;
        }
        else
        {
            alphaRate = 1;
        }
    }
}
