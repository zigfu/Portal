using UnityEngine;
using System.Collections;

public enum MenuOrientation
{
	Horizontal,
	Vertical
}

[RequireComponent(typeof(ScrollingMenu))]
public class ScrollingMenuObserverForVisualizationOMercy : MonoBehaviour {
	public ScrollingMenu menu;
	public VisualizerArrow nextArrow;
	public VisualizerArrow prevArrow;
		
	void Start()
	{
		if (null == menu) {
			menu = GetComponent<ScrollingMenu>();
		}
		ShowOrHideArrows();
	}
	
	void ItemSelector_Next()
	{
		if (null == nextArrow) return;
		nextArrow.Highlight();
		ShowOrHideArrows();
	}
	
	void ItemSelector_Prev()
	{
		if (null == prevArrow) return;
		prevArrow.Highlight();
		ShowOrHideArrows();
	}
		
	void ItemSelector_StopScrolling()
	{
		if (null != nextArrow) nextArrow.Unhighlight();
		if (null != prevArrow) prevArrow.Unhighlight();
		ShowOrHideArrows();
	}
	
	void ShowOrHideArrows()
	{
		if (null != nextArrow) {
			if (menu.CanScrollForward) {
				nextArrow.EnableOMerciness();
			} else {
				nextArrow.DisableOMercy();
			}
		}
		
		if (null != prevArrow) {
			if (menu.CanScrollBack) {
				prevArrow.EnableOMerciness();
			} else {
				prevArrow.DisableOMercy();
			}
		}
	}
}
