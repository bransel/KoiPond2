using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntensityRandom : MonoBehaviour
{
    Light light;
    public float Timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        light = this.GetComponent<Light>();    
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;
        if (Timer >9)
        {
            light.intensity = Random.Range(0.65f, 0.96f);
            Timer = 0;
        }
        
    }
}
