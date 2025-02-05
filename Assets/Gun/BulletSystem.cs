using UnityEngine;

public class BulletSystem : MonoBehaviour
{
    private bool isHoldingBullet = false; // 是否正在持有子彈
    private bool hasBullet = false; // 是否已經獲得子彈
    private float lastClickTime = 0f;
    private const float doubleClickTime = 0.3f; // 雙擊時間間隔

    public BoxCollider2D bulletBoxCollider; // 子彈盒的 2D Collider
    public BoxCollider2D gunCollider; // 槍枝的 2D Collider

    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 取得子彈
        if (Input.GetMouseButtonDown(0) && IsMouseOver(bulletBoxCollider, mousePosition) && !hasBullet)
        {
            isHoldingBullet = true;
            Debug.Log("取得子彈數據");
        }

        // 放開時判斷是否進入槍枝範圍
        if (Input.GetMouseButtonUp(0))
        {
            if (isHoldingBullet)
            {
                if (IsMouseOver(gunCollider, mousePosition))
                {
                    hasBullet = true;
                    Debug.Log("成功將子彈放入槍枝");
                }
                else
                {
                    hasBullet = false;
                    Debug.Log("子彈數據消失");
                }

                isHoldingBullet = false;
            }
        }

        // 槍枝雙擊裝填
        if (hasBullet && Input.GetMouseButtonDown(0) && IsMouseOver(gunCollider, mousePosition))
        {
            if (Time.time - lastClickTime < doubleClickTime)
            {
                Debug.Log("填裝完畢！");
            }
            lastClickTime = Time.time;
        }
    }

    // 檢查滑鼠是否在指定的 BoxCollider2D 範圍內
    private bool IsMouseOver(BoxCollider2D collider, Vector2 point)
    {
        return collider.OverlapPoint(point);
    }
}
