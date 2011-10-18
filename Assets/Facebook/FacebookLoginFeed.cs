using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FacebookLoginEntry
{
    public string UserID { get; private set; }
    public string Username { get; private set; }
    public string DisplayName { get; private set; }
    public string AccessToken { get; private set; }

    public FacebookLoginEntry(string userId, string username, string displayName, string accessToken)
    {
        UserID = userId;
        Username = username;
        DisplayName = displayName;
        AccessToken = accessToken;
    }

    public Hashtable SaveToJSON()
    {
        return new Hashtable() { { "token", AccessToken}, { "name", DisplayName}, { "id", UserID}, { "username", Username} };
    }

    public void loadFromJSON(Hashtable entry)
    {
        UserID = (string)entry["id"];
        Username = (string)entry["username"];
        DisplayName = (string)entry["name"];
        AccessToken = (string)entry["token"];
    }

    public static FacebookLoginEntry LoadFromJSON(Hashtable entry)
    {
        return new FacebookLoginEntry((string)entry["id"], (string)entry["username"], (string)entry["name"], (string)entry["token"]);
    }
}

public class FacebookLoginFeed : MonoBehaviour {

    public Transform item;
    List<FacebookLoginEntry> users = new List<FacebookLoginEntry>();
    public bool OfflineLogin;

	// Use this for initialization
	void Start () {
        string usersTxt = System.IO.File.ReadAllText(".\\users.json");
        ArrayList MyUsers = JSON.JsonDecode(usersTxt) as ArrayList;
        foreach (object user in MyUsers) {
            FacebookLoginEntry entry = FacebookLoginEntry.LoadFromJSON(user as Hashtable);
            users.Add(entry);
            AddToMenu(entry);
        }
	}

    void FacebookLogin_Success(Hashtable omercy)
    {
        FacebookLoginEntry entry = FacebookLoginEntry.LoadFromJSON(omercy);
        bool bexists = false;
        foreach (FacebookLoginEntry user in users) {
            if (user.UserID == entry.UserID) {
                // update with new token
                user.loadFromJSON(omercy);
                SaveUsersList(".\\users.json");
                print("Updaing existing entry");
                bexists = true;
                break;
            }
        }
        if (!bexists) {
            users.Add(entry);
            AddToMenu(entry);
            SaveUsersList(".\\users.json");
        }
    }

    void AddToMenu(FacebookLoginEntry entry)
    {
        Transform newObj = Instantiate(item) as Transform;
        newObj.GetComponent<FacebookLoginItem>().Init(entry);
        SendMessage("Menu_Add", newObj);
    }

    void FacebookLogin_BadToken()
    {
        // error dialog & return to login screen
    }

    void FacebookLogin_LoginError()
    {
        // error dialog & return to login screen
    }

    void SaveUsersList(string filename)
    {
        ArrayList usersJson = new ArrayList();
        foreach (FacebookLoginEntry entry in users) {
            usersJson.Add(entry.SaveToJSON());
        }
        System.IO.File.WriteAllText(filename, JSON.JsonEncode(usersJson));
    }

    void Menu_Select(Transform item)
    {
        // fake it!!
        if (OfflineLogin) {
            print("Doing offline login for user: " + item.GetComponent<FacebookLoginItem>().Entry.DisplayName);
            Hashtable tokenHash = new Hashtable() { {"token", item.GetComponent<FacebookLoginItem>().Entry.AccessToken }};
            GetComponent<FacebookLoginScreen>().FacebookLogin_Success(tokenHash);
            return;
        }
        print("Trying to login as user " + item.GetComponent<FacebookLoginItem>().Entry.DisplayName);
        // Shlomo: Just commented this out
		// StartCoroutine(GetComponent<FacebookLogin>().CheckToken(item.GetComponent<FacebookLoginItem>().Entry.AccessToken));
    }
}
