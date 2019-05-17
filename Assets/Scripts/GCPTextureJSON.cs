using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// References:
// https://cloud.google.com/storage/docs/json_api/v1/objects/list
// https://docs.unity3d.com/Manual/JSONSerialization.html
// https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.Get.html
public class GCPTextureJSON : MonoBehaviour
{
    public string uri = "https://www.googleapis.com/storage/v1/b/unity-koi-bucket/o";
    public string prefix = "?prefix=Data%2F";

	public string clientID = "823765277171-h2djqlq1hp86ctc0p6sp39kuo2pg4ufd.apps.googleusercontent.com";
	public string clientSecret = "W0LM3lyEdSGUdxtHtF_SHulf";

    public List<string> guids { get; private set; }

    public bool authorised { get; private set; }
    public bool exchanged { get; private set; }
    public bool refreshed { get; private set; }

	public string authorisationCode { get; private set; }
	public string accessToken { get; private set; }
	public string refreshToken = "1/NySkiGFB0bY98Zct1tPY89gPvoPL3Hn5m0AGoFITiUg6Ma7BK2kIY9DMnMCD0aw2";

    public bool initialised { get; private set; }

    private IEnumerator authoriseCoro;
    private IEnumerator refreshCoro;

	public JWT gcpJWT;

	public void Initialise()
    {
        StartCoroutine(InitialiseCoro());
    }

    IEnumerator InitialiseCoro()
    {
        while (!authorised)
        {
            if (authoriseCoro == null)
                Authorise();

            yield return null;
        }
        
        StartCoroutine(GetTextureList(uri + prefix));
    }

	public void Authorise()
    {
        authoriseCoro = AuthoriseCoro();
        StartCoroutine(authoriseCoro);
    }

    public void Refresh()
    {
        refreshCoro = RefreshCoro();
        StartCoroutine(refreshCoro);
    }

	// https://developers.google.com/identity/protocols/OAuth2ServiceAccount
	// https://stackoverflow.com/questions/11126439/google-oauth-2-0-jwt-token-request-for-service-application
	// https://cloud.google.com/iam/docs/creating-managing-service-account-keys#iam-service-account-keys-create-rest
    IEnumerator AuthoriseCoro()
    {
        string uri = "https://storage.googleapis.com/unity-koi-bucket/unity-koi.p12";

		byte[] p12 = null;

		gcpJWT = new JWT();
		gcpJWT.header = gcpJWT.GetJWTHeader();
		gcpJWT.claimset = gcpJWT.GetJWTClaimSet();
		
		using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
			yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log("Error: " + webRequest.error);
            }
            else
            {
				p12 = webRequest.downloadHandler.data;
            }
        }

		gcpJWT.signature = gcpJWT.GetJWTSignature(p12);
		gcpJWT.GenerateJWT();

		string url = "https://www.googleapis.com/oauth2/v4/token";
		WWWForm form = new WWWForm();
		form.AddField("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer");
		form.AddField("assertion", gcpJWT.jwt);

		using (UnityWebRequest webRequest = UnityWebRequest.Post(url, form))
        {
            webRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
			yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log("Error: " + webRequest.error);
            }
            else
            {
				OAuth2Response response = JsonUtility.FromJson<OAuth2Response>(webRequest.downloadHandler.text);
				accessToken = response.access_token;

				authorised = true;
            }
        }
    }

	// https://developers.google.com/identity/protocols/OAuth2WebServer
    IEnumerator ExchangeCoro()
    {
        WWWForm form = new WWWForm();
        form.AddField("code", authorisationCode);
        form.AddField("client_id", clientID);
        form.AddField("client_secret", clientSecret);
        form.AddField("grant_type", "authorization_code");
        form.AddField("redirect_uri", "https://oauth2.example.com/code&");
        
        using (UnityWebRequest webRequest = UnityWebRequest.Post("https://www.googleapis.com/oauth2/v4/token", form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log("Error: " + webRequest.error);
            }
            else
            {
                //print(webRequest.GetResponseHeaders());
				foreach (var item in webRequest.GetResponseHeaders())
				{
					print(item);
				}
                exchanged = true;
            }
        }
    }

    // https://developers.google.com/identity/protocols/OAuth2InstalledApp
    // https://github.com/googlesamples/google-signin-unity/issues/57
    IEnumerator RefreshCoro()
    {
        WWWForm form = new WWWForm();
        form.AddField("client_id", clientID);
        form.AddField("client_secret", clientSecret);
        form.AddField("grant_type", "refresh_token");
        form.AddField("refresh_token", refreshToken);
        
        using (UnityWebRequest webRequest = UnityWebRequest.Post("https://www.googleapis.com/oauth2/v4/token", form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log("Error: " + webRequest.error);
            }
            else
            {
                //print(webRequest.GetResponseHeaders());
				foreach (var item in webRequest.GetResponseHeaders())
				{
					print(item);
				}
                refreshed = true;
            }
        }
    }

    IEnumerator GetTextureList(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            webRequest.SetRequestHeader("Accept", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log("Error: " + webRequest.error);
            }
            else
            {
                Bucket unsortedBucket = JsonUtility.FromJson<Bucket>(webRequest.downloadHandler.text);
                guids = unsortedBucket.items.Where(i => i.contentType == "image/png").Select(i => i.name.Split("/"[0])[1]).ToList();
            }

            initialised = true;
        }
    }
}

[Serializable]
public class Bucket
{
    public Item[] items;
}

[Serializable]
public class Item
{
    public string name;
    public string contentType;
}

[Serializable]
public class OAuth2Response
{
    public string access_token;
    public string token_type;
    public int expires_in;
}