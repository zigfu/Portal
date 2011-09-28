using UnityEngine;
using System.Collections;

public class ThumbnailStripFeed : MonoBehaviour {
	public ScrollingMenu menu;
	public ThumbnailItem item;
	public Navigator navigator;
	public MenuSystemThingie menuSystemThingie;
	public TextMesh titleLabel;
	public TextMesh descriptionLabel;
	
	// Use this for initialization
	void Start () {
		foreach (MainMenuEntry mme in GetComponentsInChildren<MainMenuEntry>()) {
			AddThumbnailItem(mme);
		}
	}
	
	void AddThumbnailItem(MainMenuEntry mme)
	{
		ThumbnailItem newItem = Instantiate(item) as ThumbnailItem;
		item.transform.localRotation = Quaternion.identity;
		item.transform.localPosition = Vector3.zero;
		newItem.mme = mme;
		menu.Add(newItem.transform);
	}
	
	void Menu_Highlight(Transform item)
	{
		MainMenuEntry mme = item.gameObject.GetComponent<ThumbnailItem>().mme;
		titleLabel.text = mme.Title;
		descriptionLabel.text = mme.Description;
	}
	
	void Menu_Select(Transform item)
	{
		MainMenuEntry mme = item.gameObject.GetComponent<ThumbnailItem>().mme;
		if (mme.NavigateOnSelect && navigator) {
			navigator.NavigateTo(mme.NavigationTarget);
			if (menuSystemThingie) {
				menuSystemThingie.Set(mme.Title, mme.iconSelected, mme.targetCamera);
			}
		}
	}
	
	void OnGUI()
	{
		if (Event.current.Equals(Event.KeyboardEvent("escape"))) {
            if (GetComponent<HandPointControl>().IsActive) {
				Application.Quit();
				Event.current.Use();
			}
        }
	}
}
