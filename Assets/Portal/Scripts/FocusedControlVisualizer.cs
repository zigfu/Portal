using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
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
		CalcSizes();
		UpdatePositions();
	}
	
	void CalcSizes()
	{
		top = new Vector3(0, 0.5f * Size.y, 0);
		bottom = new Vector3(0, -0.5f * Size.y, 0);
		left = new Vector3(-0.5f * Size.x, 0, 0);
		right = new Vector3(0.5f * Size.x, 0, 0);
	}
	
	void UpdatePositions()
	{
		//arrowLeft.transform.localPosition = left + new Vector3(-horizontalArrowOffset.x, horizontalArrowOffset.y, 0);
		//arrowRight.transform.localPosition = right + new Vector3(horizontalArrowOffset.x, horizontalArrowOffset.y, 0);
		//arrowUp.transform.localPosition = top + new Vector3(verticalArrowOffset.x, -verticalArrowOffset.y, 0);
		//arrowDown.transform.localPosition = bottom + new Vector3(verticalArrowOffset.x, verticalArrowOffset.y, 0);
		
		UpdateScrollIndicator(scrollIndicatorLeft, 
		                      left + new Vector3(-scrollIndicatorOffset,0,0), 
		                      new Vector2(scrollIndicatorWidth, Size.y));
		
		
		UpdateScrollIndicator(scrollIndicatorRight, 
		                      right + new Vector3(scrollIndicatorOffset,0,0),
		                      new Vector2(scrollIndicatorWidth, Size.y));
		
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
		visualizer.Size = size;
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
