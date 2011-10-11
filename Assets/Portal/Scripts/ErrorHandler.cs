using UnityEngine;
using System.Collections;

public class ErrorHandler : MonoBehaviour {
	public TextMesh txt;
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void SetErrorText(string msg)
	{
		txt.text = TextTools.WordWrap(msg, 30);
	}
}
