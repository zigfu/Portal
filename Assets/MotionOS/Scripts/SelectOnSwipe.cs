using UnityEngine;
using System.Collections;

public class SelectOnSwipe : MonoBehaviour {
	public bool Left = false;
	public bool Right = false;
	public bool Up = false;
	public bool Down = false;

    void Start()
    {
        if (!GetComponent<SwipeDetector>()) {
            gameObject.AddComponent<SwipeDetector>();
        }
    }

    void DoYourThing()
    {
        SendMessage("Menu_SelectActive", SendMessageOptions.DontRequireReceiver);
    }
	
	void Swipe_Left()
	{
		if (Left) {
            DoYourThing();
		}
	}

	void Swipe_Right()
	{
		if (Right) {
            DoYourThing();
		}
	}
	
	void Swipe_Up()
	{
		if (Up) {
            DoYourThing();
		}
	}
	
	void Swipe_Down()
	{
		if (Down) {
            DoYourThing();
		}
	}
}
