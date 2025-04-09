using UnityEngine;
using UnityEngine.UIElements;

public class MangaPlaySys : MonoBehaviour
{

    public Animator myAnimator;
    public float Cd = 0.8f;
    public float CdCounting = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CdCounting -= Time.deltaTime;
    }

    public void LoadNextAnimation()
    {
        if (CdCounting < 0)
        {
            CdCounting = Cd;
            //Play next ani
            myAnimator.SetTrigger("nextAni");
        }
        else
        {
            //Fin now Ani ?
            
        }
    }
}
