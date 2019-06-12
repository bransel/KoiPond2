using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mute : MonoBehaviour
{
    public AudioListener[] audioListeners;

	public bool MuteAudio
	{
		set
		{ 
			foreach (var audioListener in audioListeners)
				audioListener.enabled = !value;
		}
	}
}
