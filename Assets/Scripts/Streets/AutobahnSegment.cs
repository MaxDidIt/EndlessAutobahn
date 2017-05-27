using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class AutobahnSegment : MonoBehaviour
{
    private const int PREVIEW_PRECISION = 10;
    private const float MESH_PRECISION = 1;
    private const int MESH_SUBDIVISIONS = 8;
    private const float STREET_WIDTH = 3.75f * 3;

    private static List<Vector3> meshPositions = new List<Vector3>();
    private static List<Vector3> meshNormals = new List<Vector3>();
    private static List<Vector2> meshUVs = new List<Vector2>();
    private static List<int> meshIndices = new List<int>();
    
    private Vector3 primaryPointB;

    private Vector3 secondaryPointA;
    private Vector3 secondaryPointB;

    public void SetData(Vector3 primaryPointA, Vector3 primaryPointB, Vector3 primaryPointC)
    {
        this.primaryPointB = Vector3.zero;

        transform.position = primaryPointB;

        secondaryPointA = (primaryPointA - primaryPointB) * 0.5f;
        secondaryPointB = (primaryPointC - primaryPointB) * 0.5f;

        GenerateMesh();
        GetComponent<MeshRenderer>().sharedMaterial = Resources.Load<Material>("Pavement");
        GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(1, 12);
    }

    private void GenerateMesh()
    {
        meshPositions.Clear();
        meshNormals.Clear();
        meshUVs.Clear();
        meshIndices.Clear();

        float t = 0;

        Vector3 position;
        Vector3 derivative;
        Vector3 normal;

        do
        {
            position = CalculatePositionAt(t);
            derivative = CalculateDerivativeAt(t);

            normal = Quaternion.Euler(0, 90, 0) * derivative;
            normal.y = 0;
            normal.Normalize();

            for(int i = 0; i < MESH_SUBDIVISIONS; i++)
            {
                float u = (float)i / (MESH_SUBDIVISIONS - 1);
                float uPosition = u * STREET_WIDTH - STREET_WIDTH / 2f;

                meshPositions.Add(position + normal * uPosition);
                meshNormals.Add(Vector3.up);
                meshUVs.Add(new Vector2(u, t));
            }

            t += (1 / derivative.magnitude) * MESH_PRECISION;
        } while (t < 1);

        position = CalculatePositionAt(1);
        derivative = CalculateDerivativeAt(1);

        normal = Quaternion.Euler(0, 90, 0) * derivative;
        normal.y = 0;
        normal.Normalize();

        for (int i = 0; i < MESH_SUBDIVISIONS; i++)
        {
            float u = (float)i / (MESH_SUBDIVISIONS - 1);
            float uPosition = u * STREET_WIDTH - STREET_WIDTH / 2f;

            meshPositions.Add(position + normal * uPosition);
            meshUVs.Add(new Vector2(u, t));
            meshNormals.Add(Vector3.up);
        }

        int rows = meshPositions.Count / MESH_SUBDIVISIONS;
        for(int row = 0; row < rows - 1; row++)
        {
            int rowIndex = row * MESH_SUBDIVISIONS;
            int nextRowIndex = rowIndex + MESH_SUBDIVISIONS;

            for (int column = 0; column < MESH_SUBDIVISIONS - 1; column++)
            {
                meshIndices.Add(rowIndex + column);
                meshIndices.Add(nextRowIndex + column);
                meshIndices.Add(rowIndex + column + 1);

                meshIndices.Add(rowIndex + column + 1);
                meshIndices.Add(nextRowIndex + column);
                meshIndices.Add(nextRowIndex + column + 1);
            }
        }

        Mesh mesh = new Mesh();
        mesh.SetVertices(meshPositions);
        mesh.SetNormals(meshNormals);
        mesh.SetUVs(0, meshUVs);
        mesh.SetIndices(meshIndices.ToArray(), MeshTopology.Triangles, 0);

        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(secondaryPointA + transform.position, 0.075f);
        Gizmos.DrawWireSphere(secondaryPointB + transform.position, 0.075f);

        Gizmos.color = Color.magenta;
        for(int i = 1; i <= PREVIEW_PRECISION; i++)
        {
            Vector3 positionA = CalculatePositionAt((float)(i - 1) / (float)PREVIEW_PRECISION);
            Vector3 positionB = CalculatePositionAt((float)i / (float)PREVIEW_PRECISION);

            Gizmos.DrawLine(positionA + transform.position, positionB + transform.position);
        }
    }

    private Vector3 CalculatePositionAt(float t)
    {
        return SplineMath.BezierSplineWeight(0, 2, t) * secondaryPointA +
               SplineMath.BezierSplineWeight(1, 2, t) * primaryPointB +
               SplineMath.BezierSplineWeight(2, 2, t) * secondaryPointB;
    }

    private Vector3 CalculateDerivativeAt(float t)
    {
        return SplineMath.BezierSplineWeight(0, 1, t) * (primaryPointB - secondaryPointA) +
               SplineMath.BezierSplineWeight(1, 1, t) * (secondaryPointB - primaryPointB);
    }
}
