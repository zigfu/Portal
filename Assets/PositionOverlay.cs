using UnityEngine;
using System.Collections;

public class PositionOverlay : MonoBehaviour {

    private Transform Target;
//    public Transform LeftIndicator;
//    public Transform RightIndictor;
    public Transform Overlay;
    public Renderer SelectTarget;
    private Transform oldTarget;

	// Use this for initialization
	void Start () {
        Overlay.gameObject.SetActiveRecursively(false);
	}

    // best code reuse ever
    //TODO: recursive?
    Bounds GetBoundingBox(GameObject go)
    {
        Bounds newBounds = new Bounds(go.transform.position, Vector3.zero);
        foreach (Renderer child in go.GetComponentsInChildren<Renderer>()) {
            newBounds.Encapsulate(child.bounds);
        }
        return newBounds;
    }

    private Bounds targetBounds;
    private Vector3 targetZeroToCenter;
    public float damping = 6;
	// Update is called once per frame
	void Update () {
        if (Target == null) {
            // make sure the object isn't displayed...
            // TODO: something?
            return;
        }
        if (oldTarget != Target) {
            //recalculate bbox
            targetBounds = GetBoundingBox(Target.gameObject);
            targetZeroToCenter = targetBounds.center - Target.position;
            oldTarget = Target;
        }

        Overlay.localPosition = targetZeroToCenter + Target.localPosition;
	}

    void Menu_Activate(Transform item)
    {
        Target = item;
        Overlay.gameObject.SetActiveRecursively(true);
    }
    IEnumerator Menu_Select()
    {
        //SelectTarget.material.color
        //TODO: animate color of "right" arrow
        //yield return new WaitForSeconds(0.15f);
		
		// Shlomo
        //SelectTarget.material.color = GetComponent<ScorchedOverlay>().scrollingArrowColor;
        yield return new WaitForSeconds(0.2f);
        //SelectTarget.material.color = GetComponent<ScorchedOverlay>().defaultArrowColor;
    }
    void Hand_Destroy()
    {
        Target = null;
        Overlay.gameObject.SetActiveRecursively(false);
    }

}
