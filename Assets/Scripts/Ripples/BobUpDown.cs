using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BobUpDown : MonoBehaviour
{
    [Header("Transforms to move")]
    public List<Transform> transforms;
    [Header("Settings")]
    [Range(0, 10)]
    public float minSpeed = 0.5f;
    [Range(0, 10)]
    public float maxSpeed = 2;
    [Range(0, 1)]
    public float amount = 0.3f;

    float[] speeds;
    Vector3[] startPositions;
    float[] offsets;

    private void OnValidate()
    {
        speeds = transforms.Select(t => Random.Range(minSpeed, maxSpeed)).ToArray();
        offsets = transforms.Select(t => Random.Range(0f, 100f)).ToArray();
    }

    void Start()
    {
        speeds = transforms.Select(t => Random.Range(minSpeed, maxSpeed)).ToArray();
        startPositions = transforms.Select(t => t.position).ToArray();
    }

    void Update()
    {
        for (int i = 0; i < transforms.Count; i++)
        {
            float theta = offsets[i] + Time.time * speeds[i];
            float amp = amount * Time.deltaTime;
            float dy = Mathf.Sin(theta) * amount;
            transforms[i].position = startPositions[i] + Vector3.forward * dy;
        };
    }
}
