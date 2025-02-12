using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class BloodPackDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image bloodPackImage;        // 參考血袋圖片
    public Image NewbloodPackImage;
    private RectTransform rectTransform;  // 用來控制圖片位置的 RectTransform
    private Vector3 originalPosition;    // 儲存血袋的原始位置
    private bool isDragging = false;     // 是否正在拖曳

    // 用於設定觸發放開範圍，超出範圍則隱藏血袋
    public float dragOutOfBoundsDistance = 100f;

    void Start()
    {
        rectTransform = bloodPackImage.GetComponent<RectTransform>();
        originalPosition = rectTransform.position;             // 記錄血袋的初始位置
    }

    // 開始拖曳時觸發
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    // 拖曳過程中，更新圖片位置
    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            // 設置圖片位置為當前滑鼠位置
            rectTransform.position = eventData.position;
        }
    }

    // 拖曳結束時觸發
    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        // 計算拖曳結束時血袋位置與原始位置的距離
        float distanceFromOriginalPosition = Vector3.Distance(rectTransform.position, originalPosition);

        if (distanceFromOriginalPosition > dragOutOfBoundsDistance) // 若拖曳超出範圍
        {
            StartCoroutine(FadeOutBloodPack()); // 開始淡出血袋
        }
        else
        {
            // 如果拖曳位置在範圍內，將血袋回到原來的位置
            rectTransform.position = originalPosition;
        }
    }

    // 淡出血袋效果
    private IEnumerator FadeOutBloodPack()
    {
        float alpha = bloodPackImage.color.a;

        // 淡出血袋
        while (alpha > 0)
        {
            alpha -= Time.deltaTime * 2f; // 每幀減少透明度
            bloodPackImage.color = new Color(1, 1, 1, alpha); // 設置新的透明度
            yield return null;
        }

        // 完全透明後隱藏血袋
        bloodPackImage.gameObject.SetActive(false);
    }

    // 可選的重置血袋位置方法，用於外部調用
    public void ResetBloodPack()
    {
        bloodPackImage.gameObject.SetActive(true); // 使血袋重新顯示
        bloodPackImage.color = new Color(1, 1, 1, 1); // 恢復透明度為 1
        rectTransform.position = originalPosition; // 重設位置
    }
}
