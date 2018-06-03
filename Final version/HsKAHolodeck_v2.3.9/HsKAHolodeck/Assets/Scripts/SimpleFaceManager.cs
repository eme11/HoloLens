using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;
public class SimpleFaceManager : Singleton<SimpleFaceManager>
{
    // the prefab that this object pool returns instances of
    public GameObject prefab;
    public GameObject prefabMenu;
    // collection of currently inactive instances of the prefab
    private Stack<GameObject> inactiveInstances = new Stack<GameObject>();
    // collection of currently active instances of the prefab
    private List<GameObject> activeInstances = new List<GameObject>();
    // IDCounter gives individual IDs to all active faces
    private int IDCounter = 0;
    private int TappedButton = -1;
    private bool GlobalRotateState = false;
    private bool LoadingStateFlag = false;
    private bool GlobalWatchState = false;
    private bool showPopUps = true;
    public GameObject GetObject()
    {
        GameObject spawnedGameObject;

        // if there is an inactive instance of the prefab ready to return, return that
        if (inactiveInstances.Count > 0)
        {
            // remove the instance from teh collection of inactive instances
            spawnedGameObject = inactiveInstances.Pop();


        }
        // otherwise, create a new instance
        else
        {
            spawnedGameObject = (GameObject)GameObject.Instantiate(prefab);





            // add the PooledObject component to the prefab so we know it came from this pool
            PooledFace pooledObject = spawnedGameObject.AddComponent<PooledFace>();
            pooledObject.pool = this;
        }

        // put the instance in the root of the scene and enable it
        spawnedGameObject.GetComponent<SetupFace>().FaceID = IDCounter;
        spawnedGameObject.GetComponent<FaceBehavior>().TempLocalRotateState = SimpleFaceManager.Instance.GetGlobalRotateState();
        spawnedGameObject.GetComponent<FaceBehavior>().TempLocalWatchMeState = SimpleFaceManager.Instance.GetGlobalWatchState();
        activeInstances.Add(spawnedGameObject);
        spawnedGameObject.transform.SetParent(null);
        spawnedGameObject.SetActive(true);
        IDCounter++;


        // return a reference to the instance
        return spawnedGameObject;
    }
    public void SetLoadingStateFlag(bool Change, int ButtonID)
    {
        if (TappedButton == -1 || TappedButton == ButtonID)
        {
            LoadingStateFlag = Change;
        }

    }
    public bool GetLoadingStateFlag()
    {
        return LoadingStateFlag;

    }
    public bool GetGlobalRotateState()
    {
        return GlobalRotateState;

    }

    public bool SwitchGlobalRotateState()
    {
        if (GlobalRotateState == true)
        {
            DeactivateRotate();
        }
        else
        {
            ActivateRotate();

        }
        GlobalRotateState = !GlobalRotateState;
        return GlobalRotateState;

    }
    public bool GetGlobalWatchState()
    {
        return GlobalWatchState;
    }
    public bool SwitchGlobalWatchState()
    {
        if (GlobalWatchState == true)
        {
            DeactivateWatch();
        }
        else
        {
            ActivateWatch();

        }
        GlobalWatchState = !GlobalWatchState;
        return GlobalWatchState;

    }
    // Return an instance of the prefab to the pool
    public void ReturnObject(GameObject toReturn)
    {
        PooledFace pooledFace = toReturn.GetComponent<PooledFace>();

        // if the instance came from this pool, return it to the pool
        if (pooledFace != null && pooledFace.pool == this)
        {
            // make the instance a child of this and disable it
            toReturn.transform.SetParent(null, false);
            toReturn.SetActive(false);

            // add the instance to the collection of inactive instances
            inactiveInstances.Push(toReturn);

            activeInstances.Remove(toReturn);

        }
        // otherwise, just destroy it
        else
        {
            Debug.LogWarning(toReturn.name + " was returned to a pool it wasn't spawned from! Destroying.");
            Destroy(toReturn);
        }
    }


    private void ActivateRotate()
    {
        foreach (GameObject x in activeInstances)
        {
            x.GetComponent<FaceBehavior>().TempLocalRotateState = true;
        }

    }

    private void DeactivateRotate()
    {
        foreach (GameObject x in activeInstances)
        {
            x.GetComponent<FaceBehavior>().TempLocalRotateState = false;
        }


    }
    private void ActivateWatch()
    {
        foreach (GameObject x in activeInstances)
        {
            x.GetComponent<FaceBehavior>().TempLocalWatchMeState = true;
        }

    }

    private void DeactivateWatch()
    {
        foreach (GameObject x in activeInstances)
        {
            x.GetComponent<FaceBehavior>().TempLocalWatchMeState = false;
        }
    }

    public void SwitchShowPopUps()
    {
        showPopUps = !showPopUps;
    }

    public bool GetShowPopUps()
    {
        return showPopUps;
    }



}
public class PooledFace : MonoBehaviour
{
    public SimpleFaceManager pool;
}