using System.Security.Cryptography;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BRandomShit : MonoBehaviour
{
    public float fixedJudgementDur = 0.5f;
    public float bpm = 120;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        setUpBPMToDurition(bpm);
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void ChaosFunction_RandomA()
    {
        if (gameObject.transform.position.x > 3)
        {
            gameObject.transform.localPosition = new Vector3(gameObject.transform.position.x*-1, gameObject.transform.position.y, gameObject.transform.position.z);
        }
    }

    void FixedUpdate()
    {
        
    }

    public void setUpBPMToDurition(float bpm)
    {
        fixedJudgementDur = 60 / bpm;
    }
}
