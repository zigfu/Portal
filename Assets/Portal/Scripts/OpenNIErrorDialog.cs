using UnityEngine;
using System.Collections;

public class OpenNIErrorDialog : MonoBehaviour {
	public bool CheckForOpenNIError = true;
	public ErrorHandler handler;
	
	// Use this for initialization
	void Start () {
		Navigator nav = GetComponent<Navigator>();
		
		if (CheckForOpenNIError && OpenNIContext.Instance.error) {
			handler.SetErrorText(OpenNIContext.Instance.errorMsg);
			nav.NavigateTo(handler.transform);
			return;
		}
		
		nav.NavigateHome();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
