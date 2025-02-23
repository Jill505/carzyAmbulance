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
}
