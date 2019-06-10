//Matt's UI Text fading thingy...

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITextFade : MonoBehaviour
{
    
    [Header("Basic Settings")]
    public float fadeTime = 5;
    public float fadeTimePerWord = 0.33f;    
    public float maxAlpha = 1;
    public Text text;

    [Header("Parallel Settings")]
    public bool parallelMode = false;
    public float randomElement = 0.5f;

    [Header("System Variables")]
    public string[] wordList;
    public float[] colourFloats;
    public float[] incrementValues;

    float colourFloat;
    string shownText;
    int wordIndex = 0;

    public float framesInTime;
    public float baseIncrementValue;
    public float incrementValue;

    // Start is called before the first frame update
    void Start()
    {
        wordList = text.text.Split(' ');
        Debug.Log("Word length: " + wordList.Length);

        //fadeIncrement = fadeTime/wordList.Length;



        if (parallelMode)
            ParallelInit();
        else        
            SerialInit();
    }

    void SerialInit()
    {
        fadeTimePerWord = fadeTime / wordList.Length;

        framesInTime = fadeTimePerWord / Time.fixedDeltaTime;
        baseIncrementValue = maxAlpha / framesInTime;
        incrementValue = baseIncrementValue;
        colourFloat = 0.1f;
    }
    void ParallelInit()
    {
        colourFloats = new float[wordList.Length];
        incrementValues = new float[wordList.Length];

        framesInTime = fadeTime / Time.fixedDeltaTime;
        baseIncrementValue = maxAlpha / framesInTime;
        incrementValue = baseIncrementValue;

        for (int i = 0; i < wordList.Length; i++)
        {
            colourFloats[i] = 0.08f;
            incrementValues[i] = baseIncrementValue + (Random.Range(0, randomElement) * Time.fixedDeltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (parallelMode)
            ParallelMode();
        else
            SequentialMode();
    }
    
    void ParallelMode()
    {
        text.text = "";

        for (int i = 0; i < wordList.Length; i++)
        {
            if (colourFloats[i] < maxAlpha)
            {
                colourFloats[i] += incrementValues[i];
            }

            int colorInt = (int)(Mathf.Lerp(0.0f, 1.0f, colourFloats[i]) * 255.0f);
            string wordString = "<color=\"#FFFFFF" + string.Format("{0:X}", colorInt) + "\">" + wordList[i] + "</color> ";
            text.text += wordString;            
        }
    }

    void SequentialMode()
    {
        if (wordIndex < wordList.Length)
        {
            if (colourFloat < maxAlpha)
            {
                colourFloat += incrementValue;
                int colorInt = (int)(Mathf.Lerp(0.0f, 1.0f, colourFloat) * 255.0f);
                text.text = shownText + "<color=\"#FFFFFF" + string.Format("{0:X}", colorInt) + "\">" + wordList[wordIndex] + "</color>";
            }
            else
            {
                incrementValue = baseIncrementValue;// + Random.Range(0, randomElement / framesInTime);
                colourFloat = 0.1f;
                shownText += wordList[wordIndex] + " ";
                wordIndex++;
            }
        }
    }
}
