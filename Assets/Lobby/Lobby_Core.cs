using System.Collections;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
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
    public Text r_p_text;
    public Button buttonMi;
    public Button buttonPl;
    public Animator Loading;

    public GameInfo[] gameInfo = new GameInfo[6];

    static public bool EzMode = false;

    static public GameSaveFile s_GSF;


    void Start()
    {
        loadingSort = 0;
        pNameTmp.text = gameInfo[loadingSort].name;
        r_p_text.text = gameInfo[loadingSort].description;
        buttonMi.interactable = false;
        AK_SoundObject.PlaySoundObject(carHorn);
        swapMusicPlayer();

        if (string.IsNullOrEmpty(PlayerPrefs.GetString("SaveFile")))
        {
            GameSaveFile GSF = new GameSaveFile();
            string saveFile = JsonUtility.ToJson(GSF);
            PlayerPrefs.SetString("SaveFile", saveFile);

            s_GSF = JsonUtility.FromJson<GameSaveFile>(PlayerPrefs.GetString("SaveFile"));

            Debug.Log("新建檔案");
        }
        else
        {

            s_GSF = JsonUtility.FromJson<GameSaveFile>(PlayerPrefs.GetString("SaveFile"));
            Debug.Log("成功還原檔案");
        }

        if (MangaPlaySys.spec_pre)
        {
            openAndClose();
        }
    }
    static public void SaveGameFile()
    {
        string saveFile = JsonUtility.ToJson(Lobby_Core.s_GSF);
        PlayerPrefs.SetString("SaveFile", saveFile);
        PlayerPrefs.Save();
    }

    static public void ResetAllGameFile()
    {
        Debug.Log("重置所有檔案");
        PlayerPrefs.DeleteKey("SaveFile");

        s_GSF = JsonUtility.FromJson<GameSaveFile>(PlayerPrefs.GetString("SaveFile"));

        PlayerPrefs.Save();
    }
    static public void UnlockAllGameFile()
    {
        Debug.Log("解鎖所有遊玩進度");
        GameSaveFile GSF = new GameSaveFile();

        GSF.mexUnlockGame = 6;
        s_GSF.mexUnlockGame = 6;
        for (int i = 0; i < GSF.gamePassed.Length; i++)
        {
            GSF.gamePassed[i] = true;
        }

        string saveFile = JsonUtility.ToJson(GSF);
        PlayerPrefs.SetString("SaveFile", saveFile);

        Lobby_Core.SaveGameFile();
        s_GSF = JsonUtility.FromJson<GameSaveFile>(PlayerPrefs.GetString("SaveFile"));

    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.D))
        {
            ResetAllGameFile();
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.U))
        {
            UnlockAllGameFile();
        }

        //Debug.Log("Lobby_Core.s_GSF.mexUnlockGame: "+ Lobby_Core.s_GSF.mexUnlockGame);
    }

    public void openAndClose()
    {
        isSelectionCanvasOpening = !isSelectionCanvasOpening;
        selectionCanvas2.SetActive(isSelectionCanvasOpening);

        //判定是否有看過開頭動畫
        if (s_GSF.alreadyReadOpeningManga == false)
        {
            //play
            Debug.Log("還未看過開場動畫");
            s_GSF.alreadyReadOpeningManga = true;
            MangaPlaySys.spec_pre = true;
            SaveGameFile();
        }
        else
        { 
            Debug.Log("已經看過開場動畫");
        }

        if (loadingSort + 1 < gameInfo.Length - 1 && loadingSort < Lobby_Core.s_GSF.mexUnlockGame)
        {
            buttonPl.interactable = true;
            Debug.Log("From open and close allow");
        }
        else
        {
            buttonPl.interactable = false;
            Debug.Log("From open and close not allow");
        }
    }
    public GameObject ezButton; 
    public Sprite[] EZButton = new Sprite[2];
    public void EzModeOnAndOff()
    {
        //初始為紅色
        Image buttonImage = ezButton.GetComponent<Image>();

        EzMode = !EzMode;
        if (EzMode)//圖標切換為綠色
        {
            Debug.Log("EZ模式開啟");
            buttonImage.sprite = EZButton[0]; 
        }
        else
        {
            //切換為紅色
            Debug.Log("EZ模式關閉");
            buttonImage.sprite = EZButton[1]; 
        }
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
        if (loadingSort+1 < gameInfo.Length-1 && loadingSort+1 < s_GSF.mexUnlockGame)
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
        r_p_text.text = gameInfo[loadingSort].description;
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

[System.Serializable]
public class GameSaveFile
{
    public int emptyNumber;
    public int mexUnlockGame = 0;
    public bool[] gamePassed = new bool[6];
    public bool alreadyReadOpeningManga = false;
}