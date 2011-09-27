using UnityEngine;
using System.Collections;

public class FaderVisualizer : MonoBehaviour {
	public Fader target;
	public Vector2 Size = new Vector2(10, 0.5f);
	public float NubSpeed = 5;
	public NineSegment background;
	public NineSegment Nub;
	
	// Use this for initialization
	void Start () {
		if (null != background) {
			background.Size = Size;
			background.DoUpdate();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (null != background && background.Size != Size) {
			background.Size = Size;
			background.DoUpdate();
		}
		
		if (!target || !Nub) return;
		Vector3 targetPos = transform.TransformPoint(new Vector3((target.value - 0.5f) * (Size.x - (Nub.Size.x)), 0, 0f));
		Nub.transform.position = Vector3.Lerp(Nub.transform.position, targetPos, Time.deltaTime * NubSpeed);
	}
	
	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		DrawBox(Vector3.zero, new Vector3(Size.x, Size.y, 0f));
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
}
