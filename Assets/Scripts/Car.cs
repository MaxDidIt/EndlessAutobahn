using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    private const float KMH_TO_MS = 0.277778f;

    [SerializeField]
    private float targetSpeed = 80;

    [SerializeField]
    private bool isViewer = false;

    private float currentT;
    private AutobahnSegment currentAutobahnSegment;

    public AutobahnSegment CurrentAutobahnSegment
    {
        get
        {
            return currentAutobahnSegment;
        }
    }

    public float CurrentT
    {
        get
        {
            return currentT;
        }
    }

    public void Move(Autobahn autobahn)
    {
        Vector3 position = currentAutobahnSegment.CalculatePositionAt(currentT);
        Vector3 derivative = currentAutobahnSegment.CalculateDerivativeAt(currentT);

        Vector3 normal = Quaternion.Euler(0, 90, 0) * derivative;
        normal.y = 0;
        normal.Normalize();

        transform.position = position + (isViewer ? Vector3.zero : (normal * 3.75f)) + currentAutobahnSegment.transform.position;
        transform.rotation = Quaternion.LookRotation(derivative, Vector3.up);

        currentT += (1f / derivative.magnitude) * Time.fixedDeltaTime * (targetSpeed * KMH_TO_MS);
        if (currentT > 1)
        {
            currentT -= 1;

            currentAutobahnSegment = autobahn.RequestNextAutobahnSegment(currentAutobahnSegment, isViewer);
        }
    }

    public void SetData(AutobahnSegment autobahnSegment, float t)
    {
        currentAutobahnSegment = autobahnSegment;
        currentT = t;
    }
}
