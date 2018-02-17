// Based on http://wiki.unity3d.com/index.php/CreateIcoSphere

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class CreatePlanet : ScriptableWizard
{
    public enum PlanetType
    {
        DesertPlanet,
        IslandPlanet,
        CustomBumpmapPlanet
    };
    //public PlanetType planetType;

    public int resolution = 128;
    public float radius = 0.5f;
    public bool addCollider = false;
    public string optionalName;

    // Parameters for the desert planet
    public int numberOfDesertPlanets = 32;
    public AnimationCurve desertColorCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

    // Parameters for the island planet
    public int numberOfIslandPlanets = 32;

    // Parameters for the custom bumpmap planet
    public int numberOfBumpmapPlanets = 1;
    public AnimationCurve bumpmapColorCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    public Texture2D bumpmapReferenceImage;

    [MenuItem("GameObject/Create Other/Planet...")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Create Planet", typeof(CreatePlanet));
    }


    void OnWizardUpdate()
    {
    }

    private GeneratePlanet CreateDesertPlanetGenerator()
    {
        var perlin = new PerlinGenerator();
        perlin.InitializeTables();

        var fractal = new ExponentialOctaveFractalNoiseGenerator();
        fractal.noiseSource = perlin;
        fractal.exponentialDecay = 0.7f;
        fractal.numberOfOctaves = 8;

        var generator = new DesertGenerator();
        generator.colorCurve = desertColorCurve;
        generator.resolution = resolution;
        generator.noiseSource = fractal;
        return generator;
    }

    private GeneratePlanet CreateIslandPlanetGenerator()
    {
        var perlin = new PerlinGenerator();
        perlin.InitializeTables();

        var fractal = new ExponentialOctaveFractalNoiseGenerator();
        fractal.noiseSource = perlin;
        fractal.exponentialDecay = 0.7f;
        fractal.numberOfOctaves = 8;

        var generator = new IslandGenerator();
        generator.resolution = resolution;
        generator.noiseSource = fractal;
        return generator;
    }

    private GeneratePlanet CreateBumpmapPlanetGenerator(string bumpmapSaveDir)
    {
        var perlin = new PerlinGenerator();
        perlin.InitializeTables();

        var fractal = new ExponentialOctaveFractalNoiseGenerator();
        fractal.noiseSource = perlin;
        fractal.exponentialDecay = 0.7f;
        fractal.numberOfOctaves = 8;

        var generator = new BumpMapGenerator();
        generator.bumpmapSaveDir = bumpmapSaveDir;
        generator.guideTexture = bumpmapReferenceImage;
        generator.colorCurve = bumpmapColorCurve;
        generator.resolution = resolution;
        generator.noiseSource = fractal;
        return generator;
    }

    private string PadNumber(int n, int length)
    {
        var s = n.ToString("D");
        return new string('0', length - s.Length) + s;
    }

    void OnWizardCreate()
    {
        for (var i = 0; i < numberOfDesertPlanets; ++i)
        {
            EditorUtility.DisplayProgressBar("Progess", "Planet", (float)i/64.0f);
            GeneratePlanetWithParameters("planet" + PadNumber(i, 3), PlanetType.DesertPlanet);
        }

        for (var i = 0; i < numberOfIslandPlanets; ++i)
        {
            EditorUtility.DisplayProgressBar("Progess", "Planet", (float)(i+32) / 64.0f);
            GeneratePlanetWithParameters("planet" + PadNumber(i+32, 3), PlanetType.IslandPlanet);
        }

        for (var i = 0; i < numberOfBumpmapPlanets; ++i)
        {
            GeneratePlanetWithParameters("planet" + PadNumber(i+32+32, 3), PlanetType.CustomBumpmapPlanet);
        }
        EditorUtility.ClearProgressBar();
    }

    void GeneratePlanetWithParameters(string name, PlanetType planetType)
    {
        GameObject planet = new GameObject();

        if (!string.IsNullOrEmpty(optionalName))
            planet.name = optionalName;
        else
            planet.name = "UvSphere";

        planet.transform.position = Vector3.zero;

        MeshFilter filter = (MeshFilter)planet.AddComponent(typeof(MeshFilter));
        planet.AddComponent(typeof(MeshRenderer));

        Mesh mesh = (Mesh)AssetDatabase.LoadAssetAtPath("Assets/Mesh/UvSphere20_20.asset", typeof(Mesh));

        filter.sharedMesh = mesh;
        mesh.RecalculateBounds();

        if (addCollider)
            planet.AddComponent(typeof(SphereCollider));

        var planetBasePath = "Assets/Resources/generated/" + name;

        var renderer = planet.GetComponent<Renderer>();
        var material = new Material((Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/PlanetMaterial.mat", typeof(Material))); //new Material(renderer.sharedMaterial);
        GeneratePlanet generator = null;
        switch (planetType)
        {
            case PlanetType.DesertPlanet: generator = CreateDesertPlanetGenerator(); break;
            case PlanetType.IslandPlanet: generator = CreateIslandPlanetGenerator(); break;
            case PlanetType.CustomBumpmapPlanet: generator = CreateBumpmapPlanetGenerator(planetBasePath + "_BumpMap.png"); break;
            default: Debug.Log("Specified an unknown planetType!"); return;
        }
        generator.material = material;
        generator.Generate();

        // Save all the textures and reload them
        var textureNames = new List<string>()
        {
            "_SpecGlossMap",
            "_MainTex",
            "_BumpMap"
        };

        foreach (var textureName in textureNames)
        {
            var tex = (Texture2D)material.GetTexture(textureName);
            if (tex != null)
            {
                System.IO.File.WriteAllBytes(planetBasePath + textureName + ".png", tex.EncodeToPNG());
                AssetDatabase.Refresh();
                material.SetTexture(textureName, (Texture2D)AssetDatabase.LoadAssetAtPath(planetBasePath + textureName + ".png", typeof(Texture2D)));
            }
        }

        var bumpTex = material.GetTexture("_BumpMap");
        if (bumpTex != null)
        {
            var path = planetBasePath + "_BumpMap.png";
            Debug.Log(path);
            var importer = (TextureImporter)TextureImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.NormalMap;
            AssetDatabase.ImportAsset(planetBasePath + "_BumpMap.png", ImportAssetOptions.ForceUpdate);
        }

        renderer.sharedMaterial = material;

        AssetDatabase.CreateAsset(material, planetBasePath + ".mat");
        AssetDatabase.SaveAssets();

        PrefabUtility.CreatePrefab(planetBasePath + ".prefab", planet);
        GameObject.DestroyImmediate(planet);
    }
}
