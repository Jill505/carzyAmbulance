using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class A_sampleCore : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Text showText;
    public string[] hanas;
    public int loadingTextNum = 0;
    public int hanasNum;
    Coroutine coroutine;

    [Range(0.01f, 1.5f)] public float wordSpeed = 0.1f;

    public bool isClog = false;

    // Start is called before the first frame update
    void Start()
    {
        loadingTextNum = -1;
        hanasNum = hanas.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!isClog)
            {
                if (loadingTextNum + 1 < hanasNum)
                {
                    loadingTextNum++;
                    coroutine = StartCoroutine(loadTexter(hanas[loadingTextNum]));
                }
                else
                {
                    Debug.Log("�W�X���");
                }
            }
            else
            {
                Debug.Log("�����������");
                StopCoroutine(coroutine);
                showText.text = hanas[loadingTextNum];
                isClog = false;
            }
        }
    }
    IEnumerator loadTexter(string theText)
    {
        isClog = true;

        string swapStr = "";
        int loopTimes = theText.Length;
        for (int i = 0; i < loopTimes; i++)
        {
            swapStr += theText[i];
            showText.text = swapStr;
            yield return new WaitForSeconds(wordSpeed);
        }
        Debug.Log("��ܵ���");

        isClog = false;
    }
}
