//Matt's UI Text fading thingy...

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITextFade : MonoBehaviour
{
    
    public float fadeTime = 5;
    public float fadeTimePerWord = 0.33f;
    public float maxAlpha = 1;
    public Text text;

    public string[] wordList;

    float colorFloat;
    string shownText;
    int wordIndex = 0;

    public float framesInTime;
    public float incrementValue;

    // Start is called before the first frame update
    void Start()
    {
        wordList = text.text.Split(' ');
        Debug.Log("Word length: " + wordList.Length);

        //fadeIncrement = fadeTime/wordList.Length;

        colorFloat = 0.1f;

        fadeTimePerWord = fadeTime / wordList.Length;

        framesInTime = fadeTimePerWord / Time.fixedDeltaTime;
        incrementValue = maxAlpha / framesInTime;

        Debug.Log("Fixed Delta Time: " + Time.fixedDeltaTime);
        Debug.Log("Fixed updates in time: " + framesInTime);
        Debug.Log("Increment value: " + incrementValue);
    }

    private void FixedUpdate()
    {
        if(wordIndex < wordList.Length)
        { 
            if(colorFloat < maxAlpha)
            {   
                colorFloat += incrementValue;

                int colorInt = (int)(Mathf.Lerp(0.0f, 1.0f, colorFloat) * 255.0f);
                text.text = shownText + "<color=\"#FFFFFF" + string.Format("{0:X}", colorInt) + "\">" + wordList[wordIndex] + "</color>";
            }
            else
            {
                colorFloat = 0.05f;
                shownText += wordList[wordIndex] + " ";
                wordIndex++;
            }
        }
    }
}
