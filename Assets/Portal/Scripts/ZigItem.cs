using UnityEngine;
using System.Collections;
using System.IO;
using ZigLib;

public class ZigItem : MonoBehaviour {
	
	public bool Installed { get; private set; }
	
	InstalledZig installedZig;
	string zigUri;

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
	
	IEnumerator Launch()
	{
		if (null == installedZig) {
			print("Downloading zig...");
			WWW req = new WWW(zigUri);
			while (req.progress < 1.0) {
				yield return new WaitForSeconds(0.1f);
				print(req.progress); // visualize
			}
			yield return req; // just to be sure
			File.WriteAllBytes(".\\temp.crap", req.bytes);
			print("Installing zig...");
			installedZig = ZigLib.ZigLib.InstallZig(".\\temp.crap");
		}
		print("Launching zig...");
		installedZig.Launch();
	}
	
	void MenuItem_Select()
	{
		StartCoroutine(Launch());	
	}
}
