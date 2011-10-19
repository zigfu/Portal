using UnityEngine;
using System.Collections;

public class LoadLevelOnNavigate : MonoBehaviour 
{
    public string level;

    public void Navigator_Activate()
    {
		print("About to load " + level);
        UnityEngine.Application.LoadLevel(level);
    }
}
