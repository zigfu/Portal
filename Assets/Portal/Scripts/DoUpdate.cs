using UnityEngine;
using System.Collections;
using System.IO;
using ZigLib;
using System;

public class DoUpdate : MonoBehaviour {

    public float CurrentVersion = 0.3f;
    public string OverrideVersionFile = "test.ver";
    public ZiglibInit Initializer = null;
    private bool UseTestURL = false;
	// Use this for initialization
	void Start () {
        if (Initializer != null) {
            UseTestURL = Initializer.TestMode;
        }
        if ((null != OverrideVersionFile) && (File.Exists(OverrideVersionFile))) {
            try {
                string data = File.ReadAllText(OverrideVersionFile);
                float version;
                if (float.TryParse(data.Trim(), out version)) {
                    print(string.Format("overriding version {0} with version {1} from file {2}", CurrentVersion, version, OverrideVersionFile));
                    CurrentVersion = version;
                }
            }
            catch (IOException) {
            }
        }
        //check for update only when not running in editor
#if !UNITY_EDITOR
        StartCoroutine(CheckForUpdate());
#endif
	}

    // TODO: toggle according to OS
    // TODO: maybe make getting this URL part of ziglib?
    //private string GetServerVersionURL = "http://zigfu.com/portal/version/win";
    private string GetServerVersionURL()
    {
        if (!UseTestURL) {
            return "http://zigfu.com/portal/version/win";
        }
        else {
            return "http://test.zigfu.com/static/update_test/win";
        }
    }

    const string OLD_BIN_DIR = "bin.old";

    IEnumerator CheckForUpdate()
    {
        // check for remains of an old update
        var di = new DirectoryInfo(ZigLib.Utility.GetMainModuleDirectory());
        string oldBinDir = GetOldBinDir(di);
        if (Directory.Exists(oldBinDir)) {
            print("Deleting leftovers from previous update");
            try {
                Directory.Delete(oldBinDir, true);
            }
            catch (IOException) {
                Debug.LogWarning("Failed to delete " + oldBinDir);
            }
        }

        WWW getVersionReq = new WWW(GetServerVersionURL());
        print("Getting portal version from " + GetServerVersionURL());
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
        string TempRunningDir = GetOldBinDir(di);
        print(string.Format("moving current directory ({0}) to {1}", di.FullName, TempRunningDir));
        ZigLib.Utility.MoveDirWithLockedFile(di.FullName, TempRunningDir);
        print(string.Format("moving temp directory {0} to binary directory {1}", TempExtractedPath, di.FullName));
        ZigLib.Utility.MoveDirWithLockedFile(TempExtractedPath, di.FullName);
    }

    private static string GetOldBinDir(DirectoryInfo di)
    {
        return Path.Combine(di.Parent.FullName, OLD_BIN_DIR);
    }
}
