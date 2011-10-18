using UnityEngine;
using System.Collections;

public class SimpleVisualizerSliderThingie : MonoBehaviour {
	public Renderer target;
	public float blinkTime = 0.5f;
	public Color defaultColor;
	public Color selectedColor;
	public Color disabledColor;
	
	bool blinking;
	
	// Use this for initialization
	void Start () {
		if (null == target) {
			target = GetComponent<Renderer>();
		}
		
		target.material.color = defaultColor;
		
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
	
	public void DisableOMercy()
	{
		target.material.color = disabledColor;
	}
	
	public void EnableOMerciness()
	{
		target.material.color = defaultColor;
	}
	
	public void SetProgress(float progress)
	{
		if (blinking) return;
		target.material.SetFloat("_Cutoff", progress);
		Color c = target.material.color;
		c.a = progress;
        target.material.color = c;
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

