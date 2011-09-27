using UnityEngine;
using System.Collections;

public class VisualizerArrow : MonoBehaviour {
	public Renderer arrow;	
	public Color defaultColor;
	public Color selectedColor;
	
	// Use this for initialization
	void Start () {
		arrow.material.color = defaultColor;
	}
	
	public void OnSelect()
	{
		StartCoroutine(AnimateSelect());	
	}
	
	IEnumerator AnimateSelect()
	{
		arrow.material.color = selectedColor;
		yield return new WaitForSeconds(0.5f);
		arrow.material.color = defaultColor;
	}
}

