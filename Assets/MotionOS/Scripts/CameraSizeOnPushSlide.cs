using UnityEngine;
using System.Collections;

public class CameraSizeOnPushSlide : MonoBehaviour {
    public Camera target;
    public float PushSlideDelta = 2f;
    public float rate = 3f;
    float initialSize;
    float targetSize;

	// Use this for initialization
	void Start () {
        if (!target) {
            target = Camera.mainCamera;
        }
        targetSize = initialSize = target.orthographicSize;
	}
	
	// Update is called once per frame
	void Update () {
	    target.orthographicSize = Mathf.Lerp(target.orthographicSize, targetSize, Time.deltaTime * rate);
	}

    void Menu_PushSlideStart()
    {
        targetSize = initialSize + PushSlideDelta;
    }

    void Menu_PushSlideStop()
    {
        targetSize = initialSize;
    }
}
