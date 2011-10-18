using UnityEngine;
using System.Collections;

// HACK for now just use the arrow visualizer code
public class PushVisualizer : MonoBehaviour {
	public PushDetector pushDetector;
	public SimpleVisualizerSliderThingie viz;
	public Vector3 offset;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (null != pushDetector && pushDetector.gameObject.GetComponent<HandPointControl>().IsActive) {
			viz.SetProgress(pushDetector.ClickProgress);
		}
	}
	
	void Menu_Highlight(Transform item)
	{
		viz.transform.localPosition = item.localPosition + offset;
	}
}
