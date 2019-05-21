using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SkinsController : MonoBehaviour
{
    public ButtonElement buttonPrefab;
    public Transform buttonPanel;

    private string URI = "https://storage.googleapis.com/unity-koi-bucket/";

    public GCPTextureJSON gcpTextureJSON;
    public FishController fishController;

    private List<ButtonElement> buttonElements = new List<ButtonElement>();
    private List<FishData> fishDataList = new List<FishData>();

    bool finishedDL;
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        //fishController.LinkFishDataList(fishDataList);
		
		if (!gcpTextureJSON && !(gcpTextureJSON = FindObjectOfType<GCPTextureJSON>()))
            gcpTextureJSON = gameObject.AddComponent<GCPTextureJSON>();
        
        gcpTextureJSON.Initialise();

        while (!gcpTextureJSON.initialised)
            yield return null;
        
        StartCoroutine(GetTextures());
    }

    IEnumerator GetTextures()
    {
        foreach (var guid in gcpTextureJSON.guids)
        {
            string textureURN = string.Format("Data/{0}/texture.png", guid);
            string messageURN = string.Format("Data/{0}/message.txt", guid);

            ButtonElement buttonElement;
            buttonElement = Instantiate(buttonPrefab);
            buttonElement.transform.SetParent(buttonPanel);
			buttonElement.transform.localPosition = Vector3.zero;
			buttonElement.transform.localScale = Vector3.one;
            buttonElements.Add(buttonElement);

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
    }
}

[Serializable]
public class FishData
{
    public string guid;
    public string message;
    public Texture texture;
}
