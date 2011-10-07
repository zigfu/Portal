using UnityEngine;
using System.Collections;
using System.IO;
using ZigLib;

[RequireComponent(typeof(PushDetector))]
public class ZigInfo : MonoBehaviour {
#if UNITY_EDITOR
    const bool isFullscreen = false;
#else 
    const bool isFullscreen = true;
#endif

	public GameObject downloadProgressBar;
	
	RemoteZig remoteZig;
	InstalledZig installedZig;
	
	// Use this for initialization
	void Start () {
		
		downloadProgressBar.SetActiveRecursively(false);
	}
    bool cleanupProcessLaunch = false;
	// Update is called once per frame
	void Update () {
        if (cleanupProcessLaunch) {
            OpenNIContext.Instance.UpdateContext = true;
            SessionManager.Instance.StartListening();
            cleanupProcessLaunch = false;
        }
	}
	
	public void Init(RemoteZig zig, Material icon)
	{
		// Stop coroutine from running install
		downloadProgressBar.SetActiveRecursively(false);
		StopCoroutine("UpdateInstallProgress");
		installedZig = null;
		
		remoteZig = zig;
		transform.Find("Description").gameObject.GetComponent<TextMesh>().text = TextTools.WordWrap(zig.Metadata.Description, 30);
		transform.Find("Developer").gameObject.GetComponent<TextMesh>().text = zig.Metadata.Developer;
		if (ZigLib.ZigLib.IsZigInstalled(zig)) {
			installedZig = ZigLib.ZigLib.GetInstalledZig(zig);
		}
		
		// a zig can be remote, installed, or installing

		// installed
		if (null != installedZig) {
			print("Initing installed ziginfo");
			transform.Find("ActionLabel").gameObject.GetComponent<TextMesh>().text = "LAUNCH";
		}
		
		// installing
		else if (DownloadManager.IsInstalling(zig))
		{
			print("Initing ziginfo with install in progress");
			StartCoroutine("UpdateInstallProgress", zig);
		}
		
		// remote
		else {			
			print("Initing remote ziginfo");
			transform.Find("ActionLabel").gameObject.GetComponent<TextMesh>().text = "INSTALL";
		}
		
		MSTScreen screenInfo = GetComponent<MSTScreen>();
		if (null != screenInfo) {
			screenInfo.Title = zig.Metadata.Name;
			screenInfo.Icon = icon;
		}
	}
	
	IEnumerator UpdateInstallProgress(RemoteZig zig)
	{
		WWW req = DownloadManager.GetActiveDownload(zig);
		transform.Find("ActionLabel").gameObject.GetComponent<TextMesh>().text = "INSTALLING...";
		downloadProgressBar.SetActiveRecursively(true);
		while (!req.isDone) {
			yield return new WaitForSeconds(0.1f);
			downloadProgressBar.renderer.materials[1].SetFloat("_Cutoff", req.progress);
		}
		installedZig = ZigLib.ZigLib.GetInstalledZig(zig);
		downloadProgressBar.SetActiveRecursively(false);
		transform.Find("ActionLabel").gameObject.GetComponent<TextMesh>().text = "LAUNCH";
	}
	
	IEnumerator LoadThumbnail(Material mat, string uri)
	{
		WWW req = new WWW(uri);
		yield return req;
		mat.mainTexture = req.texture;
	}

	IEnumerator Launch()
	{
        if (null == installedZig) yield break;
        //TODO: hack o'mercy
        yield return null;
		print("Launching zig...");
        //SessionManager.Instance.StopListening();
        //try {
        //    installedZig.Launch(OpenNIContext.Context, isFullscreen);
        //}
        //finally {
        //    SessionManager.Instance.StartListening();
        //}
        SessionManager.Instance.StopListening();
        OpenNIContext.Instance.UpdateContext = false;

        installedZig.Launch(OpenNIContext.Context, delegate(object s, System.EventArgs e) {
            cleanupProcessLaunch = true;
            Debug.Log("done with process");
        });
	}
	
	// temporarily react to create, destroy, and click. eventually this will be replaced by an actual menu
	
	void Hand_Create()
	{
		transform.Find("ActionLabel").renderer.material.color = Color.green;		
	}
	
	void Hand_Destroy()
	{
		transform.Find("ActionLabel").renderer.material.color = Color.white;
	}
	
	void PushDetector_Click()
	{
		// do nothing if already installing
		if (DownloadManager.IsInstalling(remoteZig)) {
			return;
		}
		
		// launch if already installed
		if (null != installedZig) {
			StartCoroutine(Launch());
            
			return;
		}
		
		// otherwise start install
		DownloadManager.StartZigDownload(remoteZig);
		StartCoroutine("UpdateInstallProgress", remoteZig);
	}
	
	void OnGUI()
	{
		if (!GetComponent<HandPointControl>().IsActive) { return; }
		
		if (Event.current.Equals(Event.KeyboardEvent("[enter]"))) {
		    PushDetector_Click();
		    Event.current.Use();
		}
	}
}
