using UnityEngine;
using System;
using System.Collections;

public class ZyprApi : MonoBehaviour {
	
	static string APIKey = "d853337bd9a8fadd58204cafe1fe1720";
	
	// Use this for initialization
	void Start () {
		StartCoroutine(GetToken("shlomo@zigfu.com", "password"));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
		
	public IEnumerator GetToken(string user, string pass)
	{
		string req = String.Format("https://pais-dev.zypr.net/api/v2/auth/login/?key={0}&username={1}&password={2}", APIKey, user, pass);
		WWW loginReq = new WWW(req);
		yield return loginReq;
		
		// Response format (good flow)
		/* {"response":
			{"information":
				{"hostname":"pais-dev.zypr.net",
				"serverclock":1320894337.84,
				"entity":"auth",
				"verb":"login",
				"host":"10.194.43.95",
				"diagnostics":[],
				"code":200,
				"status":"OK",
				"timing":
					{"main":
						{"start":1320894337.83,
						"elapsed":4.003,
						"end":1320894341.84}}},
				"data":
					[{"config": {"latestversion":"2.0"},
					  "token":"7912caf3ab257bc907db2e14adc0abd29c20f276fd54bfa3e",
					  "deviceid":"2f3b18a202f56a9180a96ec6c02da256"}],
				"action":[]}
			} 
			
		Bad flow:
		{"response":{"information":{"hostname":"pais-dev.zypr.net","serverclock":1320897278.91,"entity":"auth","verb":"login","host":"10.195.43.200","diagnostics":[],"code":401,"status":"Unauthorized","error_hint":"origin=pais","timing":{"main":{"start":1320897278.91,"elapsed":0.047,"end":1320897278.96}}},"data":[],"action":[]}}	
		*/
		
		// Check result code first (200 is good)
		Hashtable result = JSON.JsonDecode(loginReq.text) as Hashtable;
		int code = (int)(double)((result["response"] as Hashtable)["information"] as Hashtable)["code"];
		if (200 == code) {
			print("Login success");
			Hashtable info = ((result["response"] as Hashtable));
			Hashtable firstItem = ((info["data"] as ArrayList)[0] as Hashtable);
			string token = (string)firstItem["token"];
			SendMessage("ZyprApi_LoginSuccess", token, SendMessageOptions.DontRequireReceiver);
		} else {
			print("Login failed");
			SendMessage("ZyprApi_LoginFailure", code, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	string tokenLastLogin;
	void ZyprApi_LoginSuccess(string token)
	{
		tokenLastLogin = token;
	}
}
