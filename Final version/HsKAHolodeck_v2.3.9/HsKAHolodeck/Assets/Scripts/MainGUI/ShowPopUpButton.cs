using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity.InputModule;
using System;

public class ShowPopUpButton : MonoBehaviour ,IInputClickHandler{

    public Text buttonText;

    //on click, we toggle whether or not faces will show their Popup Menus
    public void OnInputClicked(InputEventData eventData)
    {
        SimpleFaceManager.Instance.SwitchShowPopUps();
        if (SimpleFaceManager.Instance.GetShowPopUps())
        {
            buttonText.text = "Pop Up: ON";
            
        }
        else
        {
            buttonText.text = "Pop Up: OFF";
        }
    }

}
