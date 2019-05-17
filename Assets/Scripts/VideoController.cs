using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
	private VideoPlayer videoPlayer;

	void Start()
	{
		DontDestroyOnLoad(gameObject);
		
		videoPlayer = GetComponent<VideoPlayer>();
		videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "Underwater.mp4");
		videoPlayer.Prepare();
	}

	public void Play()
	{
		videoPlayer.Play();
	}
}
