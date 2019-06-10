using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
// © 2017 TheFlyingKeyboard and released under MIT License
// theflyingkeyboard.net
// Modifed by Matt Cabanag & Peter Liang
public class FadeInTextLetterByLetter : MonoBehaviour
{
    [SerializeField] private Text textToUse;
    [SerializeField] private bool useThisText = false;
    [TextAreaAttribute(4, 15)]
    [SerializeField] private string textToShow;
    [SerializeField] private bool useTextText = false;
    [SerializeField] private float fadeSpeedMultiplier = 0.25f;
    [SerializeField] private bool fade;
    private float colorFloat = 0.1f;
    private int colorInt;
    private int letterCounter = 0;
    private string shownText;
	private IEnumerator fadeCoro;

    public bool wordMode = false;
    public string[] wordList;
    int wordCounter = 0;
    public float totalTime;

	public UnityEvent OnFadeFinish;

    private void Start()
    {
        if (useThisText)
        {
            textToUse = GetComponent<Text>();
        }
        if (useTextText)
        {
            textToShow = textToUse.text;
        }


        wordList = textToUse.text.Split(' ');        

        if (fade)
        {
            Fade();
        }

        
    }

    private IEnumerator FadeInText()
    {
        while (letterCounter < textToShow.Length)
        {

            if (colorFloat < 1.0f)
            {
                colorFloat += Time.deltaTime * fadeSpeedMultiplier;
                colorInt = (int)(Mathf.Lerp(0.0f, 1.0f, colorFloat) * 255.0f);
                textToUse.text = shownText + "<color=\"#FFFFFF" + string.Format("{0:X}", colorInt) + "\">" + textToShow[letterCounter] + "</color>";
            }
            else
            {
                colorFloat = 0.1f;
                shownText += textToShow[letterCounter];
                letterCounter++;
            }
            

            yield return null;
        }

		OnFadeFinish.Invoke();
    }

    private IEnumerator FadeInWords()
    {
        while (wordCounter < wordList.Length)
        {
            if (colorFloat < 1.0f)
            {
                colorFloat += Time.deltaTime * fadeSpeedMultiplier;
                colorInt = (int)(Mathf.Lerp(0.0f, 1.0f, colorFloat) * 255.0f);
                textToUse.text = shownText + "<color=\"#FFFFFF" + string.Format("{0:X}", colorInt) + "\">" + wordList[wordCounter] + "</color>";
            }
            else
            {
                colorFloat = 0.1f;
                shownText += wordList[wordCounter] + " ";
                wordCounter++;
            }
            yield return null;
        }

		OnFadeFinish.Invoke();
    }
    public void Fade()
    {
        if (fadeCoro != null)
			StopCoroutine(fadeCoro);
		
		if (wordMode)
            fadeCoro = FadeInWords();
        else
            fadeCoro = FadeInText();

		StartCoroutine(fadeCoro);
    }

	public void StopAndShowAll()
	{
		StopCoroutine(fadeCoro);
		textToUse.text = textToShow;
	}
}
