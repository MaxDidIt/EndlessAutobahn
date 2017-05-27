using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField]
    private float targetSpeed;

    public float Speed
    {
        get
        {
            return targetSpeed;
        }
    }
}
