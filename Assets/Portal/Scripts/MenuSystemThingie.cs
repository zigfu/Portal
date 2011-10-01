using UnityEngine;
using System.Collections;

public class MenuSystemThingie : MonoBehaviour {
	public GameObject myCamera;
	public GameObject title;
	public GameObject icon;
	
    public void Set(string strTitle, Material matIcon)
    {
		title.GetComponent<TextMesh>().text = strTitle;
		icon.GetComponent<Renderer>().material = matIcon;
		myCamera.gameObject.SetActiveRecursively(true); // animate
	}

	void Navigator_ActivatedItem(Transform target)
	{
		MSTScreen info = target.gameObject.GetComponent<MSTScreen>();
		if (null != info)
		{
			Set(info.Title, info.Icon);
			info.Cam.rect = new Rect(0f, 0.0f, 1.0f, 0.7f); //animate
		}
		else 
		{
			myCamera.gameObject.SetActiveRecursively(false); //animate
		}
	}
	
	void OnGUI()
	{
		if (Event.current.Equals(Event.KeyboardEvent("escape"))) {
            if (GetComponent<HandPointControl>().IsActive) {
				GetComponent<Navigator>().NavigateBack();
				Event.current.Use();
			}
        }
	}
}
