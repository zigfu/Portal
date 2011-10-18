using UnityEngine;
using System.Collections;

public class FBUtils {

    public static bool Recording;
    public static bool Offline;

    //TODO: find a way to hide the WWW request from callers and return the image directly
    //TODO: implement caching (dependent on the previous todo)
    public static WWW ImageFromFacebookID(string fromIDString)
    {
        string urlString = "https://graph.facebook.com/" + fromIDString + "/picture";
        WWW req = new WWW(urlString);
        return req;
    }

    // must be item that contains a "from" entry
    public static WWW ImageFromItem(Hashtable Item)
    {
        Hashtable fromTable = Item["from"] as Hashtable;
        string fromIDString = fromTable["id"] as string;
        
        return ImageFromFacebookID(fromIDString);
    }
    
    public static IEnumerator ImageFromIdAsync(string fromIDString, Renderer target)
    {
        if (Offline) {
            string[] sizeStr = System.IO.File.ReadAllLines("Offline\\Images\\" + fromIDString + "_size");
            Texture2D img = new Texture2D(int.Parse(sizeStr[0]), int.Parse(sizeStr[1]));
            img.LoadImage(System.IO.File.ReadAllBytes("Offline\\Images\\" + fromIDString));
            target.material.mainTexture = img;
            yield break;
        }

        WWW req = ImageFromFacebookID(fromIDString);
        yield return req;
        target.material.mainTexture = req.texture;

        if (Recording) {
            System.IO.File.WriteAllText("Offline\\Images\\" + fromIDString + "_size", req.texture.width.ToString() + "\r\n" + req.texture.height);
            System.IO.File.WriteAllBytes("Offline\\Images\\" + fromIDString, req.texture.EncodeToPNG());
        }
    }
}
