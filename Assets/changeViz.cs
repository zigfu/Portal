using UnityEngine;
using System.Collections;

public class changeViz : MonoBehaviour {
	
	
	public Material LeftArrow_inactive;
	public Material LeftArrow_active;
	public Material RightArrow_inactive;
	public Material RightArrow_active;
	public Transform LeftArrow;
	public Transform RightArrow;
	public Transform ovalSelector;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void ItemSelect(int index)
	{
		if (index == 0)
		{
			LeftArrow.renderer.material = LeftArrow_active;
		}
		else
		{
			LeftArrow.renderer.material = LeftArrow_inactive;
		}
		if (index == 2)
		{
			RightArrow.renderer.material = RightArrow_active;
		}
		else
		{
			RightArrow.renderer.material = RightArrow_inactive;
		}
		
	}
	public Vector3 leftMost;
	public Vector3 rightMost;
	public void SetCursor(float fval)
	{
		ovalSelector.localPosition = Vector3.Lerp(leftMost,rightMost,fval);
	}
}
