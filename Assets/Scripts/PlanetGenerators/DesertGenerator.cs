using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertGenerator : GeneratePlanet {

    public INoiseGenerator noiseSource;
    public AnimationCurve colorCurve;

    private Color ColorScale(float value, List<Color> palette)
    {
        float scaleValue = value * (palette.Count - 1);
        int idx = Mathf.FloorToInt(scaleValue);
        float remainder = scaleValue - idx;

        if (idx >= palette.Count - 1) return palette[palette.Count - 1];
        if (idx < 0) return palette[0];

        Color color1 = palette[idx];
        Color color2 = palette[idx + 1];
        return Color.Lerp(color1, color2, remainder);
    }

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

        var scale = 9.0f;

        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                var radius = Mathf.Cos(((float)y / height - 0.5f) * 3.14159265f);// * scale;
                var pixelSpaceCoords = new Vector3(
                    Mathf.Sin((float)x / width * 3.14159265f * 2.0f) * radius,
                    Mathf.Sin(((float)y / height - 0.5f) * 3.14159265f),
                    Mathf.Cos((float)x / width * 3.14159265f * 2.0f) * radius
                );

                var noise = noiseSource.ValueAtPoint(pixelSpaceCoords);
                colors[x + width * y] = ColorScale(colorCurve.Evaluate(noise), new List<Color> {
                    new Color(229.0f/255.0f, 172.0f/255.0f, 114.0f/255.0f),
                    new Color(0.5f, 0.4f, 0.0f),
                    new Color(1.0f, 1.0f, 1.0f)
                });
                colors[x + width * y].a = 0.9f;

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
