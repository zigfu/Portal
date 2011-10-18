using UnityEngine;
using System.Collections;
using System;

public class FacebookItem : MonoBehaviour {
    public Transform FacebookItemHeader;
    public Transform FacebookItemLikes;
    public Transform FacebookItemComment;
    
    public int maxTextLength = 60;

    Hashtable item;
    string token;
    Transform header;
    Transform likes;

    public string PostId
    {
        get
        {
            return item["id"] as string;
        }
    }

	// Use this for initialization
	void Start () {
        if (!FacebookItemHeader) {
            Debug.LogError("No item header");
        }
        if (!FacebookItemComment) {
            Debug.LogError("No item likes");
        }
        if (!FacebookItemComment) {
            Debug.LogError("No item comment");
        }
	}

    public void Init(Hashtable item, string token)
    {
        this.item = item;
        this.token = token;

        // msg
        string message = item["message"] as string;
        if (!String.IsNullOrEmpty(message)) {
            message = TextTools.WordWrap(message, maxTextLength);
        }
        else {
            message = string.Format("[{0}]", item["type"]);
        }

        //likes
        int likesCount = 0;
        Hashtable likesItem = item["likes"] as Hashtable;
        if (null != likesItem && likesItem.ContainsKey("count")) {
            double likesCountDouble = (double)likesItem["count"];
            likesCount = (int)likesCountDouble;
        }

        //comments
        int commentsCount = 0;
        Hashtable comments = item["comments"] as Hashtable;
        if (null != comments && comments.ContainsKey("count")) {
            double commentsCountDouble = (double)comments["count"];
            commentsCount = (int)commentsCountDouble;
        }

        Hashtable from = item["from"] as Hashtable;
        

        // Create menuitemheader & set text fields
        header = Instantiate(FacebookItemHeader) as Transform;
        header.Find("NameText").GetComponent<TextMesh>().text = from["name"] as string;
        header.Find("ContentText").GetComponent<TextMesh>().text = message;
       
        // add the item to our submenu (before loading images/comments)
        SendMessage("Menu_Add", header);

        likes = Instantiate(FacebookItemLikes) as Transform;
        likes.Find("LikesText").GetComponent<TextMesh>().text       = string.Format("{0} Likes",    likesCount);
        likes.Find("CommentsText").GetComponent<TextMesh>().text    = string.Format("{0} Comments", commentsCount);
        SendMessage("Menu_Add", likes);

        GetComponent<ExpandToBounds>().Background = transform.Find("BackgroundContainer");
        GetComponent<ExpandToBounds>().ExpandToCover(transform.Find("ItemsContainer").gameObject);

        // load thumbnail
        StartCoroutine(FBUtils.ImageFromIdAsync(from["id"] as string, header.Find("Thumbnail").gameObject.renderer));
        //StartCoroutine(LoadImageAsync(header.Find("Thumbnail").gameObject.renderer, FBUtils.ImageFromItem(item)));
    }

    string offsetString = "&offset={0}";
    string baseURL = "https://graph.facebook.com/{0}/comments?access_token={1}&limit={2}";

    string getCommentsURL(string postId, string token)
    {
        return string.Format(baseURL, postId, token, 10);
    }

    //TODO: implement async get of the rest of the comments
    string getCommentsWithOffset(string postId, string token, int offset)
    {
        return getCommentsURL(postId, token) + string.Format(offsetString, 10);
    }

    IEnumerator LoadCommentsAsync(WWW req)
    {
        // wait for comments to load
        yield return req;

        GetComponent<ExpandToBounds>().Background = transform.Find("BackgroundContainer");

        // parse results
        Hashtable result = JSON.JsonDecode(req.text) as Hashtable;
        ArrayList comments = (ArrayList)result["data"];
        foreach (object commentString in comments) {
            Hashtable comment = (Hashtable)commentString;

            // Create menuitemheader & set text fields
            Transform commentItem = Instantiate(FacebookItemComment) as Transform;
            SetLayerRecursively(commentItem.gameObject, gameObject.layer);
            commentItem.Find("NameText").GetComponent<TextMesh>().text = (comment["from"] as Hashtable)["name"] as string;
            commentItem.Find("ContentText").GetComponent<TextMesh>().text = TextTools.WordWrap(comment["message"] as string, maxTextLength - 10);

            /*
            // TODO: something with like number?
            int likes = 0;
            if (comment.ContainsKey("likes")) {
                likes = (int)(double)comment["likes"];
            }
             * */

            // add the item to our submenu (before loading images/comments)
            SendMessage("Menu_Add", commentItem);

            // load thumbnail
            StartCoroutine(LoadImageAsync(commentItem.Find("Thumbnail").gameObject.renderer, FBUtils.ImageFromItem(comment)));

            // resize bg
            GetComponent<ExpandToBounds>().ExpandToCover(transform.Find("ItemsContainer").gameObject);
        }
    }

    public IEnumerator LoadComments(string postId, string token)
    {
        WWW req = new WWW(getCommentsURL(postId, token));
        yield return StartCoroutine(LoadCommentsAsync(req));
    }

    IEnumerator LoadImageAsync(Renderer target, WWW image)
    {
        yield return image;
        if (image.error == null) {
            target.material.mainTexture = image.texture;
        }
    }

    void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform) {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    void Swipe_Left()
    {
        // back
        transform.parent.GetComponent<NavigatorController>().NavigateBack();
    }

    void Navigator_Deactivate()
    {
        // self destruct
        Destroy(gameObject);
    }
}
