using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableBloodPack : MonoBehaviour, IPointerClickHandler //, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //private Vector3 startPosition;
    public Image activeSlot; 
    public BloodPackDrag bloodPackDrag;
    public GameCore gameCore;

    //private bool isDragging = false; 


    /*public void OnBeginDrag(PointerEventData eventData)
    {
        if (bloodPackDrag.gameObject.activeSelf)
        {
            return; 
        }

        isDragging = true;
        startPosition = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return; 
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return; 

        ChangeBloodPack();
        isDragging = false; 
    }*/

    public void OnPointerClick(PointerEventData eventData)
    {
        ChangeBloodPack();
    }

    public void ChangeBloodPack()
    {
        /*RectTransform slotRect = activeSlot.GetComponent<RectTransform>();
        
        if (RectTransformUtility.RectangleContainsScreenPoint(slotRect, Input.mousePosition))
        {
            transform.position = startPosition;
            bloodPackDrag.gameObject.SetActive(true);
            gameCore.ChangeBloodPack();
        }
        else
        {
            transform.position = startPosition;
        }*/
            gameCore.ChangeBloodPack();
    }
}
