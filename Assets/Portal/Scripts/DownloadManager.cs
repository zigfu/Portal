using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using ZigLib;

public class DownloadManager : MonoBehaviour {
	// singleton
    static DownloadManager instance;
    public static DownloadManager Instance
    {
        get
        {
            if (null == instance) {
                instance = FindObjectOfType(typeof(DownloadManager)) as DownloadManager;
                if (null == instance) {
                    GameObject container = new GameObject();
                    DontDestroyOnLoad(container);
                    container.name = "DownloadManagerContainer";
                    instance = container.AddComponent<DownloadManager>();
                }
                DontDestroyOnLoad(instance);
            }
            return instance;
        }
    }
	
	Dictionary<string, WWW> activeDownloads;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public static WWW StartZigDownload(RemoteZig zig)
	{
		return Instance.internalStartDownload(zig);
	}
	
	WWW internalStartDownload(RemoteZig zig)
	{
		WWW req = new WWW(zig.RemoteURI);
		activeDownloads[zig.Metadata.ZigID] = req;
		StartCoroutine(DownloadAndInstallZig(req, zig));
		return req;
	}
	                                        
	IEnumerator DownloadAndInstallZig(WWW req, RemoteZig zig)
	{
		// wait for download to complete
		yield return req;
		
		// write downloaded file to temp file
		string filename = Path.GetFullPath(Path.GetFileName(zig.RemoteURI));
		File.WriteAllBytes(filename, req.bytes);
		
		// install & delete downloaded file
		ZigLib.ZigLib.InstallZig(filename);
		File.Delete(filename);
			
		// handle the fresh installation - refresh zig feeds
		foreach (ZigsFeed feed in FindObjectsOfType(typeof(ZigsFeed))) {
			if (!feed.Remote) {
				feed.ReloadZigs();
			}
		}
		
		// wait a bit to prevent silly race condition
		yield return new WaitForSeconds(2.0f);
		
		// remove from list of active downloads ?
		activeDownloads.Remove(zig.Metadata.ZigID);
	}
	
	public static bool IsInstalling(RemoteZig zig)
	{
		return (Instance.activeDownloads.ContainsKey(zig.Metadata.ZigID) && !Instance.activeDownloads[zig.Metadata.ZigID].isDone);
	}
	
	public static WWW GetActiveDownload(RemoteZig zig)
	{
		return Instance.activeDownloads[zig.Metadata.ZigID];
	}
}
