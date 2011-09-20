using UnityEngine;
using System.Collections;
using System.IO;
using ZigLib;

public class ZigItem : MonoBehaviour {
	
	public bool Installed { get; private set; }
	
	InstalledZig installedZig;
	string zigUri;
	bool installing;
	
	public void Init(ZigMetadata md)
	{
		transform.Find("NameLabel").GetComponent<TextMesh>().text = md.Name;
		transform.Find("DeveloperLabel").GetComponent<TextMesh>().text = md.Developer;
		transform.Find("DescriptionLabel").GetComponent<TextMesh>().text = md.Description;
		StartCoroutine(LoadThumbnail(md.ThumbnailURI));
	}
	
	public void InitRemote(RemoteZig zig)
	{
		Init(zig.Metadata);
		IZig iz = zig;
		if (ZigLib.ZigLib.IsZigInstalled(iz)) {
			// TODO handle installed zig
			// FUTURE TODO handle "Update" functionality if local & remote versions differ
		}
		
		zigUri = zig.RemoteURI;
	}
	
	public void InitInstalled(InstalledZig zig)
	{
		Init(zig.Metadata);
		installedZig = zig;
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
				
		// tell someone to do something about this (make sure there is a receiver)
		SendMessageUpwards("Zig_Installed", installedZig, SendMessageOptions.RequireReceiver);
	}
	
	IEnumerator Launch()
	{
		if (null == installedZig) yield break;
		print("Launching zig...");
		installedZig.Launch();
	}
	
	void MenuItem_Select()
	{
		if (null == installedZig) {
			StartCoroutine(InstallFrom(zigUri));
		} else {
			StartCoroutine(Launch());
		}
	}
	
	void OnGUI()
	{
		if (installing) {
			GUI.HorizontalSlider(new Rect(0, Screen.height - 30, Screen.width, Screen.height - 10), req.progress, 0.0f, 1.0f);
		}
	}
}
