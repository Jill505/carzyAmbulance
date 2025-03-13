using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class Lobby_Core : MonoBehaviour
{
    public GameObject selectionCanvas;
    bool isSelectionCanvasOpening = false;

    public GameObject selectionCanvas2;
    
    public Scene[] gameScene = new Scene[10];

    public string pName;
    public string pDes;
    public int loadingSort = 0;
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
        selectionCanvas2.SetActive(isSelectionCanvasOpening);
    }

    public void QuitGame()
    {
        Debug.Log("¹CÀ¸°h¥X");
        Application.Quit();
    }

    public void LoadGame(int sort)
    {
        SceneManager.LoadScene(sort);
    }
    public void loadSortPlus()
    {
        if (loadingSort++ < 6)
        {
             //allow 
        }
        loadingSort++;
    }
    public void loadSortMinus()
    {

    }
    public void loadingSceneInformation()
    {
        
    }
}


[System.Serializable]
public class GameInfo
{
    public string name;
    public string description;
}