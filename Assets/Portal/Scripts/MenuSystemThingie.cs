using UnityEngine;
using System.Collections;

public class MenuSystemThingie : MonoBehaviour {
	public GameObject myCamera;
	public GameObject title;
	public GameObject icon;
	
    public void Set(string strTitle, Material matIcon)
    {
		title.GetComponent<TextMesh>().text = strTitle;
		icon.GetComponent<Renderer>().material = matIcon;
		myCamera.gameObject.SetActiveRecursively(true); // animate
	}
    Transform lastActivatedNonModal = null;
    Transform modalDialog = null;
	void Navigator_ActivatedItem(Transform target)
	{
        // if navigating away from modal dialog
        if ((inModalDialog) && (target != modalDialog)) {
            if (target != lastActivatedNonModal) {
                lastActivatedNonModal.SendMessage("MST_Deactivate", SendMessageOptions.DontRequireReceiver);
            }
            inModalDialog = false;
        }

        // if not navigating back to the still active control
        if (target != lastActivatedNonModal) {
            target.SendMessage("MST_Activate", SendMessageOptions.DontRequireReceiver);
        }

        // if not navigating to modal dialog
        if ((!inModalDialog) || (target != modalDialog)) {
            lastActivatedNonModal = target;
            MSTScreen info = target.gameObject.GetComponent<MSTScreen>();
            if (null != info) {
                Set(info.Title, info.Icon);
                info.Cam.rect = new Rect(0f, 0.0f, 1.0f, 0.7f); //animate
            }
            else {
                myCamera.gameObject.SetActiveRecursively(false); //animate
            }
        }

    }

    void Navigator_DeactivatedItem(Transform target)
    {
        if ((!inModalDialog)||(target != lastActivatedNonModal)) {
            target.SendMessage("MST_Deactivate", SendMessageOptions.DontRequireReceiver);
        }
    }

    private bool inModalDialog = false;

    public void ShowModalDialog(Transform target)
    {
        if (inModalDialog) {
            throw new System.Exception("Trying to show a modal dialog from within one. Unsupported :(");
        }
        inModalDialog = true;
        modalDialog = target;

        //TODO: not have implicit reference to navigator?
        //TODO: review with teh shlomonator
        GetComponent<Navigator>().NavigateTo(target);
        //GetComponent<Navigator>().NavigateTo(target, false);
    }

	void OnGUI()
	{
		if (Event.current.Equals(Event.KeyboardEvent("escape"))) {
            if (GetComponent<HandPointControl>().IsActive) {
				GetComponent<Navigator>().NavigateBack();
				Event.current.Use();
			}
        }
	}
}
