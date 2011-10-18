using UnityEngine;
using System.Collections;

public class ChangeCameraOnNavigate : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void Navigator_ActivatedItem(Transform item)
    {
        // see if target has a camera component
        Camera[] cams = item.GetComponentsInChildren<Camera>();

        if (cams.Length == 0) {
            Debug.LogError("Tried to navigate to an item with no camera");
            return;
        }

        Camera toActivate = cams[0];
        foreach (Camera cam in cams) {
            if (cam.name == "Main") {
                toActivate = cam;
            }
        }

        toActivate.gameObject.SetActiveRecursively(true);
    }
}
