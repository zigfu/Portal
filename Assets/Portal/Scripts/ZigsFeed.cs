using UnityEngine;
using System.Collections;
using ZigLib;

public class ZigsFeed : MonoBehaviour {
	public ScrollingMenu menu;
	public ZigItem ZigMenuItem;
	public bool Remote = false;

	// Use this for initialization
	void Start () {
		if (!menu) {
			menu = GetComponent<ScrollingMenu>();
		}
		
		Load();
	}
	
	void Load()
	{
		// init a menu item for each zig (for great justice)
		if (Remote) {
			StartCoroutine(LoadRemoteZigs(ZigLib.ZigLib.GetRemoteZigsQuery()));
		} else {
			foreach (InstalledZig zig in ZigLib.ZigLib.EnumerateInstalledZigs()) {
				ZigItem zi = InitZig();
				zi.InitInstalled(zig);
			}
		}
	}
	
	IEnumerator LoadRemoteZigs(string url)
	{
		WWW req = new WWW(url);
		yield return req;
		foreach (RemoteZig zig in ZigLib.ZigLib.EnumerateRemoteZigs(req.text)) {
			ZigItem zi = InitZig();
			zi.InitRemote(zig);
		}
	}
	
	ZigItem InitZig()
	{
		ZigItem newZig = Instantiate(ZigMenuItem) as ZigItem;
		newZig.transform.localPosition = Vector3.zero;
		newZig.transform.localRotation = Quaternion.identity;
		menu.Add(newZig.transform);
		return newZig;
	}
	
	public void ReloadZigs()
	{
		menu.Clear();
		Load();
	}
	
	void OnGUI()
	{
		if (Event.current.Equals(Event.KeyboardEvent("f5"))) {
			ReloadZigs();
		}
	}
}
