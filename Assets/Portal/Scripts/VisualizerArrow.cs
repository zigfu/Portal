using UnityEngine;
using System.Collections;

public class VisualizerArrow : MonoBehaviour {
	public Renderer arrow;
	public float blinkTime = 0.5f;
	public Color defaultColor;
	public Color selectedColor;
	public Color disabledColor;
	
	bool blinking;
	
	// Use this for initialization
	void Start () {
		if (null == arrow) {
			arrow = GetComponent<Renderer>();
		}
		
		arrow.materials[0].color = defaultColor;
		arrow.materials[1].color = selectedColor;
		
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
		arrow.materials[0].color = disabledColor;
	}
	
	public void EnableOMerciness()
	{
		arrow.materials[0].color = defaultColor;
	}
	
	public void SetProgress(float progress)
	{
		if (blinking) return;
        //TODO: not hardcoded to the second material?
		arrow.materials[1].SetFloat("_Cutoff", progress);
		Color c = arrow.materials[1].color;
		c.a = progress;
        arrow.materials[1].color = c;
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

