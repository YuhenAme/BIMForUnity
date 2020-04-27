using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapSyncPostion : MonoBehaviour
{
    private GameObject mainCamera;

    private void Start()
    {
        mainCamera = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {
        float x = mainCamera.transform.position.x;
        float z = mainCamera.transform.position.z;
        gameObject.transform.position = new Vector3(x, transform.position.y, z);
    }
}
