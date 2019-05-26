using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RandomMove : MonoBehaviour
{
    Vector3 dest;
    public float range = 5f;
    public float speed = 1f;
    private float z; 
    void Start()
    {
        z = this.transform.position.z; 
        dest = transform.position = new Vector3(Random.Range(-range, range), Random.Range(-range, range), z);
    }

    void Update()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, dest, step);
        if (Vector3.Distance(transform.position, dest) < 0.5f)
        {
            dest = transform.position = new Vector3(Random.Range(-range, range),  Random.Range(-range, range), z);
        }
    }
}
