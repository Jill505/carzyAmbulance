using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngineInternal;
using UnityEngine.UIElements;
using System.Collections.Generic;

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

    public GameObject heartbeatAssistObject;
    public float extendRate = 0.08f;

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
    public GameObject checkIfShouldChangeColor;

    [Header("判定順序")]
    private Dictionary<int, Line> activeLines = new Dictionary<int, Line>();
    public int nextLineToCheck = 1; // 目前該判定的Line編號
    private int currentLineNumber = 1;


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
            if (Input.GetKeyDown(KeyCode.Space)) 
            {
                ProcessInput();
            }
            if (alphaRate > 0.05f)
            {
                alphaRate -= Time.deltaTime * alphaFadeRate;
            }
            hintSpriteRenderer.color = new Color(hintSpriteRenderer.color.r, hintSpriteRenderer.color.g, hintSpriteRenderer.color.b, alphaRate);
            hintSpriteRenderer2.color = new Color(hintSpriteRenderer.color.r, hintSpriteRenderer.color.g, hintSpriteRenderer.color.b, alphaRate);


            if (extendRate > 0.08f)
            {
                extendRate -= Time.deltaTime * extendRate; 
            }
            heartbeatAssistObject.transform.localScale = new Vector3(heartbeatAssistObject.transform.localScale.x, extendRate, heartbeatAssistObject.transform.localScale.z);
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
    public void BPMChange(float arguement)
    {
        duration *= (1/arguement);
    }
    public void BPMsyncToText()
    {
        bpmTextMesh.text = "BPM: "+(int)BPM;
    }

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
        //float alpha = 1;
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
            alphaRate = 0.5f;
            extendRate = 0.1f;
        }
        else
        {
            alphaRate = 0.5f;
            extendRate = 0.1f;
        }
    }

    public void RegisterLine(Line line, int lineNumber)
    {
        if (!activeLines.ContainsKey(lineNumber))
        {
            activeLines.Add(lineNumber, line);
        }
    }

    private void ProcessInput()
    {
        if (activeLines.ContainsKey(nextLineToCheck))
        {
            Line line = activeLines[nextLineToCheck];
            if (line.InThePunishPoint)
            {
                if(line != null && !line.IsChecked() )
                {
                    line.CheckInput(); // 執行按鍵判定
                }
            }
            if (line.InTheGoodPoint || line.InThePerfectPoint)
            {
                if(line != null && !line.IsChecked() && !line.punish)
                {
                    line.CheckInput(); // 執行按鍵判定
                    nextLineToCheck++; // 確保下次只能檢查下一條 Line
                }
            }
        }
    }

    public void RemoveLine(int lineNumber)
    {
        if (activeLines.ContainsKey(lineNumber))
        {
            activeLines.Remove(lineNumber);
        }
    }
    public int GetNextLineNumber()
    {
        return currentLineNumber++;
    }
}




