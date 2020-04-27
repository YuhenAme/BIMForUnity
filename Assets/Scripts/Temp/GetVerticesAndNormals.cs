using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetVerticesAndNormals : MonoBehaviour
{
    public Vector3[] vertices;
    public Vector3[] normals;
    public int[] triangles;
    
    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        normals = mesh.normals;
        triangles = mesh.triangles;
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
