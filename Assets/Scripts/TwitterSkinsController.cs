﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Events;

public class TwitterSkinsController : MonoBehaviour
{
    public float refreshTimerInSeconds = 150;
    public FishController fishController;

    private TweetClient tweetClient;
    private List<TwitterFishData> fishDataList = new List<TwitterFishData>();
    private float timerTime;
	private ProfanityClass profanityClass;
    
    // Start is called before the first frame update
    /* void Start()
    {
        tweetClient = FindObjectOfType<TweetClient>();

        if (!tweetClient)
        {
            GameObject tweetClientObject = new GameObject("TweetClient");
            tweetClient = tweetClientObject.AddComponent<TweetClient>();
        }

        fishController.LinkFishDataList(fishDataList);

        RefreshTweets();
    } */
    IEnumerator Start()
    {
        profanityClass = new ProfanityClass();
		tweetClient = FindObjectOfType<TweetClient>();

        if (!tweetClient)
        {
            GameObject tweetClientObject = new GameObject("TweetClient");
            tweetClient = tweetClientObject.AddComponent<TweetClient>();
        }

        fishController.LinkFishDataList(fishDataList);

        WaitForSeconds waitTimer = new WaitForSeconds(refreshTimerInSeconds);
        timerTime = refreshTimerInSeconds;
        
        while (refreshTimerInSeconds > 0)
        {
            RefreshTweets();

            if (timerTime != refreshTimerInSeconds)
            {
                waitTimer = new WaitForSeconds(refreshTimerInSeconds);
                timerTime = refreshTimerInSeconds;
            }
            
            yield return waitTimer;
        }
    }

    void RefreshTweets()
    {
        fishDataList.Clear();
        
        /* UnityEvent mentionsEvent = new UnityEvent();
        mentionsEvent.AddListener(delegate {
            ProcessMentionsTweets(tweetClient.GetMentionsTweets());
        });
        StartCoroutine(tweetClient.RetrieveMentionsTweets(mentionsEvent)); */

        UnityEvent userEvent = new UnityEvent();
        userEvent.AddListener(delegate {
            ProcessUserTweets(tweetClient.GetUserTweets());
        });
        StartCoroutine(tweetClient.RetrieveUserTweets(userEvent));
    }

    public Texture[] mentionsTextures;
    /* void ProcessMentionsTweets(List<Tweet> tweets)
    {
        foreach (var tweet in tweets)
        {
            if (!fishDataList.Select(i => i.id).Contains(tweet.id))
            {
                TwitterFishData fishData = new TwitterFishData();
                fishData.id = tweet.id;
                fishData.texture = mentionsTextures[UnityEngine.Random.Range(0, mentionsTextures.Length)];

                string link = tweet.text.Split(' ')[0];
                fishData.message = tweet.text.Replace(string.Format("{0} ", link), "");
                // Use tweet.user.screen_name to get the twitter handler from mentions tweets.
                fishData.message += string.Format("\r\n- @{0}", tweet.user.screen_name);
                fishData.message = WebUtility.HtmlDecode(fishData.message);

                if (profanityClass.IsContentProfane(fishData.message))
				{
					print("RUDE! This fish wanted to say: " + string.Join(", ", profanityClass.GetProfanity(fishData.message)));
					print("Original Message: " + fishData.message);
				}
				else
					fishDataList.Add(fishData);
            }
        }
    } */

    void ProcessUserTweets(List<Tweet> tweets)
    {
        foreach (var tweet in tweets)
        {
			if (!fishDataList.Select(i => i.id).Contains(tweet.id))
            {
                TwitterFishData fishData = new TwitterFishData();
                fishData.id = tweet.id;

                if (tweet.extended_entities == null || tweet.extended_entities.media.Length == 0)
                {
                    if (tweet.entities.user_mentions.Length < 2)
                        continue;
                    
                    fishData.texture = mentionsTextures[UnityEngine.Random.Range(0, mentionsTextures.Length)];

                    string[] tags = tweet.text.Split(' ');
                    fishData.message = tweet.text.Replace(string.Format("{0} ", tags[0]), "");
                    fishData.message = fishData.message.Replace(string.Format("{0} ", tags[1]), "");
                    // Use tweet.user.screen_name to get the twitter handler from mentions tweets.
                    fishData.message += string.Format("\r\n- @{0}", tweet.entities.user_mentions[0].screen_name);
                    fishData.message = WebUtility.HtmlDecode(fishData.message);

                    if (profanityClass.IsContentProfane(fishData.message))
                    {
                        print("RUDE! This fish wanted to say: " + string.Join(", ", profanityClass.GetProfanity(fishData.message)));
                        print("Original Message: " + fishData.message);
                    }
                    else
                        fishDataList.Add(fishData);
                }
                else
                {
                    fishData.textureURL = tweet.extended_entities.media[0].media_url_https;

                    string link = tweet.text.Split(' ').Last();
                    fishData.message = tweet.text.Replace(string.Format(" {0}", link), "");
                    //fishData.message = tweet.text;
                    fishData.message = WebUtility.HtmlDecode(fishData.message);

                    if (profanityClass.IsContentProfane(fishData.message))
                    {
                        print("RUDE! This fish wanted to say: " + string.Join(", ", profanityClass.GetProfanity(fishData.message)));
                        print("Original Message: " + fishData.message);
                    }
                    else
                        StartCoroutine(FinishTextureFish(fishData, fishData.textureURL));
                }
            }
        }
    }

    IEnumerator FinishTextureFish(TwitterFishData fishData, string textureURL)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(textureURL))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                Texture texture = DownloadHandlerTexture.GetContent(uwr);
                fishData.texture = texture;
                fishDataList.Add(fishData);
            }
        }
    }

    public string ProcessSpecialCharacters(string value)
    {
        return WebUtility.HtmlDecode(value);
    }
}

[Serializable]
public class TwitterFishData
{
    public long id;
    public string message;
    public string textureURL;
    public Texture texture;
}
