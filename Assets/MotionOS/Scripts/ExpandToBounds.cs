using UnityEngine;
using System.Collections;

public class ExpandToBounds : MonoBehaviour {
    public Transform Background;
    public Transform Target;
    public Vector3 MinSize = new Vector3(1.0f, 1.0f, 1.0f);
    public Vector3 Padding = new Vector3(0.1f, 0.0f, 0.1f);

    public Vector3 targetScale;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Target) {
            ExpandToCover(Target.gameObject);
        }
	}

    public void ExpandToCover(GameObject go)
    {
        Bounds bounds = GetBoundingBox(go);
        bounds.Encapsulate(bounds.min + MinSize);
        bounds.Expand(Padding);
        Background.localScale = bounds.size;
    }

    Bounds GetBoundingBox(GameObject go)
    {
        Bounds newBounds = new Bounds(go.transform.position, Vector3.zero);
        foreach (Renderer child in go.GetComponentsInChildren<Renderer>()) {
            newBounds.Encapsulate(child.bounds);
        }
        return newBounds;
    }
}
