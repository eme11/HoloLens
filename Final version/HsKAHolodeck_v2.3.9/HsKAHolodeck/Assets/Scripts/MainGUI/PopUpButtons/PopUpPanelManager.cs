using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopUpPanelManager : MonoBehaviour{

    public GameObject parent;
    public GameObject Menu;
    private bool isBeingPlaced;
    void Start () {
        isBeingPlaced = parent.GetComponent<FaceBehavior>().GetIsBeingPlaced();
    }

    // Update is called once per frame
    //In Update, we ensure that the Popup Menu always faces the camera
    //We also check if the gaze is on the menu, and if the user enabled Popups from the Main Menu
    void Update()
    {
        gameObject.transform.LookAt(Camera.main.transform.position);
        DeactivateMenu();
    }

    private void DeactivateMenu()
    {
        StartCoroutine(FadeDelay());
    }
    //We used a coroutine to allow the user time to reach the menu with the gaze, and because
    //we don't want the menu to immediately dissappear
    IEnumerator FadeDelay()
    {
        yield return new WaitForSeconds(1.0f);
        //the menu is disabled in two cases: either the user is looking away from the menu, or popups are disabled
        if (!EventSystem.current.IsPointerOverGameObject() || !SimpleFaceManager.Instance.GetShowPopUps())
        {
            Debug.Log("The panel is not being focused; " + Time.time);
            yield return new WaitForSeconds(1.0f);
            Debug.Log("We deactivate the menu. " + Time.time);
            Menu.SetActive(false);
        }

    }

}
