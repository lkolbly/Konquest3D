// Based on http://wiki.unity3d.com/index.php/CreateIcoSphere

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class CreatePlanet : ScriptableWizard
{

    public int numberOfSlices = 10;
    public int numberOfRings = 10;
    public float radius = 0.5f;
    public bool addCollider = false;
    public string optionalName;

    static Camera cam;
    static Camera lastUsedCam;

    [MenuItem("GameObject/Create Other/Planet...")]
    static void CreateWizard()
    {
        cam = Camera.current;
        // Hack because camera.current doesn't return editor camera if scene view doesn't have focus
        if (!cam)
            cam = lastUsedCam;
        else
            lastUsedCam = cam;
        ScriptableWizard.DisplayWizard("Create Planet", typeof(CreatePlanet));
    }


    void OnWizardUpdate()
    {
        if (numberOfSlices < 3) numberOfSlices = 3;
        if (numberOfRings < 4) numberOfRings = 4;
    }

    void OnWizardCreate()
    {
        GameObject planet = new GameObject();

        //AssetDatabase.CreateAsset(mesh, "Assets/Mesh/" + sphereAssetName);
        //AssetDatabase.SaveAssets();

        if (!string.IsNullOrEmpty(optionalName))
            planet.name = optionalName;
        else
            planet.name = "UvSphere";

        planet.transform.position = Vector3.zero;

        MeshFilter filter = (MeshFilter)planet.AddComponent(typeof(MeshFilter));
        planet.AddComponent(typeof(MeshRenderer));

        Mesh mesh = (Mesh)AssetDatabase.LoadAssetAtPath("Assets/Mesh/UvSphere20_20.asset", typeof(Mesh));

        // Generate the textures
        var perlin = new PerlinGenerator();
        perlin.seed = 123;
        perlin.InitializeTables();

        var fractal = new ExponentialOctaveFractalNoiseGenerator();
        fractal.noiseSource = perlin;
        fractal.exponentialDecay = 0.7f;
        fractal.numberOfOctaves = 8;

        var generator = new DesertGenerator();// new IslandGenerator();
        var renderer = planet.GetComponent<Renderer>();
        var material = new Material((Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/PlanetMaterial.mat", typeof(Material))); //new Material(renderer.sharedMaterial);
        generator.resolution = 128;
        generator.material = material;
        generator.noiseSource = fractal;
        generator.Generate();
        material.DisableKeyword("_NORMALMAP");
        renderer.sharedMaterial = material;
        //renderer.materials[0] = material;

        filter.sharedMesh = mesh;
        mesh.RecalculateBounds();

        if (addCollider)
            planet.AddComponent(typeof(SphereCollider));

        Selection.activeObject = planet;

        //var tex = material.GetTexture("_SpecGlossMap");
        //AssetDatabase.CreateAsset(tex, "Assets/Prefabs/Planets/generated/planettex000.asset");

        var tex = material.GetTexture("_MainTex");
        AssetDatabase.CreateAsset(tex, "Assets/Prefabs/Planets/generated/planettex000.asset");

        //tex = new Texture2D(material.GetTexture("_BumpMap"));
        //AssetDatabase.CreateAsset(tex, "Assets/Prefabs/Planets/generated/planetbump000.asset");

        AssetDatabase.CreateAsset(material, "Assets/Prefabs/Planets/generated/planet000.mat");
        AssetDatabase.SaveAssets();

        PrefabUtility.CreatePrefab("Assets/Prefabs/Planets/generated/planet000.prefab", planet);
        GameObject.DestroyImmediate(planet);
    }
}
