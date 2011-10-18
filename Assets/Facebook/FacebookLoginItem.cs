using UnityEngine;
using System.Collections;

public class FacebookLoginItem : MonoBehaviour {

    public FacebookLoginEntry Entry { get; private set; }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Init(FacebookLoginEntry entry)
    {
        Entry = entry;
        TextMesh name = transform.Find("Name").GetComponentInChildren<TextMesh>();
        name.text = entry.DisplayName;
        StartCoroutine(FBUtils.ImageFromIdAsync(entry.UserID, transform.Find("Thumbnail").renderer));
    }
}
