using UnityEngine;
using System.Collections;

public class FaderVisualizer : MonoBehaviour {
	public Fader target;
	public Vector2 Size = new Vector2(10, 0.5f);
	public Transform Nub;
	public float NubSpeed = 5;
	
	// Use this for initialization
	void Start () {
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Plane);
		go.transform.parent = transform;
		go.transform.localScale = new Vector3(.1f * Size.x, .1f, Size.y * .1f);
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation = Quaternion.Euler(270, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
		if (!target || !Nub) return;
		Vector3 delta = new Vector3((target.value - 0.5f) * Size.x, 0, 0);
		Nub.position = Vector3.Lerp(Nub.position, transform.position + delta, Time.deltaTime * NubSpeed);
	}
	
	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(gameObject.transform.position, new Vector3(Size.x, Size.y, 0.01f));
	}
}
