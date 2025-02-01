using UnityEngine;
using UnityEngine.UI; 

public class Lobby_Core : MonoBehaviour
{
    public GameObject selectionCanvas;
    bool isSelectionCanvasOpening = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void openAndClose()
    {
        isSelectionCanvasOpening = !isSelectionCanvasOpening;
        selectionCanvas.SetActive(isSelectionCanvasOpening);
    }

    public void QuitGame()
    {
        Debug.Log("¹CÀ¸°h¥X");
        Application.Quit();
    }
}