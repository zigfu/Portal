using UnityEngine;
using System.Collections;

public class ClickOnEnter : MonoBehaviour {


    void OnGUI()
    {
        if (Event.current.type == EventType.KeyDown) {
            if (!GetComponent<HandPointControl>().IsActive) { return; }

            if (Event.current.keyCode == KeyCode.Return) {
                SendMessage("PushDetector_Click", SendMessageOptions.DontRequireReceiver);
                Event.current.Use();
            }
        }
    }

}
