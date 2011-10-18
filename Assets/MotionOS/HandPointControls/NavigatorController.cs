using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NavigatorController : MonoBehaviour {
	public bool ActiveOnStart = false;
	public bool NavigateHomeOnActivate = false;
	public bool NavigateHomeOnSessionEnd = false;
	public Transform HomeScreen;
    public Transform StartItem;
	public Transform ActiveItem { get; private set; }

	List<Transform> historyStack = new List<Transform>();

	public void NavigateTo(string name)
	{
		Transform obj = transform.Find(name);
		if (!obj) {
			Debug.LogError("Cannot navigate to " + name);
			return;
		}
		NavigateTo(obj);
	}
	
	public void NavigateTo(Transform obj)
	{
		DeactivateItem(ActiveItem);
		historyStack.Add(obj);
		ActivateItem(obj);
	}

    public void NavigateToByName(string name)
    {
        Transform obj = GameObject.Find(name).transform;
        NavigateTo(obj);
    }
	
	public void NavigateBack()
	{
		if (historyStack.Count <= 1) return;
        
        //deactivate and remove from history
        DeactivateItem(ActiveItem);
        int lastIndex = historyStack.Count - 1;
        historyStack.RemoveAt(lastIndex);

        //activate new item without adding it to history
        lastIndex = historyStack.Count - 1;
        ActivateItem(historyStack[lastIndex]);
	}
	
	public void NavigateHome()
	{
		historyStack.Clear();
		NavigateTo(HomeScreen);
	}
	
	void ActivateItem(Transform obj)
	{
		ActiveItem = obj;
		obj.SendMessage("Navigator_Activate", SendMessageOptions.DontRequireReceiver);
		SendMessage("Navigator_ActivatedItem", obj, SendMessageOptions.DontRequireReceiver);
	}
	
	void DeactivateItem(Transform obj)
	{
		if (!obj) return;
		obj.SendMessage("Navigator_Deactivate", SendMessageOptions.DontRequireReceiver);
	}

	void Start()
	{
		// make sure we get hand points
		if (null == GetComponent<HandPointControl>())
		{
			gameObject.AddComponent<HandPointControl>();
		}
		
		if (ActiveOnStart)
		{
			NavigateHome();
		}
	}
		
	// allow nesting of NavigatorController's
	void Navigator_Activate()
	{
		if (NavigateHomeOnActivate || null == ActiveItem)
		{
			NavigateHome();
		}
		else if (null != ActiveItem)
		{
			ActivateItem(ActiveItem);
		}
	}
	
	void Navigator_Deactivate()
	{
		if (null != ActiveItem)
		{
			DeactivateItem(ActiveItem);
		}
	}
	
	void Session_End()
	{
		if (NavigateHomeOnSessionEnd)
		{
			NavigateHome();
		}
	}
	
	void SessionManager_Visualize()
	{
		GUILayout.Label("- Navigator");
		GUILayout.Label("Active: " + ((null == ActiveItem) ? "[null]" : ActiveItem.name));
	}
}
