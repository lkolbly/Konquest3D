using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandGenerator : GeneratePlanet
{

    public INoiseGenerator noiseSource;

    public override void Generate()
    {
        var startTime = Time.realtimeSinceStartup;

        int width = resolution;
        int height = resolution;
        var color = new Texture2D(width, height, TextureFormat.ARGB32, false);
        color.filterMode = FilterMode.Point;
        var albedo = new Texture2D(width, height, TextureFormat.ARGB32, false);
        albedo.filterMode = FilterMode.Point;

        var heightMap = new float[width, height];

        var colors = new Color[width * height];
        var albedoLevels = new Color[width * height];

        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                var radius = Mathf.Cos(((float)y / height - 0.5f) * 3.14159265f);
                var pixelSpaceCoords = new Vector3(
                    Mathf.Sin((float)x / width * 3.14159265f * 2.0f) * radius,
                    Mathf.Sin(((float)y / height - 0.5f) * 3.14159265f),
                    Mathf.Cos((float)x / width * 3.14159265f * 2.0f) * radius
                );

                var noise = noiseSource.ValueAtPoint(pixelSpaceCoords);
                var idx = x + width * y;
                if (noise < 0.75f)
                {
                    colors[idx] = Color.green;
                    //colors[x + width * y].a = 0.4f;
                    albedoLevels[idx].r = 0.0f;
                    albedoLevels[idx].g = 0.0f;
                    albedoLevels[idx].b = 1.0f;
                    albedoLevels[idx].a = 0.9f;
                }
                else
                {
                    colors[idx] = Color.blue;
                    //colors[x + width * y].a = 0.9f;
                    albedoLevels[idx].r = 0.0f;
                    albedoLevels[idx].g = 1.0f;
                    albedoLevels[idx].b = 0.0f;
                    albedoLevels[idx].a = 0.2f;
                }

                heightMap[x, y] = noise;
            }
        }

        color.SetPixels(colors);
        albedo.SetPixels(albedoLevels);

        color.Apply();
        albedo.Apply();

        material.EnableKeyword("_SPECGLOSSMAP");
        material.SetTexture("_MainTex", color);
        material.SetTexture("_SpecGlossMap", albedo);

        var totalTime = Time.realtimeSinceStartup - startTime;
        Debug.Log("Generating planet took " + totalTime + "s");
    }
}
