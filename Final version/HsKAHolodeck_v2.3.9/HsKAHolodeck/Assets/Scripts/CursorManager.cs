using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
public class CursorManager : Singleton<CursorManager> {
    private bool DeleteState=false;

    public bool GetDeleteState()
    {
        return DeleteState;

    }

    public void SwitchDeleteState()
    {
        DeleteState = !DeleteState;

    }

}
