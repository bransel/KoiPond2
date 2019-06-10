using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FadeIn : MonoBehaviour
{
    public CanvasGroup canvasGroup;
	public bool transparent { get; set; }
	public UnityEvent OnTransparent;
	public UnityEvent OnOpaque;

	public float fadeTimeInSeconds = 1;

	private bool triggered;

    // Update is called once per frame
    void Update()
    {
        if (transparent)
		{
			if (canvasGroup.alpha > 0)
			{
				canvasGroup.alpha -= Time.deltaTime / fadeTimeInSeconds;
				triggered = false;
			}
			else if (!triggered)
			{
				OnTransparent.Invoke();
				triggered = true;
			}
		}
		else
		{
			if (canvasGroup.alpha < 1)
			{
				canvasGroup.alpha += Time.deltaTime / fadeTimeInSeconds;
				triggered = false;
			}
			else if (!triggered)
			{
				OnOpaque.Invoke();
				triggered = true;
			}
		}
    }
}
