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

    string lastTitle;
    Material lastMaterial;
    Camera lastCamera;

    public void Set(string strTitle, Material matIcon, Camera targetCamera)
    {
        Set(strTitle, matIcon, targetCamera, false);
    }

	public void Set(string strTitle, Material matIcon, Camera targetCamera, bool keepHistory)
	{
        if (keepHistory) {
            lastTitle = title.GetComponent<TextMesh>().text;
            lastCamera = currentMenuCam;
            lastMaterial = icon.GetComponent<Renderer>().material;
        }
        else {
            lastTitle = null;
            lastMaterial = null;
            lastCamera = null;
        }
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

    public void Back()
    {
        //TODO: real implementation with a stack and stuff, not just one step back
        if (null != lastCamera) {
            Set(lastTitle, lastMaterial, lastCamera, false);
            navigator.NavigateBack();
            lastTitle = null;
            lastMaterial = null;
            lastCamera = null;
        }
        else {
            Home();
        }
    }
	
	public void Home()
	{
		myCamera.gameObject.SetActiveRecursively(false);
		GetComponent<HandPointControl>().Deactivate();
		navigator.NavigateHome();
	}
	
	void SwipeDetector_Left()
	{
		//Home();
        Back();
	}
	
	void OnGUI()
	{
		if (Event.current.Equals(Event.KeyboardEvent("escape"))) {
            if (GetComponent<HandPointControl>().IsActive) {
				Back();
				Event.current.Use();
			}
        }
	}
}
