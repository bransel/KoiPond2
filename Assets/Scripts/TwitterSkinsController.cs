using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
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
        UnityEvent mentionsEvent = new UnityEvent();
        mentionsEvent.AddListener(delegate {
            ProcessMentionsTweets(tweetClient.GetMentionsTweets());
        });
        StartCoroutine(tweetClient.RetrieveMentionsTweets(mentionsEvent));

        UnityEvent userEvent = new UnityEvent();
        userEvent.AddListener(delegate {
            ProcessUserTweets(tweetClient.GetUserTweets());
        });
        StartCoroutine(tweetClient.RetrieveUserTweets(userEvent));
    }

    public Texture[] mentionsTextures;
    void ProcessMentionsTweets(List<Tweet> tweets)
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
                //fishData.message = tweet.text;

                fishDataList.Add(fishData);
            }
        }
    }

    void ProcessUserTweets(List<Tweet> tweets)
    {
        foreach (var tweet in tweets)
        {
            if (!fishDataList.Select(i => i.id).Contains(tweet.id))
            {
                TwitterFishData fishData = new TwitterFishData();
                fishData.id = tweet.id;
                fishData.textureURL = tweet.extended_entities.media[0].media_url_https;

                string link = tweet.text.Split(' ').Last();
                fishData.message = tweet.text.Replace(string.Format(" {0}", link), "");
                //fishData.message = tweet.text;

                StartCoroutine(FinishTextureFish(fishData, fishData.textureURL));
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
}

[Serializable]
public class TwitterFishData
{
    public long id;
    public string message;
    public string textureURL;
    public Texture texture;
}
