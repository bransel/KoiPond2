using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemainingChar : MonoBehaviour
    

{
    public Text thisOne;
    public Text Message;
    public int remain = 280;
    private float Timer;
    public string[] words;
    public int current = 0;
    public InputField input; 
    // Start is called before the first frame update
    void Start()
    {
        thisOne.text = words[current] + " (" + remain + " Char Left)";
        
    }

    // Update is called once per frame
    void Update()
    {
        
        remain = input.characterLimit - input.text.Length;
        Timer += Time.deltaTime;
        thisOne.text = words[current] + " (" + remain + " Char Left)";
        /*if (Timer > 2 && remain == 280)
        {
            Reset();

            Timer = 0;
            

        }
        if (remain < 280)
        {
            thisOne.text = words[current] + " (" + remain + " Char Left)";
        }*/

        if (Timer > 5)
        {
            Reset();
            Timer = 0;
        }


    }

    private void Reset()
    {
        current = Random.Range(0, words.Length );
       
        
    }
    private void remains()
    {

    }
}
