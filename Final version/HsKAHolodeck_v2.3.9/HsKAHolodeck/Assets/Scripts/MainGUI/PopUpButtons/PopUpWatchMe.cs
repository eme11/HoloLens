using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using UnityEngine.UI;

public class PopUpWatchMe : MonoBehaviour,IInputClickHandler {

    public GameObject parent;
    void Start()
    {
        parent = transform.root.gameObject;
    }
    public void OnInputClicked(InputEventData eventData)
    {
        //if the local Rotation is OFF, WatchMe turns ON/OFF
        if (parent.GetComponent<FaceBehavior>().ReturnPressedRotateState() == false)
        {
            parent.GetComponent<FaceBehavior>().ChangePressedWatchState();
            parent.GetComponent<FaceBehavior>().ChangePopUpWatchText();
        }
        //if the local Rotation is ON, we turn it OFF, and turn ON WatchMe
        else
        {
            if(parent.GetComponent<FaceBehavior>().ReturnPressedWatchState() == false)
            {
                parent.GetComponent<FaceBehavior>().ChangePressedRotateState();
                parent.GetComponent<FaceBehavior>().ChangePopUpRotateText();
                parent.GetComponent<FaceBehavior>().ChangePressedWatchState();
                parent.GetComponent<FaceBehavior>().ChangePopUpWatchText();
            }
        }
    }
}
