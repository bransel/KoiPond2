/*
 * Matt Cabanag, DJ track switching thingy...
 * 
 * Assumes sequential order
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicFader : MonoBehaviour
{
    [Header("Turntable Audiosources")]    
    public AudioSource trackA;
    public AudioSource trackB;

    [Header("Track Parameters")]
    public int trackIndex;
    public AudioClip [] tracklist;

    [Header("Fade Settings")]    
    public float fadeInterval = 2;
    public float fadeRate = 0.5f;
    public float fadeTimeTrigger = 2;//remaining time on current track for when fading is triggered;
    public float maxAttenuation = 0;
    public float minAttenuation = -80;
    public float trackAVolume;
    public float trackBVolume;

    [Header("System Stuff")]
    public float fadeClock = 0;
    public AudioSource currentPrimary;
    public AudioSource currentSecondary;
    public float currentTime;
    public bool fadeSwitch = false;
    public AudioClip primaryClip;
    public AudioClip secondaryClip;

    // Start is called before the first frame update
    void Start()
    {
        AssignCurrentPrimary();
        currentPrimary.clip = tracklist[trackIndex];
        currentPrimary.Play();

        currentSecondary.volume = 0;
        currentSecondary.clip = tracklist[trackIndex + 1];
    }

    // Update is called once per frame
    void Update()
    {
        currentTime = currentPrimary.time;

        if (currentPrimary.clip.length - currentPrimary.time <= fadeTimeTrigger && !fadeSwitch)
        {

            if (trackIndex + 1 < tracklist.Length)
                trackIndex++;
            else
            {
                trackIndex = 0;//loop through list
            }

            fadeSwitch = true;
        }

        HandleSwitching();
        HandleFading();

    }

    void HandleSwitching()
    {
        if(fadeSwitch)
        {
            fadeSwitch = false;
            AssignCurrentPrimary();
            fadeClock = fadeInterval;

            if(!currentPrimary.isPlaying)
                currentPrimary.Play();

            if (!currentSecondary.isPlaying)
                currentSecondary.Play();
        }
    }

    void HandleFading()
    {
        if (fadeClock > 0)
        {
            fadeClock -= Time.deltaTime;

            currentPrimary.volume += fadeRate * Time.deltaTime;
            currentSecondary.volume -= fadeRate * Time.deltaTime;
        }
        else
        {
            currentPrimary.volume = 1;
            currentSecondary.volume = 0;
            currentSecondary.Stop();
            fadeClock = 0;

            //prepare next clip
            if (trackIndex + 1 < tracklist.Length)
                currentSecondary.clip = tracklist[trackIndex + 1];
            else
                currentSecondary.clip = tracklist[0];//loop through list

            primaryClip = currentPrimary.clip;
            secondaryClip = currentSecondary.clip;

        }
    }

    public void AssignCurrentPrimary()
    {
        if (trackIndex % 2 == 0)
        {
            currentPrimary = trackA;
            currentSecondary = trackB;
        }
        else
        {
            currentPrimary = trackB;
            currentSecondary = trackA;
        }
    }
}
