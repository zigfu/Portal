using UnityEngine;
using System.Collections;

public class ChangeColorOnHighlight : MonoBehaviour {

	public Color defaultColor = Color.white;
	public Color highlightColor = Color.red;
	public Color selectColor = Color.green;
	
	public float rate = 3.0f;
	public Renderer target;
	
	private Color targetColor;
	
	// Use this for initialization
	void Start () {
		if (!target)	{
			target = renderer;
		}
		targetColor = defaultColor;
	}
	
	// Update is called once per frame
	void Update () {
		if (target) {
			target.material.color = Color.Lerp(target.material.color, targetColor, Time.deltaTime * rate);
		}
	}
	
	void MenuItem_Activate()
	{
		targetColor = highlightColor;
	}
	
	void MenuItem_Deactivate()
	{
		targetColor = defaultColor;
	}
	
	void MenuItem_Select()
	{
		targetColor = selectColor;
	}
}
