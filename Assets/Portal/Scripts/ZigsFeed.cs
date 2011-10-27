using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ZigLib;

[Serializable]
public class SceneZig
{
	public string Name;
	public string Developer;
	public string Description;
	public Material Thumbnail;
	public string SceneName;
}

public class ZigsFeed : MonoBehaviour {
	public ScrollingMenu menu;
	public ZigItem ZigMenuItem;
	public bool Remote = false;
	public MenuSystemThingie mst;
	public ZigInfo zigInfo;
	public Navigator nav;
	public List<SceneZig> SceneZigs = new List<SceneZig>();
	
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
			StartCoroutine(LoadInstalledZigs());
		}
	}
	
	IEnumerator LoadRemoteZigs(string url)
	{
		WWW req = new WWW(url);
		yield return req;
		print(url);
		foreach (RemoteZig zig in ZigLib.ZigLib.EnumerateRemoteZigs(req.text)) {
			ZigItem zi = InitZig();
			zi.InitRemote(zig);
			yield return new WaitForSeconds(0.25f);
		}
	}
	
	IEnumerator LoadInstalledZigs()
	{
		yield return StartCoroutine(LoadSceneZigs());
		foreach (InstalledZig zig in ZigLib.ZigLib.EnumerateInstalledZigs()) {
			ZigItem zi = InitZig();
			zi.InitInstalled(zig);
			yield return new WaitForSeconds(0.25f);
		}
		yield break;
	}
	
	IEnumerator LoadSceneZigs()
	{
		foreach (SceneZig zig in SceneZigs)	{
			ZigItem zi = InitZig();
			zi.InitScene(zig);
			yield return new WaitForSeconds(0.25f);
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
			zigInfo.Init(remoteZig, item.GetComponent<ZigItem>().Icon);
			nav.NavigateTo(zigInfo.transform);
		}
		else {
			StartCoroutine(item.GetComponent<ZigItem>().Launch());
		}
	}
	
	void OnGUI()
	{
		if (Event.current.Equals(Event.KeyboardEvent("f5"))) {
			ReloadZigs();
		}
	}
}
