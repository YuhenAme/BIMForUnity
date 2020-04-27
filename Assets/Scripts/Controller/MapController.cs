using System.Collections;
using System.Collections.Generic;
using Xbim.Ifc4.Interfaces;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<GameObject> floorObjects;
    public bool isMapOn = false;
    private GameObject mainCamera;
    private List<float> floorElevations;


    private void Start()
    {
        floorObjects = new List<GameObject>();
        floorElevations = new List<float>();
        mainCamera = GameObject.Find("Main Camera");
    }

    private void SetFloorMap()
    {
        var objectDatas = GameObject.Find(SpatialStructureTest.project.Name).transform.GetComponentsInChildren<BuildingStoreyData>();
        
        foreach (var o in objectDatas)
        {
            floorObjects.Add(o.TheGameObject);
            IIfcBuildingStorey s = o.TheStructure as IIfcBuildingStorey;
            
            floorElevations.Add((float)s.Elevation);
        }

    }

    public void SetSize(float size)
    {
       GameObject.Find("MiniMapCamera").GetComponent<Camera>().orthographicSize = size;
    }
    
    public void SetMap()
    {
        if(isMapOn == false)
        {
            isMapOn = true;
            SetFloorMap();
        }
        else if(isMapOn == true)
        {
            isMapOn = false;
        }
    }

    private void Update()
    {
        if(isMapOn == true)
        {
            if (floorElevations.Count > 0)
            {
                for (int i = 0; i < floorElevations.Count; i++)
                {
                    if (i < floorElevations.Count - 1)
                    {
                        if (mainCamera.transform.position.y > floorElevations[i] && mainCamera.transform.position.y < floorElevations[i + 1])
                        {
                            Debug.Log("当前相机处于" + floorObjects[i].name);
                           
                            foreach(var tran in floorObjects[i].GetComponentsInChildren<Transform>())
                            {
                                tran.gameObject.layer = LayerMask.NameToLayer("CurrentFloor");
                            }
                           
                        }
                        else
                        {
                            foreach (var tran in floorObjects[i].GetComponentsInChildren<Transform>())
                            {
                                tran.gameObject.layer = LayerMask.NameToLayer("Default");
                            }
                        }
                    }
                    else if(i == floorElevations.Count - 1)
                    {
                        if(mainCamera.transform.position.y > floorElevations[i])
                        {
                            Debug.Log("当前相机处于" + floorObjects[i].name);
                            foreach (var tran in floorObjects[i].GetComponentsInChildren<Transform>())
                            {
                                tran.gameObject.layer = LayerMask.NameToLayer("CurrentFloor");
                            }
                        }
                        else
                        {
                            foreach (var tran in floorObjects[i].GetComponentsInChildren<Transform>())
                            {
                                tran.gameObject.layer = LayerMask.NameToLayer("Default");
                            }
                        }
                    }
                }
            }
        }
    }
}
