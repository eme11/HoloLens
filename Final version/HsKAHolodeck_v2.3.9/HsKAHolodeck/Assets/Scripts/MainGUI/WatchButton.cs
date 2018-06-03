using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity.InputModule;
public class WatchButton : MonoBehaviour, IInputClickHandler
{
    private SimpleFaceManager FaceManager;
    public Text ButtonText;
    public Text RotateButtonText;

    public void Start()
    {
        FaceManager = SimpleFaceManager.Instance;
        if (FaceManager == null)
        {
            Debug.LogError("This script expects that you have a SimpleFaceManager component in your scene.");
        }
    }
    public void OnInputClicked(InputEventData eventData)
    {

        //if Rotate button is OFF, WatchMe turns ON/OFF
        if (FaceManager.GetGlobalRotateState() == false)
        {
            if (FaceManager.GetGlobalWatchState() == true)
            {
                ButtonText.text = "Watch me";
            }
            else
            {
                ButtonText.text = "Stop watching";
            }
            FaceManager.SwitchGlobalWatchState();
        }
        else
        {
            //if Rotate is ON, we turn it OFF, and turn ON WatchMe
            if (FaceManager.GetGlobalWatchState() == false)
            {
                FaceManager.SwitchGlobalRotateState();
                RotateButtonText.text = "Start rotating";
                FaceManager.SwitchGlobalWatchState();
                ButtonText.text = "Stop watching";
            }

        }
    }

}
