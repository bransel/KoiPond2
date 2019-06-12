/// <summary>
/// CodeArtist.mx 2015
/// This is the main class of the project, its in charge of raycasting to a model and place brush prefabs infront of the canvas camera.
/// If you are interested in saving the painted texture you can use the method at the end and should save it to a file.
/// </summary>


using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Events;

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

    public SpriteRenderer cursorSprite;
    Sprite targetSprite;
    TweetClient tweetClient;
    public float pivotx, pivoty;

    void Awake()
    {
        Texture2D tex = new Texture2D(canvasTexture.width, canvasTexture.height, TextureFormat.RGB24, false);
        var fillColorArray = tex.GetPixels();

        for (var i = 0; i < fillColorArray.Length; ++i)
            fillColorArray[i] = Color.white;

        tex.SetPixels(fillColorArray);
        tex.Apply();
        baseMaterial.mainTexture = tex;
        
        cursorSprite = brushCursor.GetComponent<SpriteRenderer>();
        targetSprite = cursorSprite.sprite;

        GameObject tweetClientObject = new GameObject("TweetClient");
        tweetClient = tweetClientObject.AddComponent<TweetClient>();
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
            brushObj.transform.eulerAngles = cursorSprite.transform.eulerAngles;
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

        //Vector2 pivot = new Vector2(pivotx, pivoty);
        //Debug.Log(pivot);

        // cursorSprite.sprite = Sprite.Create(sprite.texture, new Rect(0, 0, width, height), Vector2.one/2f, sprite.pixelsPerUnit);
        cursorSprite.sprite = Sprite.Create(sprite.texture, new Rect(0, 0, width, height), Vector2.one / 2f, sprite.pixelsPerUnit);
        cursorSprite.color = brushColor;

        cursorPaint = sprite;
    }

    public void SetBrushSize(float size)
    { //Sets the size of the cursor brush or decal
        brushSize = size;
        brushCursor.transform.localScale = Vector3.one * brushSize;
        //trying to reset the position of cursorsprite on resize

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

    public GameObject UploadMessage;
    public GameObject EmptyMessagePanel;
    public GameObject RaycastBlocker;

    public void SaveTextureAndMessage(InputField inputField)
    {
        if (string.IsNullOrEmpty(inputField.text))
            ShowEmptyMessagePanel();
        else if (uploadInProgress)
            Debug.LogError("Upload is already in progress!");
        else
        {
            uploadInProgress = true;
            UploadMessage.SetActive(true);

            string guid = System.Guid.NewGuid().ToString();
            byte[] textureBytes = SaveTexture().EncodeToPNG();

            SceneController sceneController = FindObjectOfType<SceneController>();

            UnityEvent sceneLoadEvent = new UnityEvent();
            sceneLoadEvent.AddListener(sceneController.LoadNextScene);

            UnityEvent uploadFailEvent = new UnityEvent();
            uploadFailEvent.AddListener(DisableUploadMessage);

            tweetClient.TweetMessageAndImage(inputField.text, textureBytes, sceneLoadEvent, uploadFailEvent);
        }
    }

    void DisableUploadMessage()
    {
        UploadMessage.SetActive(false);
    }

    void ShowEmptyMessagePanel()
    {
        EmptyMessagePanel.SetActive(true);
        RaycastBlocker.SetActive(true);
    }

    public void Pond()
    {
        SceneController sceneController = FindObjectOfType<SceneController>();

        sceneController.LoadNextScene();



    }
}
