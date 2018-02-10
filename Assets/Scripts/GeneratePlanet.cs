using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratePlanet : MonoBehaviour {

    public Material material;

    public INoiseGenerator noiseSource;

    private Color ColorScale(float value, List<Color> palette)
    {
        float scaleValue = value * (palette.Count - 1);
        int idx = Mathf.FloorToInt(scaleValue);
        float remainder = scaleValue - idx;

        if (idx == palette.Count - 1) return palette[palette.Count - 1];

        Color color1 = palette[idx];
        Color color2 = palette[idx + 1];
        return Color.Lerp(color1, color2, remainder);
    }

    // Use this for initialization
    void Start () {
        var startTime = Time.realtimeSinceStartup;

        int width = 64;
        int height = 64;
        var color = new Texture2D(width, height, TextureFormat.ARGB32, false);
        color.filterMode = FilterMode.Point;
        var normal = new Texture2D(width, height, TextureFormat.ARGB32, false);
        normal.filterMode = FilterMode.Point;
        var albedo = new Texture2D(width, height, TextureFormat.ARGB32, false);
        albedo.filterMode = FilterMode.Point;

        var heightMap = new float[width,height];
        var clouds = new float[width,height];

        var colors = new Color[width*height];
        var normalColors = new Color[width*height];
        var albedoLevels = new Color[width * height];

        var scale = 9.0f;

        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                var radius = Mathf.Cos(((float)y / height - 0.5f) * 3.14159265f);// * scale;
                var pixelSpaceCoords = new Vector3(
                    Mathf.Cos((float)x / width * 3.14159265f * 2.0f) * radius,
                    Mathf.Sin((float)x / width * 3.14159265f * 2.0f) * radius,
                    Mathf.Sin(((float)y / height - 0.5f) * 3.14159265f)// * scale
                );

                var noise = noiseSource.ValueAtPoint(pixelSpaceCoords);
                colors[x + width * y] = ColorScale(noise, new List<Color> {
                    Color.blue,
                    Color.green,
                    new Color(0.5f, 0.4f, 0.0f),
                    new Color(1.0f, 1.0f, 1.0f)
                });
                colors[x + width * y].a = 0.9f;
                /*if (noise > 0.5)
                {
                    // Water
                    colors[x + width * y] = Color.blue;
                    albedoLevels[x + width * y].r = 0.0f;
                    albedoLevels[x + width * y].g = 0.0f;
                    albedoLevels[x + width * y].b = 1.0f;
                    albedoLevels[x + width * y].a = 0.9f;
                }
                else
                {
                    // Land
                    colors[x + width * y] = Color.green;
                    albedoLevels[x + width * y].r = 0.0f;
                    albedoLevels[x + width * y].g = 1.0f;
                    albedoLevels[x + width * y].b = 0.0f;
                    albedoLevels[x + width * y].a = 0.2f;
                }*/

                heightMap[x, y] = noise;
            }
        }

        var heightStrength = 0.2f;
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                var x0 = x == 0 ? width - 1 : x - 1;
                var x0val = heightMap[x0, y];
                var x1val = heightMap[(x + 1) % width, y];
                var y0 = y == 0 ? height - 1 : y - 1;
                var y0val = heightMap[x, y0];
                var y1val = heightMap[x, (y + 1) % height];
                //normal.SetPixel(x, y, new Color(
                normalColors[x + width * y].r = 0.5f;// (x1val - x0val) * heightStrength + 0.5f;
                normalColors[x + width * y].g = 0.5f;// (y1val - y0val) * heightStrength + 0.5f;
                normalColors[x + width * y].b = 1;
                normalColors[x + width * y].a = 1;
                //));
            }
        }

        color.SetPixels(colors);
        normal.SetPixels(normalColors);
        albedo.SetPixels(albedoLevels);

        normal.Apply();
        color.Apply();
        albedo.Apply();

        //material.EnableKeyword("_NORMALMAP");
        material.EnableKeyword("_SPECGLOSSMAP");
        material.SetTexture("_MainTex", color);
        //material.SetTexture("_BumpMap", normal);
        material.SetTexture("_SpecGlossMap", albedo);

        var totalTime = Time.realtimeSinceStartup - startTime;
        Debug.Log("Generating planet took "+totalTime+"s");
    }
    
    // Update is called once per frame
    void Update () {
        
    }
}
