using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneCuttingController : MonoBehaviour
{
    public GameObject planeX;
    public GameObject planeY;
    public GameObject planeZ;
    Material mat;
    public Vector3 normalX;
    public Vector3 positionX;
    public Vector3 normalY;
    public Vector3 positionY;
    public Vector3 normalZ;
    public Vector3 positionZ;
    public Renderer rend;
    // Use this for initialization
    void Start()
    {
        planeX = GameObject.Find("QuadX");
        planeY = GameObject.Find("QuadY");
        planeZ = GameObject.Find("QuadZ");
        rend = GetComponent<Renderer>();
        normalX = planeX.transform.TransformVector(new Vector3(0, 0, -1));
        positionX = planeX.transform.position;
        normalY = planeY.transform.TransformVector(new Vector3(0, 0, -1));
        positionY = planeY.transform.position;
        normalZ = planeZ.transform.TransformVector(new Vector3(0, 0, -1));
        positionZ = planeZ.transform.position;
        UpdateShaderProperties();
    }
    void Update()
    {
        UpdateShaderProperties();
    }

    private void UpdateShaderProperties()
    {

        normalX = planeX.transform.TransformVector(new Vector3(0, 0, -1));
        positionX = planeX.transform.position;
        normalY = planeY.transform.TransformVector(new Vector3(0, 0, -1));
        positionY = planeY.transform.position;
        normalZ = planeZ.transform.TransformVector(new Vector3(0, 0, -1));
        positionZ = planeZ.transform.position;
        for (int i = 0; i < rend.materials.Length; i++)
        {
            if (rend.materials[i].shader.name == "CrossSection/OnePlaneBSP")
            {
                rend.materials[i].SetVector("_PlaneNormalX", normalX);
                rend.materials[i].SetVector("_PlanePositionX", positionX);
                rend.materials[i].SetVector("_PlaneNormalY", normalY);
                rend.materials[i].SetVector("_PlanePositionY", positionY);
                rend.materials[i].SetVector("_PlaneNormalZ", normalZ);
                rend.materials[i].SetVector("_PlanePositionZ", positionZ);
            }
        }

    }
}
