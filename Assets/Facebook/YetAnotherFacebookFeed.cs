using UnityEngine;
using System.Collections;
using System;

public class YetAnotherFacebookFeed : MonoBehaviour
{
    public Transform facebookItem;
    public bool AutoFetch = true;
    string URL = "https://graph.facebook.com/me/home?access_token=";
    public string token;

    string nextURL;
    string previousURL;
    bool fetching = false;

    public bool MakeOfflineRecording = false;
    public bool Offline = false;

    void Menu_OutOfBounds(bool forwards)
    {
        if (fetching || !forwards || !AutoFetch) {
            return;
        }

        //not yield return startcoroutine, so it can move on immediately
        StartCoroutine("fetch");
    }

    void Start()
    {
        if (MakeOfflineRecording && Offline) {
            Debug.LogWarning("Cant record and playback at the same time. Playing back...");
            MakeOfflineRecording = false;
        }
		
		Launch(token, "test", null);
    }
	
	public void Launch(FacebookLoginEntry entry)
	{
		Launch(entry.AccessToken, entry.DisplayName, entry.UserID);	
	}
	
    public void Launch(string token, string name, string id)
    {
        this.token = token;
        nextURL = URL + token;
        StartCoroutine("fetch");
		
		// init the MST
		MSTScreen mst = GetComponent<MSTScreen>();
		if (mst) {
			mst.Title = name;
			StartCoroutine(FBUtils.ImageFromIdAsync(id, mst.Icon));
		}
    }

    int PageNumber;

    string GetOfflineFilename()
    {
        string dir = "Offline"; // TODO fixme
        string filename ="OfflinePage" + PageNumber;
        return System.IO.Path.Combine(dir, filename);
    }

    // Use this for initialization
    IEnumerator fetch()
    {
        fetching = true;

        // offline support
        string data = "";
        if (Offline) {
            FBUtils.Offline = true;
            nextURL = GetOfflineFilename();
            data = System.IO.File.ReadAllText(nextURL);
        }
        else {
            // get list
            WWW www = new WWW(nextURL);
            yield return www;
            data = www.text;

            if (MakeOfflineRecording) {
                FBUtils.Recording = true;
                System.IO.File.WriteAllText(GetOfflineFilename(), www.text);
            }
        }

        PageNumber++;

        // TODO: check for errors (no auth, etc.)
        Hashtable queryResult = (Hashtable)JSON.JsonDecode(data);
        Hashtable paging = (Hashtable)queryResult["paging"];
        nextURL = (string)paging["next"];
        previousURL = (string)paging["previous"];

       
        ArrayList feedItems = (ArrayList)queryResult["data"];
        foreach (Hashtable item in feedItems) {
            Transform childTransform = Instantiate(facebookItem) as Transform;
            //yield return StartCoroutine(childTransform.GetComponent<FacebookItem>().Init(item, token));
			childTransform.GetComponent<FacebookItem>().Init(item, token);
            SendMessage("Menu_Add", childTransform);
			childTransform.localRotation = Quaternion.identity;
            //yield return null;
        }
        fetching = false;
    }

    // when an item is actually selected, do the whole camera thing
    IEnumerator Menu_Select(Transform item)
    {
        // wait for comments to load
        yield return StartCoroutine(item.GetComponent<FacebookItem>().LoadComments(item.GetComponent<FacebookItem>().PostId, token));

        // reposition menu items
        SendMessage("Menu_Reposition");

        /*
        // animate the active item to be on top of the screen
        GetComponent<ScorchedEarth>().SetFirstOnscreenItem(GetComponent<ScorchedEarth>().ActiveItemIndex);
        yield return new WaitForSeconds(1.0f);

        // duplicate self in seperate layer
        Transform copy = Instantiate(item, item.position, item.rotation) as Transform;
        copy.transform.parent = transform.parent;
        SetLayerRecursively(copy.gameObject, 8);

        // dup boom
        Transform boom = transform.Find("Boom");
        Transform dupBoom = Instantiate(boom, boom.position, boom.rotation) as Transform;
        dupBoom.parent = copy;

        Camera newCam = dupBoom.GetComponentInChildren<Camera>();
        Camera orig = GetComponentInChildren<Camera>();
        //newCam.CopyFrom(orig);
        newCam.cullingMask = (1 << copy.gameObject.layer);
        orig.cullingMask &= ~(1 << copy.gameObject.layer);

        // add comments to copy object
        copy.GetComponent<FacebookItem>().LoadComments(item.GetComponent<FacebookItem>().PostId, token);

        // navigate to our new item
        transform.parent.GetComponent<NavigatorController>().NavigateTo(copy);*/
    }

    void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform) {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
