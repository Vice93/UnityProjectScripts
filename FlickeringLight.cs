using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour {

    public GameObject Lights;
    public float[] onRange;
    public float[] offRange;
    private float timer;

    void Start()
    {
        StartCoroutine(FlickeringLights());
    }

    IEnumerator FlickeringLights()
    {
        Lights.SetActive(true);
        timer = Random.Range(onRange[0], onRange[1]);
        yield return new WaitForSeconds(timer);
        Lights.SetActive(false);
        timer = Random.Range(offRange[0], offRange[1]);
        yield return new WaitForSeconds(timer);
        StartCoroutine(FlickeringLights());
    }
}
