using UnityEngine;
using System.Collections;

public class EnableOnNavigate : MonoBehaviour {

    public Transform[] targets;

	void Awake () {
        Navigator_Deactivate();
	}
	
    void Navigator_Activate()
    {
        foreach (Transform t in targets) {
            t.gameObject.SetActiveRecursively(true);
        }
    }

    void Navigator_Deactivate()
    {
        foreach (Transform t in targets) {
            t.gameObject.SetActiveRecursively(false);
        }
    }
}
