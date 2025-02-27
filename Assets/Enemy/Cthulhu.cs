using UnityEngine;

public class Cthulhu : MonoBehaviour
{
    public float counting;
    public SpriteRenderer sr;
    public int hp = 1;
    private GameCore gameCore;
    private Heartbeat heartbeat;
    void Start()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        gameCore = GameObject.Find("GameCore").GetComponent<GameCore>();
        heartbeat = GameObject.Find("Chart").GetComponent<Heartbeat>();
    }

    void Update()
    {
        counting += Time.deltaTime;
        if (counting >= 8)
        {
            //���z
            kys();
        }
    }
    void FixedUpdate()
    {
        float swapRate = counting / 100;
        float xRan = swapRate /Random.Range(-5f,5f);
        float yRan = swapRate /Random.Range(-5f, 5f);
        transform.position = new Vector2(transform.position.x + xRan, transform.position.y + yRan );    
    }
    public void TakeDamage()
    {
        hp--;
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void kys()
    {
        //GameObject.Find("Chart").GetComponent<Heartbeat>().pendingNote += 5;

        for(int note = 0;note < 5;note++)
        {
            while(gameCore.NoteDistancetimer >= 0.3f)
            {
                heartbeat.pendingNote ++;
                gameCore.PlaySoundEffect(gameCore.SoundEffects[0]);
                gameCore.NoteDistancetimer = 0;
            }
        }
        
        //GameObject.Find("GameCore").GetComponent<GameCore>().PlaySoundEffect();
        Destroy(gameObject);
    }
}
