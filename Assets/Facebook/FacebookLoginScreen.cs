using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SwipeDetector))]
public class FacebookLoginScreen : MonoBehaviour {
    public YetAnotherFacebookFeed feed;
    public bool showLogin;
    bool loggingIn;
    public string username="shlomo.zippel";
    private string password="";
    bool error;
    string errormsg;

	// Use this for initialization
	void Start () {
	    // make sure we have a swipe detector
        if (!GetComponent<SwipeDetector>()) {
            Debug.LogWarning("No swipe detector. Adding...");
            gameObject.AddComponent<SwipeDetector>();
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void Swipe_Up()
    {
        showLogin = !showLogin;
    }

    void OnGUI()
    {
        if (showLogin) {
            GUILayout.BeginArea(new Rect((Screen.width - 200) / 2, 20, 200, 150));
            GUILayout.BeginVertical();

            if (error) {
                GUILayout.Label(errormsg);
            }

            username = GUILayout.TextField(username);
            password = GUILayout.PasswordField(password, "*"[0]);

            if (loggingIn) {
                GUILayout.Label("Logging in...");
            }
            else {
                if (GUILayout.Button("Login")) {
                    loggingIn = true;
                    //StartCoroutine(GetComponent<FacebookLogin>().Login(username, password));
					// Shlomo: Commented it out
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }

    public void FacebookLogin_Success(Hashtable omercy)
    {
        showLogin = false;
        feed.Launch((string) omercy["token"]);
        transform.parent.GetComponent<NavigatorController>().NavigateToByName("FacebookFeed");
    }

    void FacebookLogin_BadToken()
    {
        loggingIn = false;
        error = true;
        errormsg = "Session timed out";
    }

    void FacebookLogin_LoginError()
    {
        loggingIn = false;
        error = true;
        errormsg = "Invalid username or password";
    }
}
