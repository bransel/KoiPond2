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

	private RectTransform bubbleParent;
	private RectTransform bubble;
	private new CanvasRenderer renderer;
	private Button button;

	public float fadeTimeInSeconds = 1;
	public bool triggered { get; set; }
	private bool fading;

	private float fadeTimer;
	private int clickCount;

	// Start is called before the first frame update
	IEnumerator Start()
	{
		text.text = message;
		bubbleParent = transform.parent.GetComponent<RectTransform>();
		bubble = GetComponent<RectTransform>();
		rigidbody = GetComponent<Rigidbody>();
		renderer = GetComponent<CanvasRenderer>();
		button = GetComponent<Button>();
		AssignActiveBubble(this);

		fadeInTextLetterByLetter.OnFadeFinish.AddListener(TextFadeFinish);

		/* Vector3 pos = transform.localPosition;
		pos.z = -100f;
		transform.localPosition = pos; */
		transform.position = Camera.main.WorldToScreenPoint(transform.position);

		for (float t = 0; t < 1; t += Time.deltaTime / fadeTimeInSeconds)
		{
			//bubbleParent.sizeDelta = Vector3.Lerp(Vector2.one * 5, Vector2.one * 120, Mathf.Pow(t, 2));
			canvasGroup.alpha = Mathf.Lerp(0, 1, Mathf.SmoothStep(0, 1, t));
			yield return null;
		}

		/* while (transform.position.z > -1.5f)
			yield return null; */

		//yield return new WaitForSeconds(10f);

		while (fadeTimer < 10f)
		{
			fadeTimer += Time.deltaTime;
			yield return null;
		}
		
		if (!fading)
			StartCoroutine(FadeOutCoro());

		//yield return new WaitForSeconds(1.5f);

		//Expand();
	}

	/* void Update()
	{
		if (!CanCameraSeePoint(Camera.main, transform.position))
			Destroy(bubbleParent.gameObject);
	} */

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

		for (float t = 0; t < 1; t += Time.deltaTime)
		{
			canvasGroup.alpha = Mathf.Lerp(alpha, 0, Mathf.SmoothStep(0, 1, t));
			yield return null;
		}

		Destroy(gameObject);
	}
}
