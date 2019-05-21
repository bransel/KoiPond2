using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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
            tweetClient.InitTweetClient();
        }

        fishController.LinkFishDataList(fishDataList);

        WaitForSeconds waitTimer = new WaitForSeconds(refreshTimerInSeconds);
        timerTime = refreshTimerInSeconds;
        
        while (refreshTimerInSeconds > 0)
        {
            RefreshTweets();
            //StartCoroutine(RefreshTweets());

            if (timerTime != refreshTimerInSeconds)
            {
                waitTimer = new WaitForSeconds(refreshTimerInSeconds);
                timerTime = refreshTimerInSeconds;
            }
            
            yield return waitTimer;
        }
    }

    public void RefreshTweets()
    {
        foreach (var tweet in tweetClient.GetMentionsTweets())
        {
            if (!fishDataList.Select(i => i.id).Contains(tweet.Id))
            {
                TwitterFishData fishData = new TwitterFishData();
                fishData.id = tweet.Id;
                fishData.message = tweet.Text;
                fishDataList.Add(fishData);
            }
        }
        
        foreach (var tweet in tweetClient.GetUserTweets())
        {
            if (!fishDataList.Select(i => i.id).Contains(tweet.Id))
            {
                TwitterFishData fishData = new TwitterFishData();
                fishData.id = tweet.Id;
                fishData.textureURL = tweet.Media[0].MediaURL;
                fishData.message = tweet.Text;
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
