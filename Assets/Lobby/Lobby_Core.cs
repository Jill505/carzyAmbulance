using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
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

    public TextMeshProUGUI pNameTmp;
    public TextMeshProUGUI pDesTmp;
    public Button buttonMi;
    public Button buttonPl;

    public GameInfo[] gameInfo = new GameInfo[6]; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        loadingSort = 0;
        pNameTmp.text = gameInfo[loadingSort].name;
        pDesTmp.text = gameInfo[loadingSort].name;
        buttonMi.interactable = false;
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
        Debug.Log("�C���h�X");
        Application.Quit();
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(loadingSort+1);
    }
    public void loadSortPlus()
    {
        if (loadingSort+1 < gameInfo.Length)
        {
            //allow 
            buttonPl.interactable = true;
        }
        else
        {
            buttonPl.interactable = false;
        }
        buttonMi.interactable = true;
        loadingSort++;
        loadingSceneInformation(loadingSort);
    }
    public void loadSortMinus()
    {
        if (loadingSort-1 > 0)
        {
            //allow
            buttonMi.interactable = true;
        }
        else
        {
            buttonMi.interactable = false;
        }
        buttonPl.interactable = true;
        loadingSort--;
        loadingSceneInformation(loadingSort);
    }
    public void loadingSceneInformation(int sort)
    {
        pNameTmp.text = gameInfo[loadingSort].name;
        pDesTmp.text = gameInfo[loadingSort].name;
    }
}


[System.Serializable]
public class GameInfo
{
    public string name;
    public string description;
}