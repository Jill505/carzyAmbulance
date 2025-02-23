using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class BloodPackDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image bloodPackImage;       
    private RectTransform rectTransform; 
    private Vector3 originalPosition;    
    private bool isDragging = false;    
    private float dragStartTime;        
    private float dragTime = 0f;         
    
    [Header("拖曳設定")]
    public float dragOutOfBoundsDistance = 100f;
    public float dragDelayTime = 2f;    
    public float dragResistance = 0.5f; 

    void Start()
    {
        rectTransform = bloodPackImage.GetComponent<RectTransform>();
        originalPosition = rectTransform.position;             
    }

    void Update()
    {
        SetActiveF();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        dragStartTime = Time.time; 
        dragTime = 0f; 
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            dragTime = Time.time - dragStartTime;

            Vector3 targetPosition = eventData.position;

            if (dragTime < dragDelayTime)
            {
                float factor = Mathf.Lerp(0f, 1f, dragTime / dragDelayTime); // 
                targetPosition = Vector3.Lerp(originalPosition, targetPosition, factor * dragResistance);
                rectTransform.position = targetPosition;
            }
            else
            {
                rectTransform.position = Vector3.Lerp(rectTransform.position, eventData.position, Time.deltaTime * 100f);
                return;
            }

            
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        float distanceFromOriginalPosition = Vector3.Distance(rectTransform.position, originalPosition);

        if (distanceFromOriginalPosition > dragOutOfBoundsDistance) 
        {
            StartCoroutine(FadeOutBloodPack());
        }
        else
        {
            rectTransform.position = originalPosition;
        }
    }

    private IEnumerator FadeOutBloodPack()
    {
        float alpha = bloodPackImage.color.a;

        while (alpha > 0)
        {
            alpha -= Time.deltaTime * 2f; 
            bloodPackImage.color = new Color(1, 1, 1, alpha); 
            yield return null;
        }
         rectTransform.position = originalPosition;
         bloodPackImage.color = new Color(1, 1, 1, 1);
         gameObject.SetActive(false);
    }

    private void SetActiveF()
    {
        if(bloodPackImage.fillAmount == 0)
        {
            gameObject.SetActive(false);
        }
    }
}
