using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class PopUpSpeed : MonoBehaviour, IInputClickHandler
{

    public GameObject parent;
    void Start () {
        parent = transform.root.gameObject;
    }

    public void OnInputClicked(InputEventData eventData)
    {
        //on click, we cycle through the speeds of local Rotation
        parent.GetComponent<FaceBehavior>().LocalRotationSpeed();
    }
}
