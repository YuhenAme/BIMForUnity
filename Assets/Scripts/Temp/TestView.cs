using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestView : MonoBehaviour
{
    public float mouseScrollSpeed = 40; //滚滚轮放大缩小速度
    public float mouseRotateSpeed = 10; //单单按右键旋转的速度
    public float mouseDragSpeed = 1.26f;//按中键移动的速度

   
    private Vector3 rotateDelta = new Vector3(0, 0, 0);

    private void FixedUpdate()
    {
        MouseEvents();
    }

    void MouseEvents()
    {
        //按住鼠标右键旋转
        if (Input.GetMouseButton(1))
        {
            rotateDelta = new Vector3(-Input.GetAxis("Mouse Y") * mouseRotateSpeed, Input.GetAxis("Mouse X") * mouseRotateSpeed, 0);
            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            {
                this.transform.Translate(new Vector3(0, 0, Time.deltaTime * mouseScrollSpeed * (rotateDelta.x + rotateDelta.y)), Space.Self);
            }
            else
            {
                transform.eulerAngles += rotateDelta;
            }

        }
        //中键拖动
        else if (Input.GetMouseButton(2))
        {
            rotateDelta = new Vector3(-Input.GetAxis("Mouse X") * mouseDragSpeed, -Input.GetAxis("Mouse Y") * mouseDragSpeed, 0);
            transform.Translate(rotateDelta, Space.Self);
        }
        
        //缩放
        if(Input.mouseScrollDelta.y != 0)
        {
            transform.Translate(new Vector3(0, 0, Time.deltaTime * mouseScrollSpeed * Input.mouseScrollDelta.y), Space.Self);
        }

    }
}
