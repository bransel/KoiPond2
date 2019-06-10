using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mute : MonoBehaviour
{
    public AudioListener audioListener;

	public bool MuteAudio
	{
		set { audioListener.enabled = !value; }
	}
}
