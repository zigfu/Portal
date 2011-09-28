using UnityEngine;
using System.Collections;

public class MenuSystemThingie : MonoBehaviour {
	public GameObject myCamera;
	public GameObject title;
	public GameObject icon;
	public GameObject backIndicator;
	
	public Navigator navigator;
	
	Camera currentMenuCam;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void Set(string strTitle, Material matIcon, Camera targetCamera)
	{
		title.GetComponent<TextMesh>().text = strTitle;
		icon.GetComponent<Renderer>().material = matIcon;
		
		if (null != currentMenuCam) {
			currentMenuCam.gameObject.SetActiveRecursively(false);
		}
		currentMenuCam = targetCamera;
		currentMenuCam.gameObject.SetActiveRecursively(true);
		
		myCamera.gameObject.SetActiveRecursively(true);
		GetComponent<HandPointControl>().Activate();
	}
	
	public void Home()
	{
		myCamera.gameObject.SetActiveRecursively(false);
		GetComponent<HandPointControl>().Deactivate();
		navigator.NavigateHome();
	}
	
	void SwipeDetector_Left()
	{
		Home();
	}
	
	void OnGUI()
	{
		if (Event.current.Equals(Event.KeyboardEvent("escape"))) {
            if (GetComponent<HandPointControl>().IsActive) {
				Home();
				Event.current.Use();
			}
        }
	}
}
