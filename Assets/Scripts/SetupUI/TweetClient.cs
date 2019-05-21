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

public class TweetClient : MonoBehaviour
{
	// Note: The following access keys & secrets are for debug ONLY. Please change to a production account for live use.
	private static string CONSUMER_KEY = "7kzahJTRFmHmWKU6BEba8hysa";
	private static string CONSUMER_SECRET = "zIgV7JOH0HCNumsPCuCsAzG9lajzDfx26HTdBgrfMWcLk1kTjD";
	private static string ACCESS_TOKEN = "1130142920147738626-mzce2SyeSm1QMhdtUgQ6z5BO0H1LYt";
	private static string ACCESS_TOKEN_SECRET = "pes1cVccQ2J2lflEXBPtbTWGW8myZY3mNxQhRpYQfNEnK";

	void Start()
	{
		DontDestroyOnLoad(gameObject);

		InitTweetClient();

		foreach (var item in GetTimelineTweets())
		{
			print(string.Format("{0}: {1}", item.CreatedBy, item.Text));
		}
	}
	
	public void InitTweetClient()
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

	public IEnumerable<ITweet> GetTimelineTweets(int maximumTweets = 40)
	{
		var homeTimelineParameter = new HomeTimelineParameters
		{
			MaximumNumberOfTweetsToRetrieve = maximumTweets,
			IncludeContributorDetails = true
			// ... setup additional parameters
		};
		
		return Timeline.GetHomeTimeline(homeTimelineParameter);
	}
}