using UnityEngine;
using System;
using System.Collections;
using OpenNI;

public enum OpenNIResolution {
    Default,
    QVGA,
    VGA
}
public class OpenNIContext : MonoBehaviour
{
    // singleton stuff
	static OpenNIContext instance;
	public static OpenNIContext Instance
	{
		get 
		{
			if (null == instance)
            {
                instance = FindObjectOfType(typeof(OpenNIContext)) as OpenNIContext;
                if (null == instance)
                {
                    GameObject container = new GameObject();
					DontDestroyOnLoad (container);
                    container.name = "OpenNIContextContainer";
                    instance = container.AddComponent<OpenNIContext>();
                }
				DontDestroyOnLoad(instance);
            }
			return instance;
		}
	}
	
	private Context context;
	public static Context Context 
	{
		get { return Instance.context; }
	}

    public DepthGenerator Depth { get; private set; }

	/*
    public bool Mirror
	{
		get { return mirrorCap.IsMirrored(); }
		set { if (!LoadFromRecording) mirrorCap.SetMirror(value); }
	}
	
	public static bool Mirror
	{
		get { return Instance.Mirror; }
		set { Instance.Mirror = value; }
	}
	*/

    private bool mirrorState;
    public bool Mirror;

	private MirrorCapability mirrorCap;
	
	public bool LoadFromRecording = false;
	public string RecordingFilename = "";
	public float RecordingFramerate = 30.0f;

    // Default key is NITE license from OpenNI.org
    public string LicenseKey = "0KOIk2JeIBYClPWVnMoRKn5cdY4=";
    public string LicenseVendor = "PrimeSense";

    public bool LoadFromXML = false;
    public string XMLFilename = ".\\OpenNI.xml";
	
    public OpenNIResolution DepthResolution = OpenNIResolution.Default;

	public OpenNIContext()
	{
	}

    // Tries to get an existing node, or opening a new one
    // if we need to
	private ProductionNode openNode(NodeType nt)
	{
        if (null == context) return null;

		ProductionNode ret=null;
		try
		{
			ret = context.FindExistingNode(nt);
		}
		catch
		{
			ret = context.CreateAnyProductionTree(nt, null);
			Generator g = ret as Generator;
			if (null != g)
			{
				g.StartGenerating();
			}
		}
		return ret;
	}
	
	public static ProductionNode OpenNode(NodeType nt)
	{
		return Instance.openNode(nt);
	}
	
	public bool error;
	public string errorMsg;
	
	public void Awake()
	{
        Debug.Log("Initing OpenNI" + (LoadFromXML ? "(" + XMLFilename + ")" : ""));
        try {
            this.context = LoadFromXML ? new Context(XMLFilename) : new Context();
        }
        catch (Exception ex) {
            Debug.LogError("Error opening OpenNI context: " + ex.Message);
			error = true;
			errorMsg = ex.Message;
            return;
        }

        // add license manually if not loading from XML
        if (!LoadFromXML) {
            License ll = new License();
            ll.Key = LicenseKey;
            ll.Vendor = LicenseVendor;
            context.AddLicense(ll);
        }

		if (LoadFromRecording)
		{
			context.OpenFileRecordingEx(RecordingFilename);
			Player player = openNode(NodeType.Player) as Player;
			player.PlaybackSpeed = 0.0;
			StartCoroutine(ReadNextFrameFromRecording(player));
		}
		
		try {
			this.Depth = openNode(NodeType.Depth) as DepthGenerator;
            if ((!LoadFromXML) && (DepthResolution != OpenNIResolution.Default)) {
                int desiredX = 320;
                switch (DepthResolution) {
                    case OpenNIResolution.QVGA:
                        desiredX = 320;
                        break;
                    case OpenNIResolution.VGA:
                        desiredX = 640;
                        break;
                }
                foreach (var mode in Depth.GetSupportedMapOutputModes()) {
                    if (mode.XRes == desiredX) {
                        this.Depth.MapOutputMode = mode;
                        print(string.Format("set output mode to {0}x{1}", mode.XRes, mode.YRes));
                        break;
                    }
                }
            }
		} catch (Exception ex) {
			error = true;
			errorMsg = "Error opening depth stream. Please make sure the sensor is connected";
			Debug.LogError("Error opening depth stream. Is the sensor connected? " + ex.Message);
			return;
		}
		this.mirrorCap = this.Depth.MirrorCapability;
        SetMirroring();
	}

    public void SetMirroring()
    {
        if (!LoadFromRecording) {
            this.mirrorCap.SetMirror(Mirror);
            mirrorState = Mirror;
        }
    }
	
	IEnumerator ReadNextFrameFromRecording(Player player)
	{
		while (true)
		{
			float waitTime = 1.0f / RecordingFramerate;
			yield return new WaitForSeconds (waitTime);
			player.ReadNext();
		}
	}
    public volatile bool UpdateContext = true;
	// (Since we add OpenNIContext singleton to a container GameObject, we get the MonoBehaviour functionality)
	public void Update () 
	{
        if (UpdateContext) {
            if (null == context) return;
            if (Mirror != mirrorState) {
                mirrorCap.SetMirror(Mirror);
                mirrorState = Mirror;
            }
            this.context.WaitNoneUpdateAll();
        }
	}
	
	public void OnApplicationQuit()
	{
        if (null == context) return;

		if (!LoadFromRecording) 
		{
			context.StopGeneratingAll();
		}
		// shutdown is deprecated, but Release doesn't do the job
		context.Shutdown();
		context = null;
		OpenNIContext.instance = null;
	}
}
