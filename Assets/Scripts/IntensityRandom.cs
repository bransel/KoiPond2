using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntensityRandom : MonoBehaviour
{
    Light light;
    public float Timer = 0;
    public float currentInt;
    public float interpolation;
    private float random; 
    // Start is called before the first frame update
    void Start()
    {
        light = this.GetComponent<Light>();
        random = Random.Range(0.65f, 1f);
        light.intensity = random;
        currentInt = light.intensity;
        
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;

        light.intensity = Mathf.Lerp(currentInt, (random), Time.deltaTime * interpolation);


        currentInt = light.intensity;

        if (Timer > 13 )
        {
            Timer = 0;
            random = Random.Range(0.65f, 1f);
        }
    }
}
