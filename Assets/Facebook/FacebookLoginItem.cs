using UnityEngine;
using System.Collections;

public class FacebookLoginItem : MonoBehaviour {

    public FacebookLoginEntry Entry { get; private set; }
	public TextMesh name;
	public Renderer thumb;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Init(FacebookLoginEntry entry)
    {
        Entry = entry;
        name.text = entry.DisplayName;
        StartCoroutine(FBUtils.ImageFromIdAsync(entry.UserID, thumb));
    }
}
