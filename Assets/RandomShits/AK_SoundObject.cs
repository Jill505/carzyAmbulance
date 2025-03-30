using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class AK_SoundObject : MonoBehaviour
{
    public float playTime = 10f;
    public int type = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, playTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void syncVoulme()
    {
        //進行音量同步
    }

    static public void PlaySoundObject(AudioClip myAC)
    {
        GameObject soundObject = new GameObject("Sound Object " + myAC.name);
        AudioSource myAS =  soundObject.AddComponent<AudioSource>();
        myAS.clip = myAC;
        myAS.Play();

        myAS.volume = AK_SoundMaster.AK_SoundMaster_All_Volume;
    }
}
