using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using System;


public class FaceBehavior : MonoBehaviour, IInputClickHandler,IFocusable
{

    [Tooltip("Place parent on tap instead of current game object.")]
    public bool PlaceParentOnTap;

    [Tooltip("Specify the parent game object to be moved on tap, if the immediate parent is not desired.")]
    public GameObject ParentGameObjectToPlace;

    /// <summary>
    /// Keeps track of if the user is moving the object or not.
    /// Setting this to true will enable the user to move and place the object in the scene.
    /// Useful when you want to place an object immediately.
    /// </summary>
    [Tooltip("Setting this to true will enable the user to move and place the object in the scene without needing to tap on the object. Useful when you want to place an object immediately.")]
    public bool IsBeingPlaced;

   /// public GameObject PopUpMenu;
    


    /// <summary>
    /// Controls spatial mapping.  In this script we access spatialMappingManager
    /// to control rendering and to access the physics layer mask.
    /// </summary>
    protected SpatialMappingManager spatialMappingManager;
    public float CursorDimension;
    public float RotationsPerSecond = 0.1f;
    public Color DeleteMode = new Color(1f, 0f, 0f, 0.5f);
    public Color TransparentMode = new Color(1f, 1f, 1f, 1f);
    public float TargetFrameRate = 60;
    
    public GameObject Menu;
    public bool showPopUp;
    private bool isPressedRotate;
    private bool isPressedWatchMe;
    public Text RotateText;
    public Text WatchMeText;
    public Text SpeedText;
    public Text CurrentSpeed;
    public float PopUpRotationPerSecond=0.1f;
    

    public Vector3 AngleIncrement;
    private Material[] defaultMaterials;
    public bool TempLocalRotateState;

    //speed of the WatchMe
    public float WatchMeSpeed = 2.0f;
    public bool TempLocalWatchMeState;
    private Vector3 defaultPosition;
    private bool WasPreviousPlaced;
    

   public void Awake()
    {

    }

    protected virtual void Start()
    {
        showPopUp = SimpleFaceManager.Instance.GetShowPopUps();
        Menu.SetActive(false);
        TempLocalRotateState = true;
        isPressedRotate = false;
        isPressedWatchMe = false;
        spatialMappingManager = SpatialMappingManager.Instance;
        if (spatialMappingManager == null)
        {
            Debug.LogError("This script expects that you have a SpatialMappingManager component in your scene.");
        }


        if (PlaceParentOnTap)
        {
            if (ParentGameObjectToPlace != null && !gameObject.transform.IsChildOf(ParentGameObjectToPlace.transform))
            {
                Debug.LogError("The specified parent object is not a parent of this object.");
            }

            DetermineParent();
        }


    }

    protected virtual void Update()
    {
        // If the user is in placing mode,
        // update the placement to match the user's gaze.
        if (IsBeingPlaced)
        {
            WasPreviousPlaced = true;
            spatialMappingManager.DrawVisualMeshes = true;
            // Do a raycast into the world that will only hit the Spatial Mapping mesh.
            Vector3 headPosition = Camera.main.transform.position;
            Vector3 gazeDirection = Camera.main.transform.forward;

            RaycastHit hitInfo;
            if (Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f, spatialMappingManager.LayerMask))
            {
                // Rotate this object to face the user.
                Quaternion toQuat = Camera.main.transform.rotation;
                //     toQuat.x = 0;
                //    toQuat.z = 0;
                toQuat.eulerAngles = new Vector3(0, toQuat.eulerAngles.y + 180, 0);
                // Move this object to where the raycast
                // hit the Spatial Mapping mesh.
                // Here is where you might consider adding intelligence
                // to how the object is placed.  For example, consider
                // placing based on the bottom of the object's
                // collider so it sits properly on surfaces.
                if (PlaceParentOnTap)
                {

                    // Place the parent object as well but keep the focus on the current game object
                    Vector3 currentMovement = hitInfo.point - gameObject.transform.position;
                    ParentGameObjectToPlace.transform.position += currentMovement;
                    //  ParentGameObjectToPlace.transform.rotation = toQuat;
                }
                else
                {
                    gameObject.transform.position = hitInfo.point;
                    gameObject.transform.position += new Vector3(0.0f, (((this.GetComponent<BoxCollider>().size.y) / 2) * gameObject.transform.lossyScale.y) - CursorDimension, 0.0f);

                    gameObject.transform.rotation = toQuat;
                }
            }
        }
        else
        {
            //Global rotation
            if (SimpleFaceManager.Instance.GetGlobalRotateState() == true && TempLocalRotateState == true)
            {
                CheckPopUpButtons();
                AngleIncrement = new Vector3(0f, 1f, 0f) * ((360 * RotationsPerSecond) / TargetFrameRate);
                gameObject.transform.Rotate(AngleIncrement);
            }
            //Local rotation
            else if (isPressedRotate == true)
            {
                AngleIncrement = new Vector3(0f, 1f, 0f) * ((360 * PopUpRotationPerSecond) / TargetFrameRate);
                gameObject.transform.Rotate(AngleIncrement);

            }
            //Global watchMe
            if (SimpleFaceManager.Instance.GetGlobalWatchState() == true && TempLocalWatchMeState == true)
            {
                CheckPopUpButtons();
                //rotates the face smoothly towards the camera
                //the smoothing factor is "WatchMeSmooth"
                var lookPos = Camera.main.transform.position - transform.position;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);

                //we use a margin of 2 degrees for WatchMe, to save extra calculations
                //this is due to reasons such as noise, but also performance and battery life
                if (Vector3.Distance(transform.eulerAngles,rotation.eulerAngles)>2)
                    transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * WatchMeSpeed);
            }
            //Local watchMe
            else if (isPressedWatchMe == true)
            {
                var lookPos = Camera.main.transform.position - transform.position;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);

                if (Vector3.Distance(transform.eulerAngles, rotation.eulerAngles) > 2)
                    transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * WatchMeSpeed);
            }
        }
        
    }

    public virtual void OnInputClicked(InputEventData eventData)
    {
        if (CursorManager.Instance.GetDeleteState() == true)
        {
            IsBeingPlaced = true;
            SimpleFaceManager.Instance.ReturnObject(this.gameObject);

        }
        else
        {
            // On each tap gesture, toggle whether the user is in placing mode.
            IsBeingPlaced = !IsBeingPlaced;

            // If the user is in placing mode, display the spatial mapping mesh.
            if (IsBeingPlaced)
            {

                spatialMappingManager.DrawVisualMeshes = true;
                //spatialMappingManager.ChangeSurfaceMaterial(true);
                Debug.Log(gameObject.name + " : Removing existing world anchor if any.");


            }
            // If the user is not in placing mode, hide the spatial mapping mesh.
            else
            {
                //The moving point during placement is the center of the object, in order to place the bottom of the object needs to be placed on the surface the object needs to be liftet by the real life size of the colider minus the dimensions of the cursor.
                //Please note that this method is only accurate if the object is asumed to be a cube and the surface on which the object will be placed is horizontal
                
                spatialMappingManager.DrawVisualMeshes = false;
                //spatialMappingManager.ChangeSurfaceMaterial(false);

            }

        }

    }

    public void GetNewMaterial()
    {
        defaultMaterials = GetComponent<MeshRenderer>().materials;

    }

    public void OnFocusEnter()
    {
        showPopUp = SimpleFaceManager.Instance.GetShowPopUps();
        if (CursorManager.Instance.GetDeleteState() == true)
        {
            TempLocalRotateState = false;
            TempLocalWatchMeState = false;
            for (int i = 0; i < defaultMaterials.Length; i++)
            {
                // Highlight the material when gaze enters using the shader property.
                defaultMaterials[i].SetColor("_Color", DeleteMode);

            }
        }
        //we activate the menu if the object is not in placing mode, and if 
        //popups are enabled from the main menu
        if (IsBeingPlaced == false)
        {
            if (showPopUp == true)
            {
                ActivateMenu();
            }
        }
    }

    public void OnFocusExit()
    {
        if (CursorManager.Instance.GetDeleteState() == true)
        {
            for (int i = 0; i < defaultMaterials.Length; i++)
            {
                // Remove highlight on material when gaze exits.
                defaultMaterials[i].SetColor("_Color", TransparentMode);
            }
            TempLocalRotateState = true;
            TempLocalWatchMeState = true;
        }

    }

    private void DetermineParent()
    {
        if (ParentGameObjectToPlace == null)
        {
            if (gameObject.transform.parent == null)
            {
                Debug.LogError("The selected GameObject has no parent.");
                PlaceParentOnTap = false;
            }
            else
            {
                Debug.LogError("No parent specified. Using immediate parent instead: " + gameObject.transform.parent.gameObject.name);
                ParentGameObjectToPlace = gameObject.transform.parent.gameObject;
            }
        }
    }

    public bool GetIsBeingPlaced()
    {
        return IsBeingPlaced;
    }
  
    private void ActivateMenu()
    {
        Menu.SetActive(true);
    }

    public void ChangePressedRotateState()
    {
        isPressedRotate = !isPressedRotate;
    }
    public bool ReturnPressedRotateState()
    {
        return isPressedRotate;
    }

    public void ChangePressedWatchState()
    {
        isPressedWatchMe = !isPressedWatchMe;
    }

    public bool ReturnPressedWatchState()
    {
        return isPressedWatchMe;
    }

    //turns off the local rotation and watchMe, if the global ones are active
    public void CheckPopUpButtons()
    {
        if (isPressedRotate == true)
        {
            ChangePressedRotateState();
            RotateText.text = "Rotate";
        }
        if (isPressedWatchMe == true)
        {
            ChangePressedWatchState();
            WatchMeText.text = "Watch Me";
        }
    }

    public void ChangePopUpRotateText()
    {
        if (isPressedRotate)
        {
            RotateText.text = "Stop Rotating";
        }
        else
        {
            RotateText.text = "Rotate";
        }
    }

    public void ChangePopUpWatchText()
    {
        if (isPressedWatchMe)
        {
            WatchMeText.text = "Stop watching";
        }
        else
        {
            WatchMeText.text = "Watch Me";
        }
    }

    //Used for selecting and displaying the speed of the local rotation
    public void LocalRotationSpeed()
    {
        switch (SpeedText.text)
        {
            case "Slow":
                PopUpRotationPerSecond = 0.1f;
                SpeedText.text = "Normal";
                CurrentSpeed.text = "Current Speed:\r\nNormal";
                break;
            case "Normal":
                PopUpRotationPerSecond = 0.25f;
                SpeedText.text = "Fast";
                CurrentSpeed.text = "Current Speed:\r\nFast";
                break;
            case "Fast":
                PopUpRotationPerSecond = 0.05f;
                SpeedText.text = "Slow";
                CurrentSpeed.text = "Current Speed:\r\nSlow";
                break;
        }
    }
    
}

