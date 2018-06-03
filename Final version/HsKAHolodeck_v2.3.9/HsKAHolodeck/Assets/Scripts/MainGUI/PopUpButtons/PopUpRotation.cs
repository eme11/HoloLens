using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using UnityEngine.UI;
using System;

public class PopUpRotation : MonoBehaviour,IInputClickHandler {


    public GameObject parent;
    public Text buttonText;
    public Text WatchText;

    void Start () {
        parent = transform.root.gameObject;
        buttonText = GameObject.Find("RotateText").GetComponent<Text>();
    }

    public void OnInputClicked(InputEventData eventData)
    {
        //if the local WatchMe is OFF, Rotation turns ON/OFF
        if (parent.GetComponent<FaceBehavior>().ReturnPressedWatchState() == false)
        {
            parent.GetComponent<FaceBehavior>().ChangePressedRotateState();
            parent.GetComponent<FaceBehavior>().ChangePopUpRotateText();
            
        }
        //if the local WatchMe is ON, we turn it OFF, and turn ON Rotation
        else
        {
            if(parent.GetComponent<FaceBehavior>().ReturnPressedRotateState() == false)
            {
                parent.GetComponent<FaceBehavior>().ChangePressedWatchState();
                parent.GetComponent<FaceBehavior>().ChangePopUpWatchText();
                parent.GetComponent<FaceBehavior>().ChangePressedRotateState();
                parent.GetComponent<FaceBehavior>().ChangePopUpRotateText();
            }
        }

    }

}
