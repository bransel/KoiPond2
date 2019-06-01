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
    }

    public void TweetMessage(string message, UnityEvent onTweetFinish = null, UnityEvent onError = null)
    {
        StartCoroutine(SendTweet(message, onTweetFinish, onError));
    }

    IEnumerator SendTweet(string message, UnityEvent onTweetFinish = null, UnityEvent onError = null)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post("https://asia-east2-unity-koi.cloudfunctions.net/tweet", message))
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
                if (onTweetFinish != null)
                    onTweetFinish.Invoke();
            }
        }
    }

    public void TweetMessageAndImage(string message, byte[] image, UnityEvent onTweetFinish = null, UnityEvent onError = null)
    {
        StartCoroutine(SendTweetAndImage(message, image, onTweetFinish, onError));
    }

    IEnumerator SendTweetAndImage(string message, byte[] image, UnityEvent onTweetFinish = null, UnityEvent onError = null)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", image, "texture.png", "image/png");
        string guid = System.Guid.NewGuid().ToString();
        string url = string.Format("https://storage.googleapis.com/unity-koi-bucket/Twitter/{0}.png", guid);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log("Error: " + webRequest.error);

                if (onError != null)
                    onError.Invoke();

                yield break;
            }
        }

        form = new WWWForm();
        form.AddField("text", message);
        form.AddField("url", url);

        using (UnityWebRequest webRequest = UnityWebRequest.Post("https://asia-east2-unity-koi.cloudfunctions.net/media", form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log("Error: " + webRequest.error);

                if (onError != null)
                    onError.Invoke();
                    
                yield break;
            }
            else
            {
                if (onTweetFinish != null)
                    onTweetFinish.Invoke();
            }
        }
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
    public User user;
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

[Serializable]
public class User
{
    public string screen_name;
}