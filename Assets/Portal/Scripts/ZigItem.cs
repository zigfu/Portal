using UnityEngine;
using System.Collections;
using System.IO;
using ZigLib;

public class ZigItem : MonoBehaviour {
	
	public bool Installed { get; private set; }
	
	public MenuSystemThingie mst;
	public ZigInfo zigInfo;
	
	InstalledZig installedZig;
	public RemoteZig remoteZig;
	string zigUri;
	bool installing;
	
	public Material Icon {
		get {
			return transform.Find("Thumbnail").renderer.material;
		}
	}
	
	public void Init(IZig zig)
	{
		SharedMetadata md = zig.Metadata;
		transform.Find("NameLabel").GetComponent<TextMesh>().text = md.Name;
		transform.Find("DeveloperLabel").GetComponent<TextMesh>().text = md.Developer;
		transform.Find("DescriptionLabel").GetComponent<TextMesh>().text = md.Description;
		transform.Find("ActionLabel").GetComponent<TextMesh>().text = (null == installedZig) ? "INSTALL" : "LAUNCH";
	}
	
	public void InitRemote(RemoteZig zig)
	{
		remoteZig = zig;
		if (ZigLib.ZigLib.IsZigInstalled(zig as IZig)) {
			installedZig = ZigLib.ZigLib.GetInstalledZig(zig);
		}
		Init(zig);
		zigUri = zig.RemoteURI;
		StartCoroutine(LoadThumbnail(zig.ThumbnailURI));
	}
	
	public void InitInstalled(InstalledZig zig)
	{
		installedZig = zig;
		Init(zig);
		StartCoroutine(LoadThumbnail(zig.ThumbnailURI));
	}
	
	IEnumerator LoadThumbnail(string uri)
	{
		WWW req = new WWW(uri);
		yield return req;
		transform.Find("Thumbnail").renderer.material.mainTexture = req.texture;
	}
	
	WWW req;
	IEnumerator InstallFrom(string uri)
	{
		// prevent double install
		if (null != req) yield break;
		
		print("Downloading zig...");
		installing = true;
		req = new WWW(uri);
		while (req.progress < 1.0) {
			yield return new WaitForSeconds(0.1f);
			print(req.progress); // visualize
		}
		yield return req; // just to be sure
		installing = false;
		
		// write downloaded file to temp file
		string filename = Path.GetFullPath(Path.GetFileName(uri));
		File.WriteAllBytes(filename, req.bytes);
		
		// install & delete downloaded file
		print("Installing zig...");
		installedZig = ZigLib.ZigLib.InstallZig(filename);
		File.Delete(filename);
			
		// handle the fresh installation
		transform.Find("ActionLabel").GetComponent<TextMesh>().text = "LAUNCH";
		foreach (ZigsFeed feed in FindObjectsOfType(typeof(ZigsFeed))) {
			if (!feed.Remote) {
				feed.ReloadZigs();
			}
		}
		
		// tell someone to do something about this (make sure there is a receiver)
		//SendMessageUpwards("Zig_Installed", installedZig, SendMessageOptions.RequireReceiver);
	}
	
	public IEnumerator Launch()
	{
		if (null == installedZig) yield break;
		print("Launching zig...");
		installedZig.Launch();
	}
	
	void MenuItem_Select()
	{
		//if (null == installedZig) {
		//	StartCoroutine(InstallFrom(zigUri));
		//} else {
		//	StartCoroutine(Launch());
		//}
		
		//if (null != remoteZig) {
		//	zigInfo.Init(remoteZig);
		//	mst.Set(remoteZig.Metadata.Name, transform.Find("Thumbnail").renderer.material, 
		//}
	}
	
	void OnGUI()
	{
		if (installing) {
			GUI.HorizontalSlider(new Rect(0, Screen.height - 30, Screen.width, Screen.height - 10), req.progress, 0.0f, 1.0f);
		}
	}
}
