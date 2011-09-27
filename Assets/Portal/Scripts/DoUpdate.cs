using UnityEngine;
using System.Collections;
using System.IO;
using ZigLib;
using System;

public class DoUpdate : MonoBehaviour {

    public float CurrentVersion = 0.1f;

	// Use this for initialization
	void Start () {
        //System.IO.Pipes.NamedPipeServerStream np = new System.IO.Pipes.NamedPipeServerStream("blahblah");

        StartCoroutine(CheckForUpdate());
	}

    // TODO: toggle according to OS
    // TODO: maybe make getting this URL part of ziglib?
    private string GetVersionURL = "http://zigfu.com/portal/version/win";

    IEnumerator CheckForUpdate()
    {
        WWW getVersionReq = new WWW(GetVersionURL);
        print("Getting portal version from " + GetVersionURL);
        yield return getVersionReq;
        string download_url;
        double version;
        Hashtable response = JSON.JsonDecode(getVersionReq.text) as Hashtable;
        download_url = (string)response["dl_url"];
        version = (double)response["version"];
        print(string.Format("Got response, remote version is {0}, url is {1}", version, download_url));
        if (version <= CurrentVersion) {
            print("current version newer, doing nothing");
            yield break;
        }
        print(string.Format("current version ({0}) is older than remote version ({1}), updating", CurrentVersion, version));
        WWW GetPortalRequest = new WWW(download_url);
        while (!GetPortalRequest.isDone) {
            yield return new WaitForSeconds(1.0f);
            print("progress: " + GetPortalRequest.progress);
        }
        print("download finished, size: " + GetPortalRequest.bytes.Length + ", first bytes: " + BitConverter.ToString(GetPortalRequest.bytes, 0, 8));
        //print("first few bytes: " + BitConverter.To
        var di = new DirectoryInfo(ZigLib.Utility.GetMainModuleDirectory());
        string TempExtractedPath = Path.Combine(di.Parent.FullName, "new_version");
        print(string.Format("downloading and extracting zip to " + TempExtractedPath));
        if (Directory.Exists(TempExtractedPath)) {
            print("Dir already exists, emptying it first");
            Directory.Delete(TempExtractedPath, true);
        }
        Directory.CreateDirectory(TempExtractedPath);

        using (var ms = new MemoryStream(GetPortalRequest.bytes, false)) {
            using (var zip = Ionic.Zip.ZipFile.Read(ms)) {
                print("opened zip");
                zip.ExtractAll(TempExtractedPath);
            }
        }
        string TempRunningDir = Path.Combine(di.Parent.FullName, "bin.old");
        print(string.Format("moving current directory ({0}) to {1}", di.FullName, TempRunningDir));
        ZigLib.Utility.MoveDirWithLockedFile(di.FullName, TempRunningDir);
        print(string.Format("moving temp directory {0} to binary directory {1}", TempExtractedPath, di.FullName));
        ZigLib.Utility.MoveDirWithLockedFile(TempExtractedPath, di.FullName);
        Application.Quit();
    }
}
