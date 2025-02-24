using System.Collections;
using UnityEngine;

public class BulletSystem : MonoBehaviour
{
    [Header ("存放碰撞體")]
    public BoxCollider2D bulletBoxCollider; 
    public BoxCollider2D gunCollider;   
    public BoxCollider2D shootRange; 


    [Header ("圖標")]
    public Texture2D crosshairShoot;    
    public Texture2D crosshairNoBullet; 

    [Header ("彈藥量")]
    public int maxAmmo = 5; 
    private int currentAmmo; 

    [Header ("攻擊力")]
    public float Firepower = 1f; 

    
    private bool isInShootRange = false; 
    private bool isHoldingBullet = false; 
    private bool loadIn = false;
    private Vector2 mousePosition;
    private Vector2 originalPos;

    public GameObject bulletbox;
    public GameCore gameCore;

    void Start()
    {
        gameCore = GameObject.Find("GameCore").GetComponent<GameCore>();     
        originalPos = bulletbox.transform.position;    
    }
    void Update()
    {
        if (gameCore.gameRunning)
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            HandleCrosshair(); //鼠標
            HoldBullet();     //拿子彈
            //LoadIn();         //上膛
            HandleShooting(); //射擊   
        }    
    }

    void HandleCrosshair()
    {
        if(IsMouseOver(shootRange, mousePosition))
        {
            if (loadIn && currentAmmo > 0)
            {
                Cursor.SetCursor(crosshairShoot, Vector2.zero, CursorMode.Auto);
                isInShootRange = true;
            }
            else
            {
                Cursor.SetCursor(crosshairNoBullet, Vector2.zero, CursorMode.Auto);
                isInShootRange = true;
            }
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            isInShootRange = false;
        }
    }

    void HoldBullet()
    {
        if (Input.GetMouseButtonDown(0) && IsMouseOver(bulletBoxCollider, mousePosition) && !loadIn)
        {
            isHoldingBullet = true;
            Debug.Log("取得子彈數據");
        }

        if (isHoldingBullet)
        {
            bulletbox.transform.position = mousePosition; 
        }
        else if(!isHoldingBullet)
        {
            bulletbox.transform.position = originalPos;
        }

        // 放開時判斷是否進入槍枝範圍
        if (Input.GetMouseButtonUp(0))
        {
            if (isHoldingBullet)
            {
                if (IsMouseOver(gunCollider, mousePosition))
                {
                    currentAmmo = maxAmmo;
                    Debug.Log("成功將子彈放入槍枝");
                    loadIn = true;
                }
                else
                {
                    Debug.Log("子彈數據消失");
                }

                isHoldingBullet = false;
            }
        }
        
    }

    /*void LoadIn()
    {
        if (hasBullet && Input.GetMouseButtonDown(0) && IsMouseOver(gunCollider, mousePosition))
        {
            if (Time.time - lastClickTime < doubleClickTime)
            {
                currentAmmo = maxAmmo;
                Debug.Log("填裝完畢！");
                hasBullet = false;
                loadIn = true;
            }
            lastClickTime = Time.time;
        }
    }*/

        
    void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0) && isInShootRange)
        {
            if (loadIn)
            {
                Shoot();
            }
            else
            {
                StartCoroutine(ShakeCursor()); // 震動鼠標
            }
        }
    }

    void Shoot()
    {
        currentAmmo--;
        if (currentAmmo <= 0)
        {
            loadIn = false;
        }

        Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition, LayerMask.GetMask("Enemy"));
        if (hitCollider != null)
        {
            Enemy enemy = hitCollider.GetComponent<Enemy>();
            enemy.TakeDamage(Firepower); 
            Debug.Log("ㄚㄚ好痛");
        }
        else
        {
            Debug.Log("空氣");
        }
    }

    IEnumerator ShakeCursor()
    {
        float duration = 0.5f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float offsetX = Random.Range(-3f, 3f);
            float offsetY = Random.Range(-3f, 3f);
            Cursor.SetCursor(crosshairNoBullet, new Vector2(offsetX, offsetY), CursorMode.Auto);

            elapsed += Time.deltaTime;
            yield return null;
        }
        Cursor.SetCursor(crosshairNoBullet, Vector2.zero, CursorMode.Auto);
    }


    private bool IsMouseOver(BoxCollider2D collider, Vector2 point)
    {
        return collider.OverlapPoint(point);
    }
}
