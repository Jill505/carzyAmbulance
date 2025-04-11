using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MangaPlaySys : MonoBehaviour
{

    public Animator myAnimator;
    public float Cd = 0.8f;
    public float CdCounting = 0f;

    static public bool spec_pre = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1f;


        CdCounting = Cd;
        if (spec_pre)
        {
            //anyway
        }
    }

    // Update is called once per frame
    void Update()
    {
        CdCounting -= Time.deltaTime;
    }

    public void LoadNextAnimation()
    {
        Debug.Log("pressed");
        if (CdCounting < 0)
        {
            Debug.Log("Load next ani");
            CdCounting = Cd;
            //Play next ani
            myAnimator.SetTrigger("nextAni");
        }
        else
        {
            //Fin now Ani ?
            
        }
    }
    public void backToLobby()
    {
        SceneManager.LoadScene(0);
    }
}
