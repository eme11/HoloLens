using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity.InputModule;
public class DeleteButton : MonoBehaviour, IInputClickHandler
{
    private bool DeleteState;
    public Text ButtonText;
    public Button CreateFaceButton;

    //on click, turns ON/OFF the option to remove faces when clicking on them
    public void OnInputClicked(InputEventData eventData)
    {
        DeleteState = CursorManager.Instance.GetDeleteState();
        if (DeleteState == true)
        {
            ButtonText.text = "Remove Faces";
            CursorManager.Instance.SwitchDeleteState();

        }
        else
        {
            ButtonText.text = "Stop Removing";
            CursorManager.Instance.SwitchDeleteState();

        }

    }
}
