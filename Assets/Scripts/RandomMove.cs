using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RandomMove : MonoBehaviour
{
    Vector3 dest;
    public float range = 5f;
    public float speed = 1f;
    private float z, orZ;
    private float x, orX, xV, xGap;  // xV , yV = new travelled distance
    private float y, orY, yV, yGap;
        public float stepX, stepY;
    public float gap = 2f; // how far things have floated away
    public float resetgap = 0.4f; // the point in which the stepX, step Y resets
    public float timer, timing;
    public bool returning = false;
    public bool stepReset = true;
    public bool negate = false; 
    Vector3 origin;
    void Start()
    {
        z = transform.position.z;
        orY = transform.position.y;
        y = transform.position.y;
        orX = transform.position.x;
        x = transform.position.x;
        origin = new Vector3(orX, orY, z);
       
        //dest = transform.position = new Vector3(Random.Range(-range, range), Random.Range(-range, range), z);
    }

    void Update()
    {

        if ((Vector3.Distance(transform.position, origin) <= 0.4f) && stepReset == true)
        {
            resetStep();

        }

        
        if ((Vector3.Distance(transform.position, origin) >= gap) && timer > timing)
        {
            // dest = transform.position = new Vector3(Random.Range(-range, range),  Random.Range(-range, range), z);
            returning = true;
        }
        if (returning == true)
        {

           
            stepX = -stepX;
            stepY = -stepY;
            returning = false;
            timer = 0;

        }
        if ((Vector3.Distance(transform.position, origin) <= resetgap) && timer > timing)
        {
            //returning = false;
            timer = 0;
            stepReset = true;
        }
        timer += Time.deltaTime;
        x += stepX;
        y += stepY;
        transform.position = new Vector3(x, y, z);

        void resetStep(){

           stepX = speed * Time.deltaTime * Random.Range(-1f, 1f);
             stepY = speed * Time.deltaTime * Random.Range(-1f, 1f);
            stepReset = false;
        }
    }
}
