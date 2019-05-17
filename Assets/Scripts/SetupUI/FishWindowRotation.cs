using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishWindowRotation : MonoBehaviour
{
    private Quaternion originalLocalRot;

    private float _verticalEuler;
    public float verticalEuler
    {
        get
        {
            return _verticalEuler;
        }
        set
        {
            _verticalEuler = value * 180;
            UpdateRotation();
        }
    }

    private float _horizontalEuler;
    public float horizontalEuler
    {
        get
        {
            return _horizontalEuler;
        }
        set
        {
            _horizontalEuler = value * 180;
            UpdateRotation();
        }
    }
    
    void Start()
    {
        originalLocalRot = transform.localRotation;
    }

    void UpdateRotation()
    {
        transform.localRotation = Quaternion.AngleAxis(-_horizontalEuler, Vector3.up) * Quaternion.AngleAxis(-_verticalEuler, Vector3.right);
    }
}
