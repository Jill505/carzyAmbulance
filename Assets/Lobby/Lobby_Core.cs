using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class Lobby_Core : MonoBehaviour
{
    public GameObject selectionCanvas;
    bool isSelectionCanvasOpening = false;

    public Scene[] gameScene = new Scene[10];
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