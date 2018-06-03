using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity.InputModule;

public class ButtonUpScript : MonoBehaviour, IInputClickHandler
{
    private ScrollRect scrollRect;
    
    public void Start()
    {
        scrollRect = transform.GetComponentInParent<ScrollRect>();



    } 


    public void OnInputClicked(InputEventData eventData)
    {
        float TempPos = scrollRect.verticalNormalizedPosition;
        if (TempPos + 0.1 > 1)
        { scrollRect.verticalNormalizedPosition = 1f; }
        else
        { scrollRect.verticalNormalizedPosition += 0.2f; }

    }

}
