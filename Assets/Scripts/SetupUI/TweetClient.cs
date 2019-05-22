using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.Events;

public class TweetClient : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        //InitTweetClient();

        /* foreach (var item in GetUserTweets())
		{
			print(string.Format("{0}: {1}", item.CreatedBy, item.Text));
			

			foreach (var media in item.Media)
			{
				print(string.Format("{0}: {1}", media.MediaType, media.MediaURL));
			}
		} */
    }

    public void TweetMessage(string message)
    {
        //Tweet.PublishTweet(string.Format("{0}", message));
    }

    public void TweetMessageAndImage(string message, byte[] image)
    {
        //var media = Upload.UploadBinary(image);

        //var tweet = Tweet.PublishTweet(message, new PublishTweetOptionalParameters
        //{
        //	Medias = new List<IMedia> { media }
        //});
    }

    List<Tweet> userTweets;
    public List<Tweet> GetUserTweets()
    {
        return userTweets;
    }

    public IEnumerator RetrieveUserTweets(UnityEvent userEvent)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("https://asia-east2-unity-koi.cloudfunctions.net/user-timeline"))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log("Error: " + webRequest.error);
            }
            else
            {
                print(webRequest.downloadHandler.text);
                userTweets = JsonConvert.DeserializeObject<List<Tweet>>(webRequest.downloadHandler.text);
                userEvent.Invoke();
            }
        }
    }

    List<Tweet> mentionsTweets;
    public List<Tweet> GetMentionsTweets()
    {
        return mentionsTweets;
    }

    public IEnumerator RetrieveMentionsTweets(UnityEvent mentionsEvent)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("https://asia-east2-unity-koi.cloudfunctions.net/mentions-timeline"))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log("Error: " + webRequest.error);
            }
            else
            {
                print(webRequest.downloadHandler.text);
                mentionsTweets = JsonConvert.DeserializeObject<List<Tweet>>(webRequest.downloadHandler.text);
                mentionsEvent.Invoke();
            }
        }
    }
}

[Serializable]
public class Tweet
{
    public long id;
    public string text;
    public Extended_Entities extended_entities;
}

[Serializable]
public class Extended_Entities
{
    public Media[] media;
}

[Serializable]
public class Media
{
    public long id;
    public string media_url;
    public string media_url_https;
    public string type;
}