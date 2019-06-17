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
    public float maxInt = 1.1f;
    public float minInt = 0.45f;
    public ReflectionProbe rp;
    RenderTexture targetTexture;

    // Start is called before the first frame update
    void Start()
    {
        light = this.GetComponent<Light>();
        random = Random.Range(minInt, maxInt);
        light.intensity = random;
        currentInt = light.intensity;
        rp.RenderProbe(targetTexture = null);

    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;

        light.intensity = Mathf.Lerp(currentInt, (random), Time.deltaTime * interpolation);


        currentInt = light.intensity;

        if (Timer > 25 )
        {
            Timer = 0;
            random = Random.Range(minInt, maxInt);
            rp.RenderProbe(targetTexture = null);
        }
    }
}
