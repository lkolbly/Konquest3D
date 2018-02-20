// C# example.
using UnityEngine;
using UnityEditor;
//using System.Diagnostics;

public class ScriptBatch
{
    [MenuItem("Konquest3D Tools/Build Executables")]
    public static void BuildGame()
    {
        // Get filename.
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
        Debug.Log(path);

        // Build the NoVR player
        string[] levels = new string[] { "Assets/noVrLobby.unity" };
        BuildPipeline.BuildPlayer(levels, path + "/novr.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);

        // Build the VR player


        // Copy a file from the project folder to the build folder, alongside the built game.
        //FileUtil.CopyFileOrDirectory("Assets/Templates/Readme.txt", path + "Readme.txt");

        // Run the game (Process class from System.Diagnostics).
        //Process proc = new Process();
        //proc.StartInfo.FileName = path + "/BuiltGame.exe";
        //proc.Start();
    }
}
