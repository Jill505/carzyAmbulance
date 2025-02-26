using UnityEngine;

public class Cthulhu : MonoBehaviour
{
    public float counting;
    public SpriteRenderer sr;
    public int hp = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        counting += Time.deltaTime;
        if (counting >= 8)
        {
            //¶€√z
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
        GameObject.Find("Chart").GetComponent<Heartbeat>().pendingNote += 5;
        
        //GameObject.Find("GameCore").GetComponent<GameCore>().PlaySoundEffect();
        Destroy(gameObject);
    }
}
