using UnityEngine;
using System.Collections;

public enum NavigateAction
{
	Home,
	Back,
	Target,
}

[RequireComponent(typeof(SwipeDetector))]
public class NavigateOnSwipe : MonoBehaviour {
	public bool Left = false;
	public bool Right = false;
	public bool Up = false;
	public bool Down = false;
	
	public Navigator navigator;
	public NavigateAction action;
	public Transform target;
	
    void Start()
    {
        if (!GetComponent<SwipeDetector>()) {
            gameObject.AddComponent<SwipeDetector>();
        }
    }

    void DoYourThing()
    {
		if (!navigator) return;
		switch (action) {
		case NavigateAction.Back: navigator.NavigateBack(); break;
		case NavigateAction.Home: navigator.NavigateHome(); break;
		case NavigateAction.Target:
			if (target) {
        		navigator.NavigateTo(target);
			} 
			break;
		}
    }
	
	void SwipeDetector_Left()
	{
		if (Left) {
            DoYourThing();
		}
	}

	void SwipeDetector_Right()
	{
		if (Right) {
            DoYourThing();
		}
	}
	
	void SwipeDetector_Up()
	{
		if (Up) {
            DoYourThing();
		}
	}
	
	void SwipeDetector_Down()
	{
		if (Down) {
            DoYourThing();
		}
	}
} 