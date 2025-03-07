using System.Collections;
using UnityEngine;

public class SyringSystem : MonoBehaviour
{
    private GameObject medicine;    
    private bool isHoldingMedicine = false; 
    private bool usingMedicine = false;
    public bool allMedicineUsed = false;
    private Vector2 mousePosition;
    private GameCore gameCore;

    [SerializeField] private GameObject syringeObject; 
    [SerializeField] private Sprite stringSprite;  
    [SerializeField] private Sprite string2Sprite;
    private SpriteRenderer syringeRenderer;

    void Start()
    {
        gameCore = GameObject.Find("GameCore").GetComponent<GameCore>(); 
        medicine = GameObject.FindWithTag("Medicine"); 
        if (syringeObject != null)
        {
            syringeRenderer = syringeObject.GetComponent<SpriteRenderer>();
            syringeRenderer.sprite = stringSprite; 
        }
    }

    void Update()
    {
        HoldMedicine();
        ChangeSyring();
        CheckAllMedicineUsed();
    }

    void HoldMedicine()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0) && !usingMedicine)
        {
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero);

            GameObject closestMedicine = null;
            float closestDistance = float.MaxValue;

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.CompareTag("Medicine"))
                {
                    float distance = Vector2.Distance(mousePosition, hit.collider.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestMedicine = hit.collider.gameObject;
                    }
                }
            }

            if (closestMedicine != null)
            {
                medicine = closestMedicine;
                isHoldingMedicine = true;
            }
        }
        

        if (isHoldingMedicine && medicine != null)
        {
            medicine.transform.position = mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isHoldingMedicine)
            {
                usingMedicine = true;
                isHoldingMedicine = false;
                StartCoroutine(UseMedicine());
                medicine.SetActive(false);
            }
        }
    }

    IEnumerator UseMedicine()
    {
        gameCore.Hocus();
        yield return new WaitForSeconds(gameCore.hocusTime -2.3f);
        usingMedicine = false;
    }

    void ChangeSyring()
    {
        if (usingMedicine == false)
        {
            syringeRenderer.sprite = stringSprite;
        }
        else
        {
            syringeRenderer.sprite = string2Sprite;
        }
    }
    public void CheckAllMedicineUsed()
    {
        GameObject[] medicines = GameObject.FindGameObjectsWithTag("Medicine");

        bool allUsed = true;
        foreach (GameObject med in medicines)
        {
            if (med.activeSelf) 
            {
                allUsed = false;
                break;
            }
        }

        allMedicineUsed = allUsed; 
    }
}
