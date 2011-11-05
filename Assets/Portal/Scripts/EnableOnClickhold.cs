using UnityEngine;
using System.Collections;

public class EnableOnClickhold : MonoBehaviour {
	
	public Camera target;
	
	SepiaToneEffect ste;
	
	// Use this for initialization
	void Start () {
		if (!target) return;
		
		ste = target.GetComponent<SepiaToneEffect>();
		if (null == ste) {
			ste = target.gameObject.AddComponent<SepiaToneEffect>();
		}
		ste.enabled = false;
	}
	
	bool IsClickholding;
	void PushDetector_ClickHold()
    {
		IsClickholding = true;
		if (ste != null) {
			ste.enabled = true;
		}
    }

    void PushDetector_Release()
    {
		if (!IsClickholding) return;
		
		IsClickholding = false;
		if (ste != null) {
			ste.enabled = false;
		}
    }
}
