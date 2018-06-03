using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity.InputModule;
public class RotateButton : MonoBehaviour, IInputClickHandler
{
    private SimpleFaceManager FaceManager;
    public Text ButtonText;
    public Text WatchButtonText;

    public void Start()
    {
        FaceManager = SimpleFaceManager.Instance;
        if (FaceManager==null)
        {
            Debug.LogError("This script expects that you have a SimpleFaceManager component in your scene.");


        }
    }
    public void OnInputClicked(InputEventData eventData)
    {
        //if WatchMe button is OFF, Rotate turns ON/OFF
        if (FaceManager.GetGlobalWatchState() == false)
        {
            if (FaceManager.GetGlobalRotateState() == true)
            {
                ButtonText.text = "Start rotating";
            }
            else
            {
                ButtonText.text = "Stop rotating";
            }
            FaceManager.SwitchGlobalRotateState();

        }
        else
        {
            if (FaceManager.GetGlobalRotateState() == false)
            {
                //if WatchMe is ON, we turn it OFF, and turn ON Rotate
                FaceManager.SwitchGlobalWatchState();
                WatchButtonText.text = "Watch Me";
                FaceManager.SwitchGlobalRotateState();
                ButtonText.text = "Stop rotating";

            }
        }
    }
}
