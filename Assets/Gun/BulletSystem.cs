using System.Collections;
using UnityEngine;

public class BulletSystem : MonoBehaviour
{
    [Header ("存放碰撞體")]
    public BoxCollider2D bulletBox; 
    public BoxCollider2D bloodPack;   
    public BoxCollider2D Medicine;   
    public BoxCollider2D shootRange; 


    [Header ("圖標")]
    public Texture2D crosshairShoot;    
    public Texture2D crosshairNoBullet; 
    public Texture2D crosshairHand; 

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
    private GameCore gameCore;
    private SyringSystem syringSystem;
    public SoundEffect soundEffect;
    private DraggableBloodPack draggableBloodPack;

    void Start()
    {
        gameCore = GameObject.Find("GameCore").GetComponent<GameCore>();     
        syringSystem = GameObject.Find("SyringSystem").GetComponent<SyringSystem>(); 
        //draggableBloodPack = GameObject.Find("BloodPackBox").GetComponent<DraggableBloodPack>();
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
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            //changebackthemouse
        }
    }

    void HandleCrosshair()
    {
        Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition, LayerMask.GetMask("Enemy"));
        bool isMouseOverCthulhu = hitCollider != null && hitCollider.TryGetComponent<Cthulhu>(out _); // 確保 hitCollider 不是 null 才檢查 Cthulhu
        if(IsMouseOver(shootRange, mousePosition) || isMouseOverCthulhu)
        {
            if (loadIn && currentAmmo > 0)
            {
                Cursor.SetCursor(crosshairShoot, new Vector2(64, 64), CursorMode.Auto);
                isInShootRange = true;
            }
            else
            {
                Cursor.SetCursor(crosshairNoBullet, new Vector2(64, 64), CursorMode.Auto);
                isInShootRange = true;
            }
        }
        
        
        else if(IsMouseOver(bulletBox, mousePosition) )
        {
            Cursor.SetCursor(crosshairHand, new Vector2(64, 64), CursorMode.Auto);
        }
        else if(IsMouseOver(Medicine, mousePosition) && syringSystem.allMedicineUsed == false)
        {
            Cursor.SetCursor(crosshairHand, new Vector2(64, 64), CursorMode.Auto);
        }
        else if(IsMouseOver(bloodPack, mousePosition)&& draggableBloodPack.isLastBloodPack == false)
        {
            Cursor.SetCursor(crosshairHand, new Vector2(64, 64), CursorMode.Auto);
        }

        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            isInShootRange = false;
        }
    }

    void HoldBullet()
    {
        if (Input.GetMouseButtonDown(0) && IsMouseOver(bulletBox, mousePosition) && !loadIn)
        {
            isHoldingBullet = true;
            //Debug.Log("取得子彈數據");
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
                /*
                if (IsMouseOver(gunCollider, mousePosition))
                {
                */
                    soundEffect.ReloadSound();
                    currentAmmo = maxAmmo;
                    Debug.Log("成功將子彈放入槍枝");
                    loadIn = true;
                    ChangeColor(bulletbox,"#806A6A");
                /*
                }
                else
                {
                    Debug.Log("子彈數據消失");
                }
                */

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
        soundEffect.ShotGunSound();
        if (currentAmmo <= 0)
        {
            loadIn = false;
            ChangeColor(bulletbox,"#FFFFFF");
        }

        Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition, LayerMask.GetMask("Enemy"));
        if (hitCollider != null)
        {
            if (hitCollider.gameObject.TryGetComponent<Enemy>(out Enemy theEnemy) == true)
            {
                //theEnemy = hitCollider.GetComponent<Enemy>();
                theEnemy.TakeDamage(Firepower);
                Debug.Log("Hit Enemy");
            }
            else if (hitCollider.gameObject.TryGetComponent<Cthulhu>(out Cthulhu cthulhu) == true)
            {
                cthulhu.TakeDamage();
                Debug.Log("Hit Cthulhu");
            }
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
        Vector2 originalHotspot = new Vector2(64,64);
        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 3f);
            float offsetY = Random.Range(-1f, 3f);
            Vector2 shakeHotspot = originalHotspot + new Vector2(offsetX, offsetY);

            // 設定游標
            Cursor.SetCursor(crosshairNoBullet, shakeHotspot, CursorMode.Auto);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        Cursor.SetCursor(crosshairNoBullet, Vector2.zero, CursorMode.Auto);
    }


    private bool IsMouseOver(BoxCollider2D collider, Vector2 point)
    {
        return collider.OverlapPoint(point);
    }

    void ChangeColor(GameObject obj,string hexColor)
    {
        if (ColorUtility.TryParseHtmlString(hexColor, out Color newColor))
        {
            SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sprite in sprites)
            {
                sprite.color = newColor;
            }
        }
    }

    
}
