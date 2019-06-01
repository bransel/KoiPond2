using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITextFade : MonoBehaviour
{
    public float fadeRate = 0.2f;
    public Text text;

    // Start is called before the first frame update
    void Start()
    {
        Color c = text.color;
        c.a = 0;
        text.color = c;
    }

    private void Update()
    {
        Color c = text.color;

        Debug.Log("C: a: " + c.a);
        if(c.a < 1)
        { 
            c.a += fadeRate * Time.deltaTime;
            text.color = c;
        }
    }
}
