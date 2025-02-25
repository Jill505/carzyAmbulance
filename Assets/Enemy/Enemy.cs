using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Heartbeat heartbeat;
    
    [Header("基本數值")]
    public float health = 3;
    public float attackFrequency = 5f;

    [Header("大小變化")]  //做出遠近效果
    private Vector3 targetScale = Vector3.one * 0.2f;  //目標大小（0.2 倍）
    

    [Header("左右移動")]
    public float moveSpeed = 5f;  //速度
    private Vector3 moveDirection = Vector3.right; //初始方向

    [Header("傾斜")]
    public float tiltAngle = 15f; //最大傾斜角
    private float currentTiltAngle = 0f; //當前傾斜角
    public float tiltSpeed = 5f; //傾斜變化的速度


    private bool canMove = true;
    private bool shootRange; 

    private float attackDuration = 1.2f;  //攻擊過程的持續時間
    private bool isAttacking = false;
    private float attackTimer = 0f;  

    private float returnDuration = 1.2f; //攻擊結束的過渡時間
    private bool isReturningToTargetScale = false;
    private float returnTimer = 0f;
    

    void Start()
    {
        heartbeat = GameObject.Find("Chart").GetComponent<Heartbeat>();     
        StartCoroutine(AttackCycle());
    }

    void Update()
    {
        HandleScaling();
        HandleMovement();
        HandleTilting();
    }

    

    //敵人變大的過程
    void HandleScaling()
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
    }
    

    //敵人左右移動
    void HandleMovement()
    {
        if (canMove && !isAttacking && !isReturningToTargetScale)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
    }

    //敵人的傾斜
   void HandleTilting()
{
    if (!isAttacking && !isReturningToTargetScale)  
    {
        float targetTilt;

        if (moveDirection.x > 0)
        {
            targetTilt = tiltAngle;  //當 moveDirection.x 為正時，目標傾斜角度為 tiltAngle
        }
        else
        {
            targetTilt = -tiltAngle;  //當 moveDirection.x 為負時，目標傾斜角度為 -tiltAngle
        }

        //平滑過渡
        currentTiltAngle = Mathf.Lerp(currentTiltAngle, targetTilt, tiltSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, currentTiltAngle);
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
                isAttacking = true; 
                attackTimer = 0f;  
                Attack();  
            }

            yield return new WaitForSeconds(1.2f); 

            isAttacking = false;  

            returnTimer = 0f;
            isReturningToTargetScale = true;  

            yield return new WaitForSeconds(1.2f);
            isReturningToTargetScale = false;
        }
    }


    

    public void Attack()
    {
        heartbeat.InjuryAndSpawnANote();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
