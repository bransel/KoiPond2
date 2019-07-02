using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BubbleButton : MonoBehaviour
{
	public Text text;
	public Image image;
	public CanvasGroup canvasGroup;
	public FadeInTextLetterByLetter fadeInTextLetterByLetter;
    public UITextFade parallelfade;

	public string message { get; set; }
	public new Rigidbody rigidbody { get; private set; }
	public Fish fish { get; set; }
	public FishController fishController { get; set; }

	private RectTransform bubble;
	private new CanvasRenderer renderer;
	private Button button;

	public float fadeTimeInSeconds = 1;
	public float fadeTimeOutSeconds = 1;
	public float riseSpeed = 1;
	public bool triggered { get; set; }
	private bool fading;

	private float fadeTimer;
    public float fadeWait; //hopefully this is how long to wait before fades

	private int clickCount;
	private Color origImageColour;
	private Color transparentColor;

	private PostAndUIControls postAndUIControls;
	private LayoutElement textLayoutElement;
	private RectTransform textRectTransform;
	private ContentSizeFitter textContentSizeFitter;

	private Transform bg;
	private RectTransform bgRectTransform;
	private BoxCollider2D boxCollider2D;

	// Start is called before the first frame update
	IEnumerator Start()
	{
		canvasGroup.alpha = 0;
		//origImageColour = image.color;
		//transparentColor = origImageColour;
		//transparentColor.a = 0;
		//image.color = transparentColor;

		text.text = message;
		bubble = GetComponent<RectTransform>();
		rigidbody = GetComponent<Rigidbody>();
		renderer = GetComponent<CanvasRenderer>();
		button = GetComponent<Button>();
		AssignActiveBubble(this);

		postAndUIControls = FindObjectOfType<PostAndUIControls>();
		textLayoutElement = text.GetComponent<LayoutElement>();
		textRectTransform = text.GetComponent<RectTransform>();
		textContentSizeFitter = text.GetComponent<ContentSizeFitter>();

		bg = text.transform.GetChild(0);
		bgRectTransform = bg.GetComponent<RectTransform>();
		boxCollider2D = GetComponent<BoxCollider2D>();

		textRectTransform.ForceUpdateRectTransforms();

		yield return null;

		if (textRectTransform.sizeDelta.x > 0)
			textLayoutElement.preferredWidth = bubble.sizeDelta.x;

		yield return null;

		textContentSizeFitter.enabled = false;

		bgRectTransform.ForceUpdateRectTransforms();
		Vector2 rectSize = new Vector2(bgRectTransform.rect.width, bgRectTransform.rect.height);
		boxCollider2D.size = rectSize + Vector2.one * 10;

		bg.SetParent(text.transform.parent);
		bg.SetAsFirstSibling();

        fadeInTextLetterByLetter.enabled = true;
        //parallelfade.enabled = true;

        fadeInTextLetterByLetter.OnFadeFinish.AddListener(TextFadeFinish);
        //parallelfade.OnfadeFinish.AddListener(TextFadeFinish);

		//transform.position = Camera.main.WorldToScreenPoint(transform.position);
		//Vector2 clampedPos = Input.mousePosition;

		RectTransform bubbleRegion = GameObject.FindWithTag("BubbleRegion").GetComponent<RectTransform>();
		
		// x = left, y = right, z = top, w = bottom
		Vector4 lrtb = new Vector4(-bubbleRegion.sizeDelta.x / 2f + bubbleRegion.anchoredPosition.x, 
								   -bubbleRegion.sizeDelta.x / 2f - bubbleRegion.anchoredPosition.x,
								   -bubbleRegion.sizeDelta.y / 2f - bubbleRegion.anchoredPosition.y,
								   -bubbleRegion.sizeDelta.y / 2f + bubbleRegion.anchoredPosition.y);
								   
		Vector2 clampedPos = Camera.main.WorldToScreenPoint(fish.transform.position);
		clampedPos.x = Mathf.Clamp(clampedPos.x, lrtb.x, Screen.width - lrtb.y);
		clampedPos.y = Mathf.Clamp(clampedPos.y, lrtb.w, Screen.height - lrtb.z);
		bubble.anchoredPosition = clampedPos;

		StartCoroutine(UpdateUI());
		
		for (float t = 0; t < 1; t += Time.deltaTime / fadeTimeInSeconds)
		{
			canvasGroup.alpha = Mathf.Lerp(0, 1, Mathf.SmoothStep(0, 1, t));
			//image.color = Color.Lerp(transparentColor, origImageColour, Mathf.SmoothStep(0, 1, t));
			yield return null;
		}

		while (fadeTimer < fadeWait)
		{
			fadeTimer += Time.deltaTime;
			yield return null;
		}
		
		if (!fading)
			StartCoroutine(FadeOutCoro());
	}

	IEnumerator UpdateUI()
	{
		int fontSize = 0;
		Vector2 bubbleSizeDelta = Vector2.zero;
		WaitForSeconds waitYield = new WaitForSeconds(0.1f);

		while (true)
		{
			fontSize = postAndUIControls.bubbleFontSizeFloat;
			bubbleSizeDelta = new Vector2(postAndUIControls.bubbleWidthFloat, bubble.sizeDelta.y);
			
			if (text.fontSize != fontSize || bubble.sizeDelta != bubbleSizeDelta)
			{
				text.fontSize = fontSize;
				bubble.sizeDelta = bubbleSizeDelta;
				
				fadeInTextLetterByLetter.enabled = false;
				fontSize = text.fontSize;
				bg.SetParent(text.transform);
				textContentSizeFitter.enabled = true;
				textLayoutElement.preferredWidth = bubbleSizeDelta.x;
				textRectTransform.ForceUpdateRectTransforms();

				yield return null;

				fadeInTextLetterByLetter.enabled = true;

				bgRectTransform.ForceUpdateRectTransforms();
				boxCollider2D.size = new Vector2(bgRectTransform.rect.width, bgRectTransform.rect.height) + Vector2.one * 10;

				textContentSizeFitter.enabled = false;
				bg.SetParent(text.transform.parent);
				bg.SetAsFirstSibling();
			}

			yield return waitYield;
		}
	}

	void Update()
	{
		transform.position += Vector3.up * riseSpeed;
	}

	// https://forum.unity.com/threads/point-in-camera-view.72523/#post-464141
	bool CanCameraSeePoint(Camera camera, Vector3 point)
	{
		Vector3 viewportPoint = camera.WorldToViewportPoint(point);
		return (viewportPoint.z > 0 && (new Rect(0, 0, 1, 1)).Contains(viewportPoint));
	}

	public void AssignActiveBubble(BubbleButton bubble)
	{
		fishController.AssignActiveBubble(bubble);
	}

	public void FadeOut()
	{
		if (!triggered)
		{
			triggered = true;
			fish.currentTarget = fishController.GetNewTargetPos();
			fish.moveSpeed = Random.Range(fish.minMoveSpeed, fish.maxMoveSpeed);
			fish.rotSpeed = Random.Range(fish.minRotSpeed, fish.maxRotSpeed);
		}
		
		if (!fading)
			StartCoroutine(FadeOutCoro());
	}

	public void TextFadeFinish()
	{
		if (clickCount == 0)
			clickCount++;
	}

	public void Click()
	{
		if (clickCount == 0)
		{
			fadeInTextLetterByLetter.StopAndShowAll();
			clickCount++;
		}
		else if (clickCount == 1)
		{
			fadeTimer = fadeWait;
			clickCount++;
		}
	}

	IEnumerator FadeOutCoro()
	{
		fading = true;

		float alpha = canvasGroup.alpha;

		for (float t = 0; t < 1; t += Time.deltaTime / fadeTimeOutSeconds)
		{
			canvasGroup.alpha = Mathf.Lerp(alpha, 0, Mathf.SmoothStep(0, 1, t));
			//image.color = Color.Lerp(origImageColour, transparentColor, Mathf.SmoothStep(0, 1, t));
			yield return null;
		}

		Destroy(gameObject);
	}
}
