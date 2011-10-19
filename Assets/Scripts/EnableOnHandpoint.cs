using UnityEngine;
using System.Collections;

public class EnableOnHandpoint : MonoBehaviour {

    public Transform[] targets;

	// Use this for initialization
	void Awake () {
        Hand_Destroy();
	}
	
    void Hand_Create(Vector3 pos)
    {
        foreach (Transform t in targets) {
            t.gameObject.SetActiveRecursively(true);
        }
    }

    void Hand_Destroy()
    {
        foreach (Transform t in targets) {
            t.gameObject.SetActiveRecursively(false);
        }
    }
}
