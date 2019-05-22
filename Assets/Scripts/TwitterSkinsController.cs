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

    void ProcessMentionsTweets(List<Tweet> tweets)
    {
        foreach (var tweet in tweets)
        {
            if (!fishDataList.Select(i => i.id).Contains(tweet.id))
            {
                TwitterFishData fishData = new TwitterFishData();
                fishData.id = tweet.id;
                fishData.message = tweet.text;
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
                fishData.textureURL = tweet.extended_entities.media[0].media_url;
                fishData.message = tweet.text;
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

    /* IEnumerator RefreshTweets()
    {
        foreach (var item in collection)
        {
            
        }
        
        foreach (var guid in gcpTextureJSON.guids)
        {
            string textureURN = string.Format("Data/{0}/texture.png", guid);
            string messageURN = string.Format("Data/{0}/message.txt", guid);

            FishData fishData = new FishData();
            fishData.guid = guid;
            
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(URI + textureURN))
            {
                yield return uwr.SendWebRequest();

                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    Debug.Log(uwr.error);
                }
                else
                {
                    Texture texture = DownloadHandlerTexture.GetContent(uwr);

                    buttonElement.AssignTexture(texture);
                    fishData.texture = texture;
                }
            }

            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(URI + messageURN))
            {
                yield return uwr.SendWebRequest();

                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    Debug.Log(uwr.error);
                }
                else
                {
                    string message = uwr.downloadHandler.text;

                    buttonElement.AssignMessage(message);
                    fishData.message = message;
                }
            }

            fishDataList.Add(fishData);
        }

        finishedDL = true;
    } */
}

[Serializable]
public class TwitterFishData
{
    public long id;
    public string message;
    public string textureURL;
    public Texture texture;
}
