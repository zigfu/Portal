using UnityEngine;
using System.Collections;
using Accord.Statistics.Analysis;

[RequireComponent(typeof(HandPointControl))]
public class SteadyDetector : MonoBehaviour {
	public float maxVariance = 50;
	public float timedBufferSize = 0.5f;
	public float minSteadyTime = 0.1f;
	TimedBuffer<Vector3> points;

	public bool IsSteady { get; private set; }
	
	// Use this for initialization
	void Start () {
		points = new TimedBuffer<Vector3>(timedBufferSize);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void Hand_Create(Vector3 pos) {
		points.Clear();
		Hand_Update(pos);
	}
	
	void Hand_Update(Vector3 pos) {
		points.AddDataPoint(pos);
		bool currentFrameSteady = GetSingularValues().x < maxVariance;
		if (!IsSteady && currentFrameSteady) {
			StartCoroutine("WaitForSteady");
		}
		if (IsSteady && !currentFrameSteady) {
			StopCoroutine("WaitForSteady");
		}
		IsSteady = currentFrameSteady;
	}
	
	IEnumerator WaitForSteady()
	{
		yield return new WaitForSeconds(minSteadyTime);
		SendMessage("SteadyDetector_Steady", SendMessageOptions.DontRequireReceiver);
	}
	
	Vector3 GetSingularValues()
	{
		var buffer = points.Buffer;
		if (buffer.Count < 4) {
			return Vector3.zero;
		}
		
		double[,] output = new double[buffer.Count, 3];
		int i = 0;
		foreach(var pt in buffer) {
			Vector3 pos = pt.obj;
			output[i,0] = pos.x;
			output[i,1] = pos.y;
			output[i,2] = pos.z;
			i++;
		}
		PrincipalComponentAnalysis anal = new PrincipalComponentAnalysis(output);
		anal.Compute();

		return new Vector3((float)anal.SingularValues[0], 
		                   (float)anal.SingularValues[1],
		                   (float)anal.SingularValues[2]);
	}
	
	void SessionManager_Visualize()
	{
		GUILayout.Label("- SteadyDetector");
		GUI.color = (IsSteady) ? Color.green : Color.red;
		GUILayout.Label(IsSteady ? "STEADY" : "NOT STEADY");
	}
	
	public bool verbose = false;
	void SteadyDetector_Steady()
	{
		if (verbose) print ("SteadyDetector - Steady");
	}
}
