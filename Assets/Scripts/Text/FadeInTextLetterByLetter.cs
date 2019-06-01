using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// © 2017 TheFlyingKeyboard and released under MIT License
// theflyingkeyboard.net
// Modifed by Matt Cabanag
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

    public bool wordMode = false;
    public string[] wordList;
    int wordCounter = 0;
    public float totalTime;   

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
    }
    public void Fade()
    {
        if (wordMode)
            StartCoroutine(FadeInWords());
        else
            StartCoroutine(FadeInText());
    }
}
