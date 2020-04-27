using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveDirection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Material mat = GetComponent<Renderer>().material;
        float minX, maxX,minY,maxY,minZ,maxZ;
        CalculateMinMaxX(out minX, out maxX);
        CalculateMinMaxY(out minY, out maxY);
        CalculateMinMaxZ(out minZ, out maxZ);
        Debug.Log(minX);
        mat.SetFloat("_MinBorderX", minX);
        mat.SetFloat("_MaxBorderX", maxX);
        mat.SetFloat("_MinBorderY", minY);
        mat.SetFloat("_MaxBorderY", maxY);
        mat.SetFloat("_MinBorderZ", minZ);
        mat.SetFloat("_MaxBorderZ", maxZ);
    }
    //计算边界
    void CalculateMinMaxX(out float minX, out float maxX)
    {
        Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;
        minX = maxX = vertices[0].x;
        for (int i = 1; i < vertices.Length; i++)
        {
            float x = vertices[i].x;
            if (x < minX)
                minX = x;
            if (x > maxX)
                maxX = x;
        }
    }
    void CalculateMinMaxY(out float minY, out float maxY)
    {
        Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;
        minY = maxY = vertices[0].y;
        for (int i = 1; i < vertices.Length; i++)
        {
            float y = vertices[i].y;
            if (y < minY)
                minY = y;
            if (y > maxY)
                maxY = y;
        }
    }
    void CalculateMinMaxZ(out float minZ, out float maxZ)
    {
        Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;
        minZ = maxZ = vertices[0].z;
        for (int i = 1; i < vertices.Length; i++)
        {
            float z = vertices[i].z;
            if (z < minZ)
                minZ = z;
            if (z > maxZ)
                maxZ = z;
        }
    }
}
