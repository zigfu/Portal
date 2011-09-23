using UnityEngine;
using System.Collections;

public class ItemSelectorVisualizer : MonoBehaviour {
	public ItemSelector target;
	public Vector2 Size = new Vector2(10, 0.5f);
	public Transform Nub;
	public float NubSpeed = 5;
	
	GameObject goBack;
	GameObject goNext;
	GameObject[] goItems;
	
	// Use this for initialization
	void Start () {
		// create scroll region segments
		if (target.scrollRegion > 0) {
			goBack = CreateVisualizerSegment(0, target.scrollRegion);
			goNext = CreateVisualizerSegment(1.0f - target.scrollRegion, 1.0f);
		}
		
		// create individual item segments
		goItems = new GameObject[target.numItems];
		float curr = target.scrollRegion;
		for (int i=0; i<target.numItems; i++, curr+=target.itemWidth) {
			goItems[i] = CreateVisualizerSegment(curr, curr+target.itemWidth);
		}
	}
	
	GameObject CreateVisualizerSegment(float min, float max)
	{
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Plane);
		go.transform.parent = transform;
		go.transform.localRotation = Quaternion.Euler(270, 0, 0);
		SetSegmentBounds(go, min, max);
		return go;
	}
	
	void SetSegmentBounds(GameObject go, float min, float max)
	{
		float size = max - min;
		go.transform.localScale = 0.1f * new Vector3(Size.x * size, 1, Size.y);
		go.transform.localPosition = (((max + min) * 0.5f) - 0.5f) * Size.x * Vector3.right;;
	}
	
	// Update is called once per frame
	void Update () {
		// move nub
		if (target && Nub) {
			Vector3 delta = new Vector3((target.fader.value - 0.5f) * Size.x, 0, 0);
			Nub.position = Vector3.Lerp(Nub.position, transform.position + delta, Time.deltaTime * NubSpeed);
		}
	}
	
	// react to item selector events for visualization
	
	
	void OnDrawGizmos()
	{
		if (!target) {
			Gizmos.color = Color.red;
			DrawBox(0, 1);	
			return;
		}
		
		// create scroll region segments
		if (target.scrollRegion > 0) {
			Gizmos.color = Color.blue;
			DrawBox(0, target.scrollRegion);
			DrawBox(1.0f - target.scrollRegion, 1.0f);
		}
		
		// create individual item segments
		Gizmos.color = Color.green;
		float curr = target.scrollRegion;
		for (int i=0; i<target.numItems; i++, curr+=target.itemWidth) {
			DrawBox(curr, curr+target.itemWidth);
		}
	}
	
	void DrawSegmentGizmo(float min, float max)
	{
		Vector3 vecFrom = gameObject.transform.position + transform.TransformPoint((Size.x * (min - 0.5f)) * Vector3.right);
		Vector3 vecTo = gameObject.transform.position + transform.TransformPoint((Size.x * (max - 0.5f)) * Vector3.right);
		Gizmos.DrawLine(vecFrom, vecTo);
	}
	
	void DrawBox(float min, float max)
	{
		Vector3 p1 = WorldPosition(Vector2.Scale(Size, new Vector2(min - 0.5f, -0.5f)));
		Vector3 p2 = WorldPosition(Vector2.Scale(Size, new Vector2(max - 0.5f, -0.5f)));
		Vector3 p3 = WorldPosition(Vector2.Scale(Size, new Vector2(max - 0.5f, 0.5f)));
		Vector3 p4 = WorldPosition(Vector2.Scale(Size, new Vector2(min - 0.5f, 0.5f)));
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
