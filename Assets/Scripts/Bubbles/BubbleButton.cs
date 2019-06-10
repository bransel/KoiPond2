using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BubbleButton : MonoBehaviour
{
	public Text text;
	public CanvasGroup canvasGroup;
	public FadeInTextLetterByLetter fadeInTextLetterByLetter;

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
	private int clickCount;

	// Start is called before the first frame update
	IEnumerator Start()
	{
		text.text = message;
		bubble = GetComponent<RectTransform>();
		rigidbody = GetComponent<Rigidbody>();
		renderer = GetComponent<CanvasRenderer>();
		button = GetComponent<Button>();
		AssignActiveBubble(this);

		RectTransform textRectTransform = text.GetComponent<RectTransform>();
		textRectTransform.ForceUpdateRectTransforms();
		yield return null;

		if (textRectTransform.sizeDelta.x > 0)
		{
			LayoutElement layoutElement = text.GetComponent<LayoutElement>();
			layoutElement.preferredWidth = bubble.sizeDelta.x;
		}

		yield return null;

		text.GetComponent<ContentSizeFitter>().enabled = false;

		Transform bg = text.transform.GetChild(0);
		RectTransform bgRectTransform = bg.GetComponent<RectTransform>();
		bgRectTransform.ForceUpdateRectTransforms();
		Vector2 rectSize = new Vector2(bgRectTransform.rect.width, bgRectTransform.rect.height);
		GetComponent<BoxCollider2D>().size = rectSize + Vector2.one * 10;

		bg.SetParent(text.transform.parent);
		bg.SetAsFirstSibling();
		
		fadeInTextLetterByLetter.enabled = true;

		fadeInTextLetterByLetter.OnFadeFinish.AddListener(TextFadeFinish);

		//transform.position = Camera.main.WorldToScreenPoint(transform.position);
		Vector2 clampedPos = Input.mousePosition;
		clampedPos.x = Mathf.Clamp(clampedPos.x, 100, Screen.width - 100);
		clampedPos.y = Mathf.Clamp(clampedPos.y, 100, Screen.height - 100);
		bubble.anchoredPosition = clampedPos;

		for (float t = 0; t < 1; t += Time.deltaTime / fadeTimeInSeconds)
		{
			canvasGroup.alpha = Mathf.Lerp(0, 1, Mathf.SmoothStep(0, 1, t));
			yield return null;
		}

		while (fadeTimer < 10f)
		{
			fadeTimer += Time.deltaTime;
			yield return null;
		}
		
		if (!fading)
			StartCoroutine(FadeOutCoro());
	}

	void Update()
	{
		/* if (!CanCameraSeePoint(Camera.main, transform.position))
			Destroy(bubbleParent.gameObject); */
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
			fadeTimer = 10f;
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
			yield return null;
		}

		Destroy(gameObject);
	}
}
