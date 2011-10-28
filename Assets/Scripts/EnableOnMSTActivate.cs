using UnityEngine;
using System.Collections;

public class EnableOnMSTActivate : MonoBehaviour {

    public Transform[] targets;

	void Awake () {
        MST_Deactivate();
	}
	
    void MST_Activate()
    {
        foreach (Transform t in targets) {
            t.gameObject.SetActiveRecursively(true);
        }
    }

    void MST_Deactivate()
    {
        foreach (Transform t in targets) {
            t.gameObject.SetActiveRecursively(false);
        }
    }
}
