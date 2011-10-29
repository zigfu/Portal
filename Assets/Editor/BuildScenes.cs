using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Linq;

public class BuildScene {
    static string[] extraFiles = { "first.wav", "second.wav" };
    static string scene = "Assets/Portal/_Portal.unity";
    static string outputPath = "BuildOutput/Portal";
	
    [MenuItem("Build/Build Portal Executable")]
    static void Build()
    {   
		if (!Directory.Exists(outputPath)) {
			Directory.CreateDirectory(outputPath);
		}
		
		PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.HiddenByDefault;
		PlayerSettings.defaultIsFullScreen = true;
		PlayerSettings.defaultWebScreenWidth = 1280;
		PlayerSettings.defaultScreenHeight = 720;
		//PlayerSettings.defaultWebScreenWidth = 1024;
		//PlayerSettings.defaultScreenHeight = 768;
		
		string sceneName = Path.GetFileNameWithoutExtension(scene);
		Debug.Log("About to build " + sceneName);
		PlayerSettings.companyName = "ZigFu";
		PlayerSettings.productName = "Portal";
        string res = BuildPipeline.BuildPlayer(new string[] { scene }, getOutputPath("Portal"), BuildTarget.StandaloneWindows, BuildOptions.None);
        Debug.Log("result: " + res);
        foreach (string filename in extraFiles) {
            File.Copy(filename, Path.Combine(Path.Combine(outputPath, "bin"), Path.GetFileName(filename)), true);
        }
    }

    private static string getOutputPath(string scene)
    {
        return string.Format("{0}/bin/{1}.exe", outputPath, scene);
    }
}
