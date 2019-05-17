using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

public class TweetClient
{
	// Note: The following access keys & secrets are for debug ONLY. Please change to a production account for live use.
	private static string CONSUMER_KEY = "1xopSCf75Vpib1hEajqPy00Ga";
	private static string CONSUMER_SECRET = "vi3V1HuTFlUotm5hIp67UXQzORyMYX5WXBOlDhka56YBN4QOZC";
	private static string ACCESS_TOKEN = "992795554781454336-Lnlh7rKpTyBpwcF5idacaGD9iKBgXRa";
	private static string ACCESS_TOKEN_SECRET = "YZ2ITFMjCA8h55cQ5v5O7p3ubdqT3Gn0fzTLDU1TVD300";

	public TweetClient()
	{
		Auth.SetUserCredentials(CONSUMER_KEY, CONSUMER_SECRET, ACCESS_TOKEN, ACCESS_TOKEN_SECRET);
	}

	public void TweetMessage(string message)
	{
		Tweet.PublishTweet(string.Format("{0}", message));
	}

	public void TweetMessageAndImage(string message, byte[] image)
	{
		var media = Upload.UploadBinary(image);

		var tweet = Tweet.PublishTweet(message, new PublishTweetOptionalParameters
		{
			Medias = new List<IMedia> { media }
		});
	}
}