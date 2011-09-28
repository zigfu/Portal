using UnityEngine;
using System.Collections;

public class FocusedControlVisualizer : MonoBehaviour {
	
	public Vector2 Size;

	public float IndicatorSize;
	public bool HorizontalIndicator;
	public bool VerticalIndicator;
	
	public NineSegment frame;
	
	public Vector2 horizontalArrowOffset;
	public Vector2 verticalArrowOffset;
	public float scrollIndicatorWidth;
	public float scrollIndicatorOffset;
	
	public bool ShowArrowLeft;
	public bool ShowArrowRight;
	public bool ShowArrowTop;
	public bool ShowArrowBottom;
	public bool ShowIndicatorLeft;
	public bool ShowIndicatorRight;
	public bool ShowIndicatorTop;
	public bool ShowIndicatorBottom;
	
	public VisualizerArrow arrowRight;
	public VisualizerArrow arrowLeft;
	public VisualizerArrow arrowUp;
	public VisualizerArrow arrowDown;
	
	public FaderVisualizer scrollIndicatorLeft;
	public FaderVisualizer scrollIndicatorRight;
	public FaderVisualizer scrollIndicatorTop;
	public FaderVisualizer scrollIndicatorBottom;

	Vector3 top;
	Vector3 left;
	Vector3 right;
	Vector3 bottom;
	
	public Fader verticalFader;
	public Fader horizontalFader;

	void Start()
	{
		if (null != horizontalFader) {
			scrollIndicatorTop.target = horizontalFader;
			scrollIndicatorBottom.target = horizontalFader;			
		}
		if (null != verticalFader) {
			scrollIndicatorLeft.target = verticalFader;
			scrollIndicatorRight.target = verticalFader;
		}
		
		CalcSizes();
		UpdatePositions();
	}
	
	void Update()
	{
		EnableAndDisableComponents();
		CalcSizes();
		UpdatePositions();
	}
	
	void EnableAndDisableComponents()
	{
		if (null != scrollIndicatorLeft && ShowIndicatorLeft != scrollIndicatorLeft.gameObject.active) {
			scrollIndicatorLeft.gameObject.SetActiveRecursively(ShowIndicatorLeft);
		}
		if (null != scrollIndicatorRight && ShowIndicatorRight != scrollIndicatorRight.gameObject.active) {
			scrollIndicatorRight.gameObject.SetActiveRecursively(ShowIndicatorRight);
		}
		if (null != scrollIndicatorTop && ShowIndicatorTop != scrollIndicatorTop.gameObject.active) {
			scrollIndicatorTop.gameObject.SetActiveRecursively(ShowIndicatorTop);
		}
		if (null != scrollIndicatorBottom && ShowIndicatorBottom != scrollIndicatorBottom.gameObject.active) {
			scrollIndicatorBottom.gameObject.SetActiveRecursively(ShowIndicatorBottom);
		}
		
		if (ShowArrowLeft != arrowLeft.gameObject.active) {
			arrowLeft.gameObject.SetActiveRecursively(ShowArrowLeft);
		}
		if (ShowArrowRight != arrowRight.gameObject.active) {
			arrowRight.gameObject.SetActiveRecursively(ShowArrowRight);
		}
		if (ShowArrowTop != arrowUp.gameObject.active) {
			arrowUp.gameObject.SetActiveRecursively(ShowArrowTop);
		}
		if (ShowArrowBottom != arrowDown.gameObject.active) {
			arrowDown.gameObject.SetActiveRecursively(ShowArrowBottom);
		}
	}
	
	void CalcSizes()
	{
		if (null != frame && frame.Size != Size) {
			frame.Size = Size;
			frame.DoUpdate();
		}
		
		top = new Vector3(0, 0.5f * Size.y, 0);
		bottom = new Vector3(0, -0.5f * Size.y, 0);
		left = new Vector3(-0.5f * Size.x, 0, 0);
		right = new Vector3(0.5f * Size.x, 0, 0);
	}
	
	void UpdatePositions()
	{
		if (null != arrowLeft) arrowLeft.transform.localPosition = left + new Vector3(-horizontalArrowOffset.x, horizontalArrowOffset.y, 0);
		if (null != arrowRight) arrowRight.transform.localPosition = right + new Vector3(horizontalArrowOffset.x, horizontalArrowOffset.y, 0);
		if (null != arrowUp) arrowUp.transform.localPosition = top + new Vector3(verticalArrowOffset.x, -verticalArrowOffset.y, 0);
		if (null != arrowDown) arrowDown.transform.localPosition = bottom + new Vector3(verticalArrowOffset.x, verticalArrowOffset.y, 0);
		
		UpdateScrollIndicator(scrollIndicatorLeft, 
		                      left + new Vector3(-scrollIndicatorOffset,0,0), 
		                      new Vector2(Size.y, scrollIndicatorWidth));
		
		UpdateScrollIndicator(scrollIndicatorRight, 
		                      right + new Vector3(scrollIndicatorOffset,0,0),
		                      new Vector2(Size.y, scrollIndicatorWidth));
		
		UpdateScrollIndicator(scrollIndicatorTop, 
		                      top + new Vector3(0,scrollIndicatorOffset,0),
		                      new Vector2(Size.x, scrollIndicatorWidth));
		
		UpdateScrollIndicator(scrollIndicatorBottom, 
		                      bottom + new Vector3(0,-scrollIndicatorOffset,0),
		                      new Vector2(Size.x, scrollIndicatorWidth));
	}
	
	void UpdateScrollIndicator(FaderVisualizer visualizer, Vector3 localPos, Vector2 size)
	{
		if (null == visualizer) return;
		
		visualizer.transform.localPosition = localPos;
		if (visualizer.Size != Size) {
			visualizer.Size = size;
		}
	}
	
	void OnDrawGizmos()
	{
		// draw frame gizmo
		Gizmos.color = Color.yellow;
		DrawBox(Vector3.zero, new Vector3(Size.x, Size.y, 0));
	}
	
	// ignores z for size
	void DrawBox(Vector3 center, Vector3 size)
	{
		Vector3 p1 = transform.TransformPoint(center + (Vector3.Scale(size, new Vector3(-0.5f, -0.5f, 1f))));
		Vector3 p2 = transform.TransformPoint(center + (Vector3.Scale(size, new Vector3(0.5f, -0.5f, 1f))));
		Vector3 p3 = transform.TransformPoint(center + (Vector3.Scale(size, new Vector3(0.5f, 0.5f, 1f))));
		Vector3 p4 = transform.TransformPoint(center + (Vector3.Scale(size, new Vector3(-0.5f, 0.5f, 1f))));
		Gizmos.DrawLine(p1, p2);
		Gizmos.DrawLine(p2, p3);
		Gizmos.DrawLine(p3, p4);
		Gizmos.DrawLine(p4, p1);
	}
	
	void DrawBox(Vector2 min, Vector2 size)
	{
		Vector3 p1 = WorldPosition(min);
		Vector3 p2 = WorldPosition(min + new Vector2(size.x, 0));
		Vector3 p3 = WorldPosition(min + size);
		Vector3 p4 = WorldPosition(min + new Vector2(0, size.y));
		Gizmos.DrawLine(p1, p2);
		Gizmos.DrawLine(p2, p3);
		Gizmos.DrawLine(p3, p4);
		Gizmos.DrawLine(p4, p1);
	}
			                
	Vector3 WorldPosition(Vector2 local) 
	{
		return transform.TransformPoint(new Vector3(local.x, local.y, 0));
	}
}
