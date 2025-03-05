using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableBloodPack : MonoBehaviour,  IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 startPosition;
    public Image activeSlot; 
    public BloodPackDrag bloodPackDrag;
    public GameCore gameCore;

    private bool isDragging = false; 


    public void OnBeginDrag(PointerEventData eventData)
    {
        /*if (bloodPackDrag.gameObject.activeSelf)
        {
            return; 
        }*/

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
    }

    public void ChangeBloodPack()
    {
        transform.position = startPosition;
        //bloodPackDrag.gameObject.SetActive(true);
        gameCore.ChangeBloodPack();
    }
}
