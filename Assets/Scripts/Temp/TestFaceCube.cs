using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFaceCube : MonoBehaviour
{
    public GameObject go;
    public Vector3[] vertices =
    {
        new Vector3(0,0,0),
        new Vector3(1,0,0),
        new Vector3(1,1,0),
        new Vector3(0,1,0),
        new Vector3(0,1,1),
        new Vector3(1,1,1),
        new Vector3(1,0,1),
        new Vector3(0,0,1),
    };

    public int[] triangles =
    {
        0,2,1,
        0,3,2,

        2,3,4,
        2,4,5,

        1,2,5,
        1,5,6,

        0,7,4,
        0,4,3,

        5,4,7,
        5,7,6,

        0,6,7,
        0,1,6,
    };

    //public Vector3[] verticesAfter;
    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Material material = new Material(Shader.Find("Standard"));
        GetComponent<Renderer>().material = material;
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.Optimize();
        mesh.RecalculateNormals();
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            vertices = go.GetComponent<GetVerticesAndNormals>().vertices;
            triangles = go.GetComponent<GetVerticesAndNormals>().triangles;

            Mesh mesh = GetComponent<MeshFilter>().mesh;
            Material material = new Material(Shader.Find("Standard"));
            GetComponent<Renderer>().material = material;
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.Optimize();
            mesh.RecalculateNormals();
        }
    }
}
