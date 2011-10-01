using UnityEngine;
using System.Collections;
using System.IO;
using ZigLib;

[RequireComponent(typeof(PushDetector))]
public class ZigInfo : MonoBehaviour {

	RemoteZig remoteZig;
	InstalledZig installedZig;
	
	bool installing;
	WWW installReq;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void Init(RemoteZig zig, Material icon)
	{
		remoteZig = zig;
		transform.Find("Description").gameObject.GetComponent<TextMesh>().text = TextTools.WordWrap(zig.Metadata.Description, 30);
		transform.Find("Developer").gameObject.GetComponent<TextMesh>().text = zig.Metadata.Developer;
		if (ZigLib.ZigLib.IsZigInstalled(zig)) {
			installedZig = ZigLib.ZigLib.GetInstalledZig(zig);
		}
		
		if (null != installedZig) {
			
			transform.Find("ActionLabel").gameObject.GetComponent<TextMesh>().text = "LAUNCH";
		}
		else {			
			//StartCoroutine(LoadThumbnail(zig.ThumbnailURI));
			transform.Find("ActionLabel").gameObject.GetComponent<TextMesh>().text = "INSTALL";
		}
		
		MSTScreen screenInfo = GetComponent<MSTScreen>();
		if (null != screenInfo) {
			screenInfo.Title = zig.Metadata.Name;
			screenInfo.Icon = icon;
		}
	}
	
	IEnumerator LoadThumbnail(Material mat, string uri)
	{
		WWW req = new WWW(uri);
		yield return req;
		mat.mainTexture = req.texture;
	}
	
		
	IEnumerator InstallFrom(string uri)
	{
		// prevent double install
		if (null != installReq) yield break;
		
		print("Downloading zig...");
		installing = true;
		installReq = new WWW(uri);
		while (installReq.progress < 1.0) {
			yield return new WaitForSeconds(0.1f);
			print(installReq.progress); // visualize
		}
		yield return installReq; // just to be sure
		installing = false;
		
		// write downloaded file to temp file
		string filename = Path.GetFullPath(Path.GetFileName(uri));
		File.WriteAllBytes(filename, installReq.bytes);
		
		// install & delete downloaded file
		print("Installing zig...");
		installedZig = ZigLib.ZigLib.InstallZig(filename);
		File.Delete(filename);
			
		// handle the fresh installation
		transform.Find("ActionLabel").gameObject.GetComponent<TextMesh>().text = "LAUNCH";
		foreach (ZigsFeed feed in FindObjectsOfType(typeof(ZigsFeed))) {
			if (!feed.Remote) {
				feed.ReloadZigs();
			}
		}
	}
	
	IEnumerator Launch()
	{
		if (null == installedZig) yield break;
		print("Launching zig...");
		installedZig.Launch();
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
		if (null == installedZig) {
			StartCoroutine(InstallFrom(remoteZig.RemoteURI));
		} else {
			StartCoroutine(Launch());
		}
	}
}
