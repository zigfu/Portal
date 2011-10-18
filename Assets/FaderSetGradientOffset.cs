using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(Fader))]
public class FaderSetGradientOffset : MonoBehaviour {

    public Renderer target;
    public Fader fader;

    public Vector2 direction = Vector2.up;
    private float targetPos = 0.0f;
    public float damping = 10f;

	// Use this for initialization
	void Start () {
        if (fader == null) {
            fader = GetComponent<Fader>();
            if (fader == null) {
                Debug.LogError("no fader for SliderGradient :(");
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        
        //TODO: only when not pushing?
        targetPos = fader.value - 0.5f; //-0.5 to 0.5
        target.material.mainTextureOffset = Vector2.Lerp(target.material.mainTextureOffset, direction.normalized * targetPos, Time.deltaTime*damping);
	}

    
}
