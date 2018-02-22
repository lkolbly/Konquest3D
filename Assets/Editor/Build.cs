// C# example.
using UnityEngine;
using UnityEditor;
//using System.Diagnostics;

public class ScriptBatch
{
    private static void BuildNoVR(string path)
    {
        // Build the NoVR player
        string[] levels = new string[] { "Assets/noVrLobby.unity", "Assets/multiplayer.unity" };
        BuildPipeline.BuildPlayer(levels, path + "/novr.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
    }

    private static void BuildFull(string path)
    {
        // Build the full VR player
        string[] levels = new string[] { "Assets/mainMenu.unity", "Assets/lobby.unity", "Assets/multiplayer.unity", "Assets/konquest3d.unity" };
        BuildPipeline.BuildPlayer(levels, path + "/full.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
    }

    private static void BuildSingleTest(string path)
    {
        // Build the singleplayer test VR player
        string[] levels = new string[] { "Assets/konquest3d.unity" };
        BuildPipeline.BuildPlayer(levels, path + "/singleplayer.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
    }

    private static void BuildMultiTest(string path)
    {
        // Build the multiplayer test VR player
        string[] levels = new string[] { "Assets/lobby.unity", "Assets/multiplayer.unity" };
        BuildPipeline.BuildPlayer(levels, path + "/multiplayer.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
    }

    [MenuItem("Konquest3D Tools/Build No VR Test")]
    public static void BuildNoVRTest()
    {
        // Get filename.
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "Builds");
        Debug.Log(path);

        BuildNoVR(path);
    }

    [MenuItem("Konquest3D Tools/Build Executables")]
    public static void BuildGame()
    {
        // Get filename.
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "Builds");
        Debug.Log(path);

        BuildFull(path);
        BuildNoVR(path);
        BuildSingleTest(path);
        BuildMultiTest(path);

        // Copy a file from the project folder to the build folder, alongside the built game.
        //FileUtil.CopyFileOrDirectory("Assets/Templates/Readme.txt", path + "Readme.txt");

        // Run the game (Process class from System.Diagnostics).
        //Process proc = new Process();
        //proc.StartInfo.FileName = path + "/BuiltGame.exe";
        //proc.Start();
    }
}
