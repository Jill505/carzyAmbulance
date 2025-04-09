using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Heartbeat heartbeat;
    private GameCore gamecore;
    
    [Header("基本數值")]
    public float health = 3;
    public float attackFrequency = 5f;

    [Header("大小變化")]  //做出遠近效果
    //private Vector3 targetScale = Vector3.one * 0.2f;  //目標大小（0.2 倍）
    

    [Header("左右移動")]
    public float moveSpeed = 5f;  //速度
    private Vector3 moveDirection = Vector3.right; //初始方向

    

    private bool canMove = true;
    private bool used = false;
    private bool shootRange;   

    public Animator animator;
    
    

    void Start()
    {
        heartbeat = GameObject.Find("Chart").GetComponent<Heartbeat>();   
        gamecore = GameObject.Find("GameCore").GetComponent<GameCore>();
        gamecore.PlaySoundEffect(gamecore.SoundEffects[12]); 
        
        moveSpeed += Random.Range(0.2f,0.8f); 
        attackFrequency += Random.Range(-2f,2f);
        
        StartCoroutine(AttackCycle());
        
        
    }

    void Update()
    {
        //HandleScaling();
        HandleMovement();
        HandleTilting();
         
        
    }

    void LateUpdate() 
    {
        if (Mathf.Approximately(transform.position.y, -0.06547654f) && used == false)
        {
            Renderer renderer = GetComponent<Renderer>();
            transform.localScale = new Vector3(0.16f, 0.16f, 0.16f);
            renderer.sortingOrder = 0;
        }
    }

    

    //敵人變大的過程
    /*void HandleScaling()
    {
        
            if (isAttacking)
            {
                attackTimer += Time.deltaTime;
                float progressAttack = Mathf.Clamp01(attackTimer / attackDuration);
                transform.localScale = Vector3.Lerp(targetScale, Vector3.one * 0.3f, progressAttack);
                
            }
            else if (isReturningToTargetScale)
            {
                returnTimer += Time.deltaTime;
                float progressReturn = Mathf.Clamp01(returnTimer / returnDuration);
                transform.localScale = Vector3.Lerp(Vector3.one * 0.3f, targetScale, progressReturn);
            }
    }*/
    

    //敵人左右移動
    void HandleMovement()
    {
        if (canMove)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
    }

    //敵人的傾斜
   void HandleTilting()
   {
        if (moveDirection.x > 0)
        {
            animator.SetBool("TileL",false);
            animator.SetBool("TileR",true);
        }
        else if(moveDirection.x < 0)
        {
            animator.SetBool("TileR",false);
            animator.SetBool("TileL",true);
        }
    }


    // 當敵人碰到牆壁時，會進行方向反轉並等幾秒
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall") && canMove)
        {
            StartCoroutine(WaitAndChangeDirection(0.5f)); 
        }

        if (other.CompareTag("ShootRange"))
        {
            shootRange = true; 
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("ShootRange"))
        {
            shootRange = false; 
        }
    }

    IEnumerator WaitAndChangeDirection(float waitTime)
    {
        canMove = false; 
        yield return new WaitForSeconds(waitTime);
        moveDirection *= -1; 
        canMove = true;
    }

    IEnumerator AttackCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackFrequency);  
            

            if(shootRange)
            {
                animator.SetTrigger("AttackCycle");  
                Attack();  
            }
        }
    }


    

    public void Attack()
    {
        if (gamecore.gameRunning)
        {
            gamecore.PlaySoundEffect(gamecore.SoundEffects[13]); 
            heartbeat.BanditAttack();
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            used = true;
            animator.SetBool("FlipDown", true);
            gamecore.PlaySoundEffect(gamecore.SoundEffects[14]); 
        }
    }
    

    

    void Die()
    {
        Destroy(gameObject);
    }
}
