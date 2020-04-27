using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialController : MonoBehaviour
{
    public enum RenderMode
    {
        Normal,
        Wireframe,
        QuadWireframe
    }

    private GameObject[] gameObjects;

    private void Start()
    {
        gameObjects = GameObject.FindGameObjectsWithTag("ShapeInstance");
    }


    private RenderMode renderMode = RenderMode.Normal;

    private void MaterialRendering()
    {

        switch (renderMode)
        {
            case RenderMode.Normal:
                NormalRendering();
                break;
            case RenderMode.Wireframe:
                WireframeRendering();
                break;
            case RenderMode.QuadWireframe:
                QuadWireframeRendering();
                break;
        }
    }

    private void NormalRendering()
    {
        gameObjects = GameObject.FindGameObjectsWithTag("ShapeInstance");
        foreach (GameObject go in gameObjects)
        {
            if (go.GetComponent<NewWireframeRenderer>() != null)
                Destroy(go.GetComponent<NewWireframeRenderer>());
        }
    }

    private void WireframeRendering()
    {
        gameObjects = GameObject.FindGameObjectsWithTag("ShapeInstance");
        foreach (GameObject go in gameObjects)
        {
            if(go.GetComponent<NewWireframeRenderer>() == null)
            {
                go.AddComponent<NewWireframeRenderer>();
                go.transform.GetChild(0).GetComponent<Renderer>().material.DisableKeyword("ENABLE_DRAWQUAD");
            }
            else
            {
                go.transform.GetChild(0).GetComponent<Renderer>().material.DisableKeyword("ENABLE_DRAWQUAD");
            }
                
        }
    }

    private void QuadWireframeRendering()
    {
        gameObjects = GameObject.FindGameObjectsWithTag("ShapeInstance");
        foreach (GameObject go in gameObjects)
        {
            if (go.GetComponent<NewWireframeRenderer>() == null)
            {
                go.AddComponent<NewWireframeRenderer>();
                go.transform.GetChild(0).GetComponent<Renderer>().material.EnableKeyword("ENABLE_DRAWQUAD");
            }
            else
            {
                go.transform.GetChild(0).GetComponent<Renderer>().material.EnableKeyword("ENABLE_DRAWQUAD");
            }
            
        }
        
    }

    public void SetNormalRendering()
    {
        renderMode = RenderMode.Normal;
        MaterialRendering();
    }
    public void SetWireframeRendering()
    {
        renderMode = RenderMode.Wireframe;
        MaterialRendering();
    }
    public void SetQuadWireframeRendering()
    {
        renderMode = RenderMode.QuadWireframe;
        MaterialRendering();
    }
}
