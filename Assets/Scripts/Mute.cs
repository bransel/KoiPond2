using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mute : MonoBehaviour
{
    public AudioSource[] audioSources;

	void Start()
	{
		audioSources = FindObjectsOfType<AudioSource>();
	}

	public bool MuteAudio
	{
		set
		{ 
			foreach (var audioSource in audioSources)
				audioSource.enabled = !value;
		}
	}
}
