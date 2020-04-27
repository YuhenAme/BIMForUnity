using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CutController : MonoBehaviour
{
    private bool isCutOn = false;

    private GameObject planeX;
    private GameObject planeY;
    private GameObject planeZ;
    private GameObject[] gameObjects;

    private void Start()
    {
        planeZ = GameObject.Find("PlaneCut").transform.GetChild(0).gameObject;
        planeY = GameObject.Find("PlaneCut").transform.GetChild(1).gameObject;
        planeX = GameObject.Find("PlaneCut").transform.GetChild(2).gameObject;

        planeZ.SetActive(false);
        planeY.SetActive(false);
        planeX.SetActive(false);
    }

    public void CutButtonDown()
    {
        if(isCutOn == false)
        {
            
            CutOn();
            isCutOn = true;
        }
        else if(isCutOn == true)
        {
            
            CutOff();

            isCutOn = false;
        }
    }

    private void CutOn()
    {
        planeZ.SetActive(true);
        planeY.SetActive(true);
        planeX.SetActive(true);
        gameObjects = GameObject.FindGameObjectsWithTag("ShapeInstance");
        foreach(GameObject go in gameObjects)
        {
            Color color = go.GetComponent<MeshRenderer>().material.color;
            Material material = new Material(Shader.Find("CrossSection/OnePlaneBSP"));
            material.color = color;

            if (color.a < 0.9f)
            {
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.renderQueue = 3000;
            }
            else
            {
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.renderQueue = -1;
            }
            go.GetComponent<MeshRenderer>().material = material;
            go.AddComponent<PlaneCuttingController>();
        }
    }
    private void CutOff()
    {
        planeZ.SetActive(false);
        planeY.SetActive(false);
        planeX.SetActive(false);
        gameObjects = GameObject.FindGameObjectsWithTag("ShapeInstance");
        foreach(GameObject go in gameObjects)
        {
            Color color = go.GetComponent<MeshRenderer>().material.color;
            Material material = new Material(Shader.Find("Unlit/NormalRender"));
            material.color = color;
            if (color.a < 0.9f)
            {
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.renderQueue = 3000;
            }
            else
            {
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.renderQueue = -1;
            }
            go.GetComponent<MeshRenderer>().material = material;
            Destroy(go.GetComponent<PlaneCuttingController>());
        }

    }

    public void XAxisChange(float value)
    {
        planeX.transform.position = new Vector3(value, planeX.transform.position.y, planeX.transform.position.z);
    }
    public void YAxisChange(float value)
    {
        planeY.transform.position = new Vector3(planeY.transform.position.x, planeY.transform.position.y, value);
    }
    public void ZAxisChange(float value)
    {
        planeZ.transform.position = new Vector3(planeZ.transform.position.x, value, planeZ.transform.position.z);
    }
    
}
