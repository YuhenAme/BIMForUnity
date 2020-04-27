using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveOrFalseEvent : MonoBehaviour
{
    public void Invoke()
    {
        if(gameObject.activeSelf == true)
        {
            gameObject.SetActive(false);
        }
        else if(gameObject.activeSelf == false)
        {
            gameObject.SetActive(true);
        }
    }
}
