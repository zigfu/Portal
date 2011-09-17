using UnityEngine;
using System.Collections;

public class ThumbnailItem : MonoBehaviour {
	public MainMenuEntry mme;
	
	Renderer target;
	
	void Awake () 
	{
		target = transform.Find("Thumbnail").renderer;
	}
	
	void Start()
	{
		MenuItem_Unhighlight();
	}
	
	void MenuItem_Highlight()
	{
		//target.material.mainTexture = highlightTexture;
	}
	
	void MenuItem_Unhighlight()
	{
		//target.material.mainTexture = defaultTexture;
	}
}
