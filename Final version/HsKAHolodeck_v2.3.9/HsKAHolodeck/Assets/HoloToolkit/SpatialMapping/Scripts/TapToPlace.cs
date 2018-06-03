﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace HoloToolkit.Unity.SpatialMapping
{
    /// <summary>
    /// The TapToPlace class is a basic way to enable users to move objects 
    /// and place them on real world surfaces.
    /// Put this script on the object you want to be able to move. 
    /// Users will be able to tap objects, gaze elsewhere, and perform the
    /// tap gesture again to place.
    /// This script is used in conjunction with GazeManager, GestureManager,
    /// and SpatialMappingManager.
    /// TapToPlace also adds a WorldAnchor component to enable persistence.
    /// </summary>
    
    public class TapToPlace : MonoBehaviour, IInputClickHandler
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



        /// <summary>
        /// Controls spatial mapping.  In this script we access spatialMappingManager
        /// to control rendering and to access the physics layer mask.
        /// </summary>
        protected SpatialMappingManager spatialMappingManager;
        public float CursorDimension;
        public bool Rotate { get; set; }
        public float RotationsPerSecond;
        private float TargetFrameRate = 60;
        private Vector3 AngleIncrement;
        protected virtual void Start()
        {

            AngleIncrement = new Vector3(0f, 1f, 0f) * ((360 * RotationsPerSecond) / TargetFrameRate);

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
            spatialMappingManager.DrawVisualMeshes = true;
        }

        protected virtual void Update()
        {
            // If the user is in placing mode,
            // update the placement to match the user's gaze.
            if (IsBeingPlaced)
            {
                // Do a raycast into the world that will only hit the Spatial Mapping mesh.
                Vector3 headPosition = Camera.main.transform.position;
                Vector3 gazeDirection = Camera.main.transform.forward;

                RaycastHit hitInfo;
                if (Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f, spatialMappingManager.LayerMask))
                {
                    // Rotate this object to face the user.
                    Quaternion toQuat = Camera.main.transform.localRotation;
                    toQuat.x = 0;
                    toQuat.z = 0;
                    toQuat.y = 180;
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
                        
                        gameObject.transform.rotation = toQuat;
                    }
                }
            }
            else
            {
                if(Rotate==true)
                {
                    gameObject.transform.Rotate(AngleIncrement);

                }


            }
        }

        public virtual void OnInputClicked(InputEventData eventData)
        {
            if(CursorManager.Instance.GetDeleteState()==true)
            {
                IsBeingPlaced = true;
                Rotate = false;
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
                    Debug.Log(gameObject.name + " : Removing existing world anchor if any.");

                   
                }
                // If the user is not in placing mode, hide the spatial mapping mesh.
                else
                {
                    //The moving point during placement is the center of the object, in order to place the bottom of the object needs to be placed on the surface the object needs to be liftet by the real life size of the colider minus the dimensions of the cursor.
                    //Please note that this method is only accurate if the object is asumed to be a cube and the surface on which the object will be placed is horizontal
                    gameObject.transform.position += new Vector3(0.0f, (((this.GetComponent<BoxCollider>().size.y) / 2) * gameObject.transform.lossyScale.y) - CursorDimension, 0.0f);
                    spatialMappingManager.DrawVisualMeshes = false;

                }

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
    }
}
