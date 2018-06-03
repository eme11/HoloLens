// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class TogglePopupMenu : MonoBehaviour
    {

        [SerializeField]
        private GameObject popupMenu = null;

        [SerializeField]
        private GameObject button = null;

        private void Awake()
        {
            ShowPopup();
        }

        private void OnDisable()
        {
            if (button)
            {
                //button.Activated -= ShowPopup;
            }
        }

        private void ShowPopup()
        {
            if (popupMenu != null)
            {
                if (popupMenu.activeSelf == false)
                {
                    popupMenu.SetActive(true);

                    StartCoroutine(WaitForPopupToClose());
                }
            }
        }

        private IEnumerator WaitForPopupToClose()
        {
            if (popupMenu)
            {
                while (popupMenu.activeSelf == true)
                {
                    yield return null;
                }
            }

            if (button)
            {
               // button.Selected = false;
            }
        }
    }
}