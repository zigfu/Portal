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
		while (!LoaderLib.LoaderAPI.ServerExists()) {
		    yield return new WaitForSeconds(0.5f);
		}
	}
	
	void StartLoader()
	{
		if (!LoaderLib.LoaderAPI.ServerExists()) {
			LoaderLib.LoaderAPI.LaunchServer();
		}
	}
	
	void OnApplicationShutdown()
	{
		// Handles shutdown server internally
		LoaderLib.LoaderAPI.ShutdownClient();
	}
}
