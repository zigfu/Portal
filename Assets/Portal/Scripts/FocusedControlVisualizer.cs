using UnityEngine;
using System.Collections;

public class FocusedControlVisualizer : MonoBehaviour {
	
	public Vector2 Size;
	public float IndicatorSize;
	public ScrollingMenu target;
	public bool HorizontalIndicator;
	public bool VerticalIndicator;
	
	public NineSegment frame;
	public NineSegment indicatorBackground;
	public NineSegment indicatorNub;
	
	Vector3 frameCenter;
	Vector3 frameSize;
	Vector3 vertCenter;
	Vector3 vertSize;
	Vector3 horizCenter;
	Vector3 horizSize;

	void Start()
	{
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
		frameSize = Size;
		frameCenter = Vector3.zero;
		
		if (HorizontalIndicator) {
			frameSize.y -= IndicatorSize;
			frameCenter.y += IndicatorSize / 2;
			horizCenter = new Vector3(-0.5f * IndicatorSize, -0.5f * (Size.y - IndicatorSize), 0);
			horizSize = new Vector3(Size.x - IndicatorSize, IndicatorSize, 0);
		}
		if (VerticalIndicator) {
			frameSize.x -= IndicatorSize;
			frameCenter.x -= IndicatorSize / 2;
			vertCenter = new Vector3(0.5f * (Size.x - IndicatorSize), 0.5f * IndicatorSize, 0);
			vertSize = new Vector3(IndicatorSize, Size.y - IndicatorSize, 0);
		}
	}
	
	void UpdatePositions()
	{
		if (frame) {
			frame.transform.position = transform.TransformPoint(frameCenter);
			frame.Size = frameSize;
		}
		
		if (HorizontalIndicator && indicatorBackground && indicatorNub)
		{
			indicatorBackground.transform.position = transform.TransformPoint(horizCenter);
			indicatorBackground.Size = horizSize;
		}
	}
	
	void OnDrawGizmos()
	{
		CalcSizes();
		
		// draw frame gizmo
		Gizmos.color = Color.yellow;
		DrawBox(frameCenter, frameSize);

		// draw indicator gizmos
		Gizmos.color = Color.green;
		if (HorizontalIndicator) {
			DrawBox(horizCenter, horizSize);
		}
		if (VerticalIndicator) {
			DrawBox(vertCenter, vertSize);
		}
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
