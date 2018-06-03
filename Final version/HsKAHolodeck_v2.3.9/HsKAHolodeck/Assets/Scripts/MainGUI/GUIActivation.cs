using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using UnityEngine.VR.WSA.Input;
using UnityEngine.UI;

public class GUIActivation : MonoBehaviour
{
    GestureRecognizer recognizer;
    public GameObject MainGUI;
    private bool IsActive = false;
    public GameObject FaceContentPanel;
    public GameObject LoadingScreen;
    public GameObject MainMenu;
    // Use this for initialization
    void Start()
    {

        recognizer = new GestureRecognizer();

        recognizer.TappedEvent += RecognizerGUI_TappedEvent;

        recognizer.StartCapturingGestures();

        MainGUI.SetActive(IsActive);


    }

    private void RecognizerGUI_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        RaycastHit TempHit=  GazeManager.Instance.HitInfo;
        if (TempHit.collider != null)
        {
            if (TempHit.collider.gameObject.layer == 31)
            {

                IsActive = !IsActive;
                if (IsActive == true)
                {
                    MainGUI.transform.position = Camera.main.transform.position + 3 * Camera.main.transform.forward;
                }
                MainGUI.SetActive(IsActive);


            }
        }
        
    }
    public void LoadingScreenFace(bool Loading)
    {
        if(Loading)
        {
            LoadingScreen.SetActive(true);
        }
        else
        {
            LoadingScreen.SetActive(false);
        }

    }
    public void DeactivateGUI()
    {
        this.IsActive = false;
        this.gameObject.SetActive(IsActive);

    }

}

