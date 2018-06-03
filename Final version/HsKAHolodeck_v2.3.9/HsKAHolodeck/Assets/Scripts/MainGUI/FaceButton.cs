using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using UnityEngine.UI;
public class FaceButton : MonoBehaviour, IInputClickHandler
{
    public GameObject Content;
    public Button RemoveButton;
    private bool DeleteState;

    //on click, navigates to the list of faces available
    //it also switches off Remove Faces, if it was turned on prior to the click
    public void OnInputClicked(InputEventData eventData)
    {
        DeleteState = CursorManager.Instance.GetDeleteState();
        if (DeleteState == true)
        {
            RemoveButton.GetComponentInChildren<Text>().text = "Remove Faces";
            CursorManager.Instance.SwitchDeleteState();
        }
        FaceScrollList f = Content.GetComponentInChildren<FaceScrollList>();
        f.ScanForFaces();
    }
}
