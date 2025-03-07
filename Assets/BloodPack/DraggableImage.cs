using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableBloodPack : MonoBehaviour,  IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //public Image activeSlot; 
    //public BloodPackDrag bloodPackDrag;
    public GameCore gameCore;
    public Image newbloodPack;
    private Image bloodPackBox;

    public Sprite[] bloodPackSprites; // 存放四張不同的圖片
    private int currentIndex = 0;

    private bool isDragging = false;
    private Vector2 mousePosition;
    private Vector2 originalPos;
    public bool isLastBloodPack = false; 

    private void Start()
    {
        bloodPackBox = GetComponent<Image>();
        originalPos = newbloodPack.rectTransform.position;
    }

    public void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        /*if (bloodPackDrag.gameObject.activeSelf)
        {
            return; 
        }*/

        if (bloodPackSprites.Length == 0) return; // 確保有圖片可切換
        
        if (currentIndex < bloodPackSprites.Length - 1) // 確保不超過最後一張
        {
            currentIndex++; 
            bloodPackBox.sprite = bloodPackSprites[currentIndex]; // 設定新的圖片
            isDragging = true;
            newbloodPack.rectTransform.position = mousePosition; 
        }
        if (currentIndex == bloodPackSprites.Length - 1)
        {
            isLastBloodPack = true;  // 設置為 true
        }
        else
        {
            isLastBloodPack = false; // 不是最後一張則設置為 false
        }
        
        
    }
    

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return; 
        newbloodPack.rectTransform.position = mousePosition; 
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return; 

        ChangeBloodPack();
        isDragging = false; 
        newbloodPack.rectTransform.position = originalPos; 
    }

    public void ChangeBloodPack()
    {
        //bloodPackDrag.gameObject.SetActive(true);
        gameCore.ChangeBloodPack();
    }
}
