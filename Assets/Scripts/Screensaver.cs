//Matt's screensaver thingy

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screensaver : MonoBehaviour
{
    public GameObject primaryCamera;
    public GameObject secondaryCamera;

    public float idleInterval = 5;
    public float idleClock;
    // Start is called before the first frame update
    void Start()
    {
        idleClock = idleInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if(idleClock > 0)
        {
            idleClock -= Time.deltaTime;

            primaryCamera.SetActive(true);
            secondaryCamera.SetActive(false);
        }
        else
        {
            primaryCamera.SetActive(false);
            secondaryCamera.SetActive(true);
        }

        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            idleClock = idleInterval;
    }
}
