﻿//Matt's screensaver thingy

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screensaver : MonoBehaviour
{
    public GameObject primaryCamera;
    public GameObject secondaryCamera;

    public float screenSwitchInterval = 60;
    public float screenSwitchClock = 0;

    private Vector3 mouseDelta;

    // Update is called once per frame
    void Update()
    {
        if (screenSwitchClock > 0)
        {
            screenSwitchClock -= Time.deltaTime;
        }
        else
        {
            primaryCamera.SetActive(!primaryCamera.activeSelf);
            secondaryCamera.SetActive(!primaryCamera.activeSelf);
            screenSwitchClock = screenSwitchInterval;
        }

        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            screenSwitchClock = screenSwitchInterval;
        
        if (mouseDelta != Input.mousePosition)
        {
            screenSwitchClock = screenSwitchInterval;
            mouseDelta = Input.mousePosition;
        }
    }

    public void Switch()
    {
        screenSwitchClock = 0;
    }
}
