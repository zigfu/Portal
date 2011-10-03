using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TextMesh))]
public class TextShadow : MonoBehaviour {

    public Color ShadowColor = Color.black;
    public Vector3 Offset = new Vector3(0.1f, -0.1f, 0.1f);
    public Vector3 Scale = new Vector3(1.0f, 1.0f, 1.0f);

    TextMesh target;
    TextMesh shadowMesh;
    bool clone = false;
    // Use this for initialization
	void Start () {
        if (clone) {
            return;
        }
        
        target = GetComponent<TextMesh>();
        shadowMesh = (TextMesh)Instantiate(target);
        // TODO: hack of doom - find a better way to do this
        var ts = shadowMesh.GetComponent<TextShadow>();
        ts.clone = true;
        Destroy(ts);
        // done with hack of doom
        shadowMesh.renderer.material.color = ShadowColor;
        shadowMesh.transform.parent = transform; // make a child of the current text
        
        shadowMesh.renderer.material.renderQueue = renderer.material.renderQueue - 1; //make sure we draw the shadow after the current text
	}
	
	// Update is called once per frame
	void Update () {
        if (clone) return;

        if (shadowMesh.text != target.text) {
            shadowMesh.text = target.text;
        }
        //TODO: only on instantiate?
        shadowMesh.transform.localPosition = Offset;
        shadowMesh.transform.localScale = Scale;
	}
}
