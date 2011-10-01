using UnityEngine;
using System.Collections;
using ZigLib;

public class ZigsFeed : MonoBehaviour {
	public ScrollingMenu menu;
	public ZigItem ZigMenuItem;
	public bool Remote = false;
	public MenuSystemThingie mst;
	public ZigInfo zigInfo;
	public Navigator nav;
	
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
		print(req.text);
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
	
	void Menu_Select(Transform item)
	{
		if (Remote) {
			RemoteZig remoteZig = item.GetComponent<ZigItem>().remoteZig;
			zigInfo.Init(remoteZig);
			nav.NavigateTo(zigInfo.transform);
		}
		else {
			item.GetComponent<ZigItem>().Launch();
		}
	}
	
	void OnGUI()
	{
		if (Event.current.Equals(Event.KeyboardEvent("f5"))) {
			ReloadZigs();
		}
	}
}
