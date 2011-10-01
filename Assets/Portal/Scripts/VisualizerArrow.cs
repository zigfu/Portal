using UnityEngine;
using System.Collections;

public class VisualizerArrow : MonoBehaviour {
	public Renderer arrow;
	public Renderer fill;
	public float blinkTime = 0.5f;
	public Color defaultColor;
	public Color selectedColor;
	
	bool blinking;
	
	// Use this for initialization
	void Start () {
		if (null == arrow) {
			arrow = GetComponent<Renderer>();
		}
		if (null == fill) {
			fill = transform.Find("Fill").gameObject.GetComponent<Renderer>();
		}
		
		Unhighlight();
	}
	
	public void Blink(float time, int reps)
	{
		StartCoroutine(AnimateSelect(time, reps));
	}
	
	public void Blink()
	{
		if (blinking) return;
		StartCoroutine(AnimateSelect(blinkTime, 1));	
	}
	
	public void Highlight()
	{
		SetProgress(1.0f);
	}
	
	public void Unhighlight()
	{
		SetProgress(0.0f);
	}
	
	public void SetProgress(float progress)
	{
		if (blinking) return;
		fill.material.SetFloat("_Cutoff", progress);
		Color c = fill.material.color;
		c.a = progress;
		fill.material.color = c;
	}
	
	IEnumerator AnimateSelect(float time, int reps)
	{
		blinking = true;
		for (int i=0; i<reps; i++) {
			SetProgress(1.0f);
			yield return new WaitForSeconds(time);
			SetProgress(0.0f);
		}
		blinking = false;
	}
}

