using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class Lobby_Core : MonoBehaviour
{
    [Header ("音效")]
    public AudioClip carHorn;
    public AudioClip attention;
    public AudioClip changeScene;
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
    public Animator Loading;

    public GameInfo[] gameInfo = new GameInfo[6]; 

    

    
    void Start()
    {
        loadingSort = 0;
        pNameTmp.text = gameInfo[loadingSort].name;
        pDesTmp.text = gameInfo[loadingSort].name;
        buttonMi.interactable = false;
        AK_SoundObject.PlaySoundObject(carHorn);
        swapMusicPlayer();
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
        StartCoroutine(LoadingAnimator());
    }
    IEnumerator LoadingAnimator()
    {
        AK_SoundObject.PlaySoundObject(changeScene);
        Loading.SetTrigger("LoadOut");
        stopSwapMusicPLayer();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(loadingSort+1);
    }
    public void loadSortPlus()
    {
        if (loadingSort+1 < gameInfo.Length-1)
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

    [Header("BGM and SoundEffects")]
    public AudioClip bgmClip;
    GameObject theBgmPlayer;
    public void swapMusicPlayer()
    {
        theBgmPlayer = new GameObject("BgmPlayer");
        AudioSource AS =  theBgmPlayer.AddComponent<AudioSource>();
        AS.clip = bgmClip;
        AS.loop = true;
        AS.Play();
    }
    public void stopSwapMusicPLayer()
    {
        Destroy(theBgmPlayer);
    }
}


[System.Serializable]
public class GameInfo
{
    public string name;
    public string description;
}