using UnityEngine;
using System.Collections;
using ZigLib;

public class StartZiglibLoader : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartLoader();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	IEnumerator WaitForLoader()
	{
        yield break;
	}
	
	void StartLoader()
	{
	}
	
	void OnApplicationShutdown()
	{
	}
}
