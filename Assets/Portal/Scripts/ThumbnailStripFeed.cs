using UnityEngine;
using System.Collections;

public class ThumbnailStripFeed : MonoBehaviour {
	public ScrollingMenu menu;
	public ThumbnailItem item;
	public TextMesh titleLabel;
	public TextMesh descriptionLabel;
	
	// Use this for initialization
	void Start () {
		AddThumbnailItem("Applications", "List of installed applications");
		AddThumbnailItem("OpenNI Arena", "Browse the latest demos provided by the OpenNI community");
		AddThumbnailItem("Whats new", "Latest and greatest in the ZigFu empire");
		AddThumbnailItem("Settings", "Tweaking n'shit");
		AddThumbnailItem("Applications", "List of installed applications");
		AddThumbnailItem("OpenNI Arena", "Browse the latest demos provided by the OpenNI community");
		AddThumbnailItem("Whats new", "Latest and greatest in the ZigFu empire");
		AddThumbnailItem("Settings", "Tweaking n'shit");
		AddThumbnailItem("Applications", "List of installed applications");
		AddThumbnailItem("OpenNI Arena", "Browse the latest demos provided by the OpenNI community");
		AddThumbnailItem("Whats new", "Latest and greatest in the ZigFu empire");
		AddThumbnailItem("Settings", "Tweaking n'shit");
	}
	
	void AddThumbnailItem(string title, string description)
	{
		ThumbnailItem newItem = Instantiate(item) as ThumbnailItem;
		item.transform.localRotation = Quaternion.identity;
		item.transform.localPosition = Vector3.zero;
		newItem.Title = title;
		newItem.Description = description;
		menu.Add(newItem.transform);
	}
	
	void Menu_Highlight(Transform item)
	{
		ThumbnailItem ti = item.gameObject.GetComponent<ThumbnailItem>();
		titleLabel.text = ti.Title;
		descriptionLabel.text = ti.Description;
	}
}
