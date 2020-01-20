using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Events;
using Newtonsoft.Json;

public class TwitterSkinsController : MonoBehaviour
{
    public float refreshTimerInSeconds = 150;
    public FishController fishController;

    private TweetClient tweetClient;
    private string[] blacklist;
    private List<TwitterFishData> fishDataList = new List<TwitterFishData>();
    private float timerTime;
	private ProfanityClass profanityClass;

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

        UnityEvent refreshTweetEvent = new UnityEvent();
        refreshTweetEvent.AddListener(RefreshTweets);
        
        while (refreshTimerInSeconds > 0)
        {
            RefreshBlacklist(blacklist, refreshTweetEvent);

            if (timerTime != refreshTimerInSeconds)
            {
                waitTimer = new WaitForSeconds(refreshTimerInSeconds);
                timerTime = refreshTimerInSeconds;
            }
            
            yield return waitTimer;
        }
    }

    void RefreshBlacklist(string[] blacklistRef, UnityEvent onRefresh)
    {
        StartCoroutine(RefreshBlackList(onRefresh));
    }

    void RefreshTweets()
    {
        fishDataList.Clear();

        UnityEvent userEvent = new UnityEvent();
        userEvent.AddListener(delegate {
            ProcessUserTweets(tweetClient.GetSearchTweets());
        });
        StartCoroutine(tweetClient.RetrieveSearchTweets(userEvent));
    }

    public Texture[] mentionsTextures;

    void ProcessUserTweets(List<Tweet> tweets)
    {
        foreach (var tweet in tweets)
        {
			if (!fishDataList.Select(i => i.id).Contains(tweet.id))
            {
                TwitterFishData fishData = new TwitterFishData();
                fishData.id = tweet.id;

                fishData.texture = mentionsTextures[UnityEngine.Random.Range(0, mentionsTextures.Length)];

                string[] tags = tweet.text.Split(' ');
                fishData.message = tweet.text.Replace(string.Format("{0} ", tags[0]), "");
                fishData.message = fishData.message.Replace(string.Format("{0} ", tags[1]), "");
                fishData.message = WebUtility.HtmlDecode(fishData.message);

                if (profanityClass.IsContentProfane(fishData.message))
                {
                    print("RUDE! This fish wanted to say: " + string.Join(", ", profanityClass.GetProfanity(fishData.message)));
                    print("Original Message: " + fishData.message);
                }
                else
                {
                    if (blacklist.Contains(tweet.user.screen_name))
                        continue;

                    fishDataList.Add(fishData);
                }
            }
        }
    }

    public IEnumerator RefreshBlackList(UnityEvent onRefreshed = null, UnityEvent onError = null)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("https://firestore.googleapis.com/v1/projects/pausefest-93ff3/databases/(default)/documents/twitter/NvlhgJtZM6DqVGZcwRM5"))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log("Error: " + webRequest.error);

                if (onError != null)
                    onError.Invoke();
            }
            else
            {
                blacklist = JsonConvert.DeserializeObject<FirebaseDocument>(webRequest.downloadHandler.text).Fields.Blacklist.ArrayValue.Values.Select(i => i.StringValue).ToArray();
                
                if (onRefreshed != null)
                    onRefreshed.Invoke();
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

[Serializable]
public class FirebaseDocument
{
    public FirebaseField Fields;
}

[Serializable]
public class FirebaseField
{
    public FirebaseBlacklist Blacklist;
}

[Serializable]
public class FirebaseBlacklist
{
    public FirebaseArrayValue ArrayValue;
}

[Serializable]
public class FirebaseArrayValue
{
    public FirebaseValue[] Values;
}

[Serializable]
public class FirebaseValue
{
    public string StringValue;
}