using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SwipeDetector))]
public class SwipeOverlay : MonoBehaviour {

    public float FullPoint = 0;
    public float StartPoint = 0.3f;

    public Material target;
    private Fader source;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        if (source == null) {
            source = GetComponent<SwipeDetector>().swipeFader;
        }
        if ((source != null) & (target != null)) {
            float length = FullPoint - StartPoint; // no abs intentionally
            float pos = 1.0f - Mathf.Clamp01((source.value - StartPoint) / length);
            target.SetFloat("_Cutoff", pos);
        }
	}
}
