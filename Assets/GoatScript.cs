using UnityEngine;
using System.Collections;

public class GoatScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        carShake();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Header("shake ¨t²Î")]
    public GameObject allObjectFather;
    public float shakeStrength = 5f;
    public float shakeInterval = 0.1f;
    Coroutine theShakeCoroutine;

    public void carShake()
    {
        if (theShakeCoroutine != null)
        {
            StopCoroutine(theShakeCoroutine);
        }
        theShakeCoroutine = StartCoroutine(shakeCoroutine());
    }

    IEnumerator shakeCoroutine()
    {
        float swapX = 0;
        float swapY = 0;

        //float defultVelocity = 1f;
        float defultTime = 1000f;

        float timePast = 0;

        WaitForSeconds sec = new WaitForSeconds(shakeInterval);

        while (defultTime > 0)
        {
            timePast += Time.deltaTime;

            swapX = Mathf.Sin(timePast) * Random.Range(-1f, 1f) * shakeStrength;
            swapY = Mathf.Cos(timePast) * Random.Range(-1f, 1f) * shakeStrength;

            allObjectFather.transform.position = gameObject.transform.position + new Vector3(swapX, swapY, 0);

            defultTime -= Time.deltaTime;
            yield return sec;
        }

        yield return null;
        allObjectFather.transform.position = gameObject.transform.position +  new Vector3(0, 0, 0);
    }

}
