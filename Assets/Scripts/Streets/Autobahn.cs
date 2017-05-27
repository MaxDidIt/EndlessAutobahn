﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Autobahn : MonoBehaviour
{
    private List<Vector3> primaryPoints = new List<Vector3>();
    private List<AutobahnSegment> autobahnSegments = new List<AutobahnSegment>();

    [SerializeField]
    private float chanceOfCurve = 0.2f;

    [SerializeField]
    private float minSegmentDistance = 1000;
    [SerializeField]
    private float maxSegmentDistance = 1500;

    [SerializeField]
    private float maxRotation = 40;

    [SerializeField]
    private Car viewerCar;

    private void Start()
    {
        // Create initial primary point
        primaryPoints.Add(Vector3.zero);

        // Create initial direction
        float angle = UnityEngine.Random.Range(0, Mathf.PI * 2f);
        float distance = UnityEngine.Random.Range(minSegmentDistance, maxSegmentDistance);
        primaryPoints.Add(new Vector3(Mathf.Sin(angle) * distance, 0, Mathf.Cos(angle) * distance));

        for (int i = 0; i < 20; i++)
        {
            CreateNewPrimaryPoint();
        }

        for(int i = 1; i < primaryPoints.Count - 1; i++)
        {
            CreateNewAutobahnSegment(primaryPoints[i - 1], primaryPoints[i], primaryPoints[i + 1]);
        }
    }

    private void CreateNewAutobahnSegment(Vector3 primaryPointA, Vector3 primaryPointB, Vector3 primaryPointC)
    {
        GameObject segmentGO = new GameObject();
        segmentGO.name = "AutobahnSegment " + primaryPointB.ToString();

        AutobahnSegment segment = segmentGO.AddComponent<AutobahnSegment>();
        segment.SetData(primaryPointA, primaryPointB, primaryPointC);
        autobahnSegments.Add(segment);
    }

    private void CreateNewPrimaryPoint()
    {
        Vector3 previousDirection = primaryPoints[primaryPoints.Count - 1] - primaryPoints[primaryPoints.Count - 2];
        Vector3 nextDirection = previousDirection;

        if (UnityEngine.Random.Range(0, 1) <= chanceOfCurve)
        {
            Quaternion rotation = Quaternion.Euler(0, UnityEngine.Random.Range(-maxRotation, maxRotation), 0);
            nextDirection = rotation * previousDirection;
        }

        float distance = UnityEngine.Random.Range(minSegmentDistance, maxSegmentDistance);
        nextDirection.Normalize();

        primaryPoints.Add(primaryPoints[primaryPoints.Count - 1] + nextDirection * distance);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (Vector3 primaryPoint in primaryPoints)
        {
            Gizmos.DrawWireSphere(primaryPoint, 0.1f);
        }
    }
}
