using UnityEngine;
using System.Collections;

public class EnableOnNavigate : MonoBehaviour {

    public Transform[] targets;

	// Use this for initialization
	void Start () {
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
