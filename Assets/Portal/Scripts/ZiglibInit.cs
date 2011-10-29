using UnityEngine;
using System.Collections;
using ZigLib;

public class ZiglibInit : MonoBehaviour {
    public bool TestMode = false;
    public string ConfigFile = "ziglib_test.cfg";
    public string TestServerPath = "http://test.zigfu.com/main";
    void Awake()
    {
        if (TestMode) {
            ZigLib.ZigLib.Init(ZigLib.ZigLib.GetDefaultZDBPath(), TestServerPath);
        }
    }
}
