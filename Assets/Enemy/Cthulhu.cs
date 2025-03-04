using UnityEngine;
using System.Collections;

public class Cthulhu : MonoBehaviour
{
    public float counting;
    public SpriteRenderer sr;
    public int hp = 1;
    private GameCore gameCore;
    private Heartbeat heartbeat;

    public GameObject magicChild;
    public Animator magicAnimator;
    public SpriteRenderer mySr;

    public bool onDieWay = false;

    Vector2 edgeTL;
    Vector2 edgeBR;
    void Start()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        gameCore = GameObject.Find("GameCore").GetComponent<GameCore>();
        heartbeat = GameObject.Find("Chart").GetComponent<Heartbeat>();

        edgeTL = GameObject.Find("edgeTL").GetComponent<Transform>().position;
        edgeBR = GameObject.Find("edgeRB").GetComponent<Transform>().position;

        //just in case, check.
        if (magicChild == null || magicAnimator == null)
        {
            magicChild = gameObject.transform.GetChild(0).gameObject;
            magicAnimator = magicChild.gameObject.GetComponent<Animator>();
        }

        mySr = gameObject.GetComponent<SpriteRenderer>();

        StartCoroutine(onSpawn());
    }

    void Update()
    {
        if (onDieWay == false)
        {
            counting += Time.deltaTime;
            if (counting >= 8)
            {
                //���z
                kys();
            }
        }
    }
    void FixedUpdate()
    {
        float swapRate = counting / 100;
        float xRan = (swapRate *Random.Range(-3f,3f)) /5;
        float yRan = (swapRate *Random.Range(-3f, 3f)) /5;
        transform.position = new Vector2(transform.position.x + xRan, transform.position.y + yRan );    

        if (transform.position.x > edgeBR.x)
        {
            transform.position = new Vector2(edgeBR.x, transform.position.y);
        }
        else if (transform.position.x < edgeTL.x)
        {
            transform.position = new Vector2(edgeTL.x, transform.position.y);
        }

        if (transform.position.y > edgeTL.y)
        {
            transform.position = new Vector2(transform.position.x, edgeTL.y);
        }
        else if (transform.position.y < edgeBR.y)
        {
            transform.position = new Vector2(transform.position.x, edgeBR.y);
        }
    }
    public void TakeDamage()
    {
        hp--;
        if (hp <= 0)
        {
            StartCoroutine(onDie());
            //Destroy(gameObject);
        }
    }
    public void kys()
    {
        GameObject.Find("Chart").GetComponent<Heartbeat>().pendingNote += 5;

        /*
        for(int note = 0;note < 5;note++)
        {
            while(gameCore.NoteDistancetimer >= 0.3f)
            {
                heartbeat.pendingNote ++;
                gameCore.PlaySoundEffect(gameCore.SoundEffects[0]);
                gameCore.NoteDistancetimer = 0;
            }
        }*/

        gameCore.PlaySoundEffect(gameCore.SoundEffects[0]);
        Destroy(gameObject);
    }

    IEnumerator onSpawn()
    {
        onDieWay = true;

        //make spwan animation.
        magicAnimator.SetBool("spawn", true);

        float alpha = 0f;
        StartCoroutine(fadeIn());
        for (int i = 0; i < 20; i++)
        {
            alpha += 0.05f;
            mySr.color = new Color(mySr.color.r, mySr.color.g, mySr.color.b,alpha);
            yield return new WaitForSeconds(0.02f);
        }

        yield return null;
        onDieWay = false;
    }
    IEnumerator fadeIn()
    {
        yield return null;
    }

    IEnumerator onDie()
    {
        onDieWay = true;
        yield return null;
        //make die animation

        float alpha = 1f;

        for (int i = 0; i < 20; i++)
        {
            alpha -= 0.05f;
            mySr.color = new Color(mySr.color.r, mySr.color.g, mySr.color.b, alpha);
            yield return new WaitForSeconds(0.02f);
        }

        Destroy(gameObject);
    }
}
