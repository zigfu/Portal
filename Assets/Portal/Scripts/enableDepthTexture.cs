using UnityEngine;
using System.Collections;

public class enableDepthTexture : MonoBehaviour {
	public Camera target;
	public DepthTextureMode mode = DepthTextureMode.Depth;
	

	// Update is called once per frame
	void FixedUpdate () {
		target.depthTextureMode = mode;
	}
}
