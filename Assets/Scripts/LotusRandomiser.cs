using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LotusRandomiser : MonoBehaviour
{
    public Rigidbody rBody;
    public float speedFactor = 0.5f;
    public float rotationFactor = 10;
    public float screenXRight = 12;
    public float screenXLeft = -12;
    public float screenYTop = 10;
    public float screenYBottom = -10;
    

    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        rBody.useGravity = false;

        SetMotion();
    }

    private void Update()
    {
        if(transform.position.x < screenXLeft)
        {
            Vector3 newPos = transform.position;
            newPos.x = screenXRight;

            if(transform.position.y > screenYTop)
            {
                newPos.y = screenYBottom;
            }

            if (transform.position.y < screenYBottom)
            {
                newPos.y = screenYTop;
            }

            transform.position = newPos;
        }
    }

    [ContextMenu("SetMotion")]
    public void SetMotion()
    {
        Debug.Log("SET MOTION");
        Vector3 direction = new Vector3(Random.Range(-1.0f, 0), Random.Range(-0.15f, 0.15f), 0);
        rBody.velocity = direction * speedFactor;

        rBody.AddTorque(new Vector3(0,0,Random.Range(-rotationFactor, rotationFactor)) );
    }
}
