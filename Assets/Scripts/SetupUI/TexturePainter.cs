/// <summary>
/// CodeArtist.mx 2015
/// This is the main class of the project, its in charge of raycasting to a model and place brush prefabs infront of the canvas camera.
/// If you are interested in saving the painted texture you can use the method at the end and should save it to a file.
/// </summary>


using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class TexturePainter : MonoBehaviour
{
    public GameObject brushCursor, brushContainer; //The cursor that overlaps the model and our container for the brushes painted
    public Camera sceneCamera, canvasCam;  //The camera that looks at the model, and the camera that looks at the canvas.
    public Sprite cursorPaint; // Cursor for the differen functions 
    public RenderTexture canvasTexture; // Render Texture that looks at our Base Texture and the painted brushes
    public Material baseMaterial; // The material of our base texture (Were we will save the painted texture)
    public Color brushColor; //The selected color

    float brushSize = 1.0f, brushOpacity = 1.0f, brushBlur = 1.0f; //The size of our brush
    int brushCounter = 0, MAX_BRUSH_COUNT = 1000; //To avoid having millions of brushes
    bool saving = false; //Flag to check if we are saving the texture
    bool uploadInProgress = false, messageUploaded = false, textureUploaded = false;

    SpriteRenderer cursorSprite;
    Sprite targetSprite;
	TweetClient tweetClient;

    void Awake()
    {
        cursorSprite = brushCursor.GetComponent<SpriteRenderer>();
        targetSprite = cursorSprite.sprite;

        GameObject tweetClientObject = new GameObject("TweetClient");
        tweetClient = tweetClientObject.AddComponent<TweetClient>();
        tweetClient.InitTweetClient();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            DoAction();
        }

        UpdateBrushCursor();
    }

    //The main action, instantiates a brush or decal entity at the clicked position on the UV map
    void DoAction()
    {
        if (saving)
            return;

        Vector3 uvWorldPosition = Vector3.zero;

        if (HitTestUVPosition(ref uvWorldPosition))
        {
            GameObject brushObj = new GameObject("Brush");

            float width = cursorPaint.texture.width;
            float height = cursorPaint.texture.height;

            SpriteRenderer brushSprite = brushObj.AddComponent<SpriteRenderer>();

            //brushSprite.sprite = Sprite.Create(cursorPaint.texture, new Rect(0, 0, width, height), Vector2.one / 2f, cursorPaint.pixelsPerUnit);
            brushSprite.sprite = cursorSprite.sprite;
            brushSprite.color = brushColor; //Set the brush color
            brushSprite.sortingOrder = brushCounter;
            cursorSprite.sortingOrder = brushCounter + 1;

            //brushColor.a = brushSize * 2.0f; // Brushes have alpha to have a merging effect when painted over.
            brushColor.a = brushOpacity;
            brushObj.transform.parent = brushContainer.transform; //Add the brush to our container to be wiped later
            brushObj.transform.localPosition = uvWorldPosition; //The position of the brush (in the UVMap)
            brushObj.transform.localScale = Vector3.one * brushSize;//The size of the brush
        }

        brushCounter++; //Add to the max brushes

        if (brushCounter >= MAX_BRUSH_COUNT)
        { //If we reach the max brushes available, flatten the texture and clear the brushes
            brushCursor.SetActive(false);
            saving = true;
            Invoke("SaveTexture", 0.1f);

        }
    }

    //To update at realtime the painting cursor on the mesh
    void UpdateBrushCursor()
    {
        Vector3 uvWorldPosition = Vector3.zero;
        if (HitTestUVPosition(ref uvWorldPosition) && !saving)
        {
            brushCursor.SetActive(true);
            brushCursor.transform.position = uvWorldPosition + brushContainer.transform.position;
        }
        else
        {
            brushCursor.SetActive(false);
        }
    }

    //Returns the position on the texuremap according to a hit in the mesh collider
    bool HitTestUVPosition(ref Vector3 uvWorldPosition)
    {
        RaycastHit hit;
        Vector3 cursorPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
        Ray cursorRay = sceneCamera.ScreenPointToRay(cursorPos);
        if (Physics.Raycast(cursorRay, out hit, 200))
        {
            MeshCollider meshCollider = hit.collider as MeshCollider;
            if (meshCollider == null || meshCollider.sharedMesh == null)
                return false;
            Vector2 pixelUV = new Vector2(hit.textureCoord.x, hit.textureCoord.y);
            uvWorldPosition.x = pixelUV.x - canvasCam.orthographicSize;//To center the UV on X
            uvWorldPosition.y = pixelUV.y - canvasCam.orthographicSize;//To center the UV on Y
            uvWorldPosition.z = 0.0f;
            return true;
        }
        else
        {
            return false;
        }
    }

    //Sets the base material with a our canvas texture, then removes all our brushes
    Texture2D SaveTexture()
    {
        brushCounter = 0;
        System.DateTime date = System.DateTime.Now;
        RenderTexture.active = canvasTexture;
        Texture2D tex = new Texture2D(canvasTexture.width, canvasTexture.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, canvasTexture.width, canvasTexture.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        baseMaterial.mainTexture = tex; //Put the painted texture as the base

        foreach (Transform child in brushContainer.transform)
        {
            //Clear brushes
			Destroy(child.gameObject);
        }

        //StartCoroutine ("SaveTextureToFile"); //Do you want to save the texture? This is your method!
        Invoke("ShowCursor", 0.1f);

        return tex;
    }

    //Show again the user cursor (To avoid saving it to the texture)
    void ShowCursor()
    {
        saving = false;
    }

    ////////////////// PUBLIC METHODS //////////////////

    public void SetBrush(Sprite sprite)
    {
        targetSprite = sprite;
        
        float width = sprite.texture.width;
		float height = sprite.texture.height;
		
		brushCursor.GetComponent<SpriteRenderer>().sprite = Sprite.Create(sprite.texture, new Rect(0, 0, width, height), Vector2.one / 2f, sprite.pixelsPerUnit);
        cursorPaint = sprite;
    }

    public void SetBrush(Image image)
    {
        targetSprite = image.sprite;
        Sprite sprite = image.sprite;
        
        float width = sprite.texture.width;
		float height = sprite.texture.height;
		
        cursorSprite.sprite = Sprite.Create(sprite.texture, new Rect(0, 0, width, height), Vector2.one / 2f, sprite.pixelsPerUnit);
        cursorSprite.color = brushColor;

        cursorPaint = sprite;
    }

    public void SetBrushSize(float size)
    { //Sets the size of the cursor brush or decal
        brushSize = size;
        brushCursor.transform.localScale = Vector3.one * brushSize;
    }

    public void SetBrushColour(Image image)
    {
        if (!cursorSprite)
            return;
        
        brushColor = image.color;
        brushColor.a = brushOpacity;

        cursorSprite.color = brushColor;
    }

    public void SetBrushColour(Color newBrushColour)
    {
        if (!cursorSprite)
            return;
        
        brushColor = newBrushColour;
        brushColor.a = brushOpacity;

        cursorSprite.color = brushColor;
    }

    public void SetBrushOpacity(float alpha)
    {
        if (!cursorSprite)
            return;
        
        brushOpacity = alpha;
        brushColor.a = brushOpacity;

        cursorSprite.color = brushColor;
    }

	public void SetBrushRotation(float angle)
    {
        cursorSprite.transform.eulerAngles = -Vector3.forward * angle * 360;
    }

    ////////////////// OPTIONAL METHODS //////////////////

    public void SaveTextureAndMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
            Debug.LogError("Message is empty!");
        else
        {
            string guid = System.Guid.NewGuid().ToString();
			byte[] textureBytes = SaveTexture().EncodeToPNG();

			tweetClient.TweetMessageAndImage(message, textureBytes);

            //StartCoroutine(UploadTexture(textureBytes, guid));
            //StartCoroutine(UploadMessage(message, guid));
        }
    }

    public void SaveTextureAndMessage(Text text)
    {
        if (string.IsNullOrEmpty(text.text))
            Debug.LogError("Message is empty!");
        else if (uploadInProgress)
            Debug.LogError("Upload is already in progress!");
        else
        {
            uploadInProgress = true;

            string guid = System.Guid.NewGuid().ToString();
			byte[] textureBytes = SaveTexture().EncodeToPNG();

			tweetClient.TweetMessageAndImage(text.text, textureBytes);

            //StartCoroutine(UploadTexture(textureBytes, guid));
            //StartCoroutine(UploadMessage(text.text, guid));
            StartCoroutine(LoadSceneOnUpload());
        }
    }

    IEnumerator UploadTexture(byte[] textureBytes, string guid)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", textureBytes, "texture.png", "image/png");
        
        using (UnityWebRequest webRequest = UnityWebRequest.Post(string.Format("https://storage.googleapis.com/unity-koi-bucket/Data/{0}/texture.png", guid), form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
                Debug.Log("Error: " + webRequest.error);
            else
                textureUploaded = true;
        }

        yield return null;
    }

    IEnumerator UploadMessage(string message, string guid)
    {
        byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", messageBytes, "message.txt", "text/plain");
        
        using (UnityWebRequest webRequest = UnityWebRequest.Post(string.Format("https://storage.googleapis.com/unity-koi-bucket/Data/{0}/message.txt", guid), form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
                Debug.Log("Error: " + webRequest.error);
            else
                messageUploaded = true;
        }

        yield return null;
    }

    IEnumerator LoadSceneOnUpload()
    {
        /* int timeout = 1000;
        int timer = 0; */

        SceneController sceneController = FindObjectOfType<SceneController>();

        yield return new WaitForSeconds(2.5f);

        /* while (!textureUploaded || !messageUploaded)
        {
            timer++;

            if (timer >= timeout)
            {
                uploadInProgress = false;
                yield break;
            }

            yield return null;
        } */

        sceneController.LoadNextScene();
    }
}
