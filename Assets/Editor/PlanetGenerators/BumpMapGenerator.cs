using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumpMapGenerator : GeneratePlanet
{

    public INoiseGenerator noiseSource;
    public Texture2D guideTexture;
    public AnimationCurve colorCurve;
    public string bumpmapSaveDir;

    // From https://gamedev.stackexchange.com/questions/106703/create-a-normal-map-using-a-script-unity
    private Texture2D NormalMap(Texture2D source, float strength)
    {
        strength = Mathf.Clamp(strength, 0.0F, 1.0F);

        Texture2D normalTexture;
        float xLeft;
        float xRight;
        float yUp;
        float yDown;
        float yDelta;
        float xDelta;

        normalTexture = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false, true);
        //byte[] textureData = new byte[source.width * source.height * 4];

        for (int y = 0; y < normalTexture.height; y++)
        {
            for (int x = 0; x < normalTexture.width; x++)
            {
                xLeft = source.GetPixel(x - 1, y).grayscale * strength;
                xRight = source.GetPixel(x + 1, y).grayscale * strength;
                yUp = source.GetPixel(x, y - 1).grayscale * strength;
                yDown = source.GetPixel(x, y + 1).grayscale * strength;
                xDelta = ((xLeft - xRight) + 1) * 0.5f;
                yDelta = ((yUp - yDown) + 1) * 0.5f;

                //byte r = 127;
                //byte g = 127;

                //var color = new Color(0.5f, 0.5f, 1.0f, 0.5f);

                normalTexture.SetPixel(x, y, new Color(xDelta, yDelta, 1.0f, yDelta));
                //normalTexture.SetPixel(x, y, new Color(0, color.g, 0, color.a));
            }
        }
        //normalTexture.LoadRawTextureData(textureData);
        normalTexture.Apply();

        //Code for exporting the image to assets folder
        //System.IO.File.WriteAllBytes("Assets/NormalMap.png", normalTexture.EncodeToPNG());
        //System.IO.File.WriteAllBytes(bumpmapSaveDir, normalTexture.EncodeToPNG());

        return normalTexture;
    }

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
        var heightTex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        heightTex.filterMode = FilterMode.Point;

        var heightMap = new Color[width * height];

        var heightMapGuide = guideTexture;

        var colors = new Color[width * height];
        var albedoLevels = new Color[width * height];

        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                var guideHeight = heightMapGuide.GetPixelBilinear((float)x / width, -(float)y / height);

                var radius = Mathf.Cos(((float)y / height - 0.5f) * 3.14159265f);
                var pixelSpaceCoords = new Vector3(
                    Mathf.Sin((float)x / width * 3.14159265f * 2.0f) * radius,
                    Mathf.Sin(((float)y / height - 0.5f) * 3.14159265f),
                    Mathf.Cos((float)x / width * 3.14159265f * 2.0f) * radius
                );

                var noise = guideHeight.r * noiseSource.ValueAtPoint(pixelSpaceCoords*6.0f);
                var idx = x + width * y;
                if (noise < 0.15f)
                {
                    colors[idx] = Color.blue;
                    //colors[x + width * y].a = 0.4f;
                    albedoLevels[idx].r = 0.0f;
                    albedoLevels[idx].g = 0.0f;
                    albedoLevels[idx].b = 1.0f;
                    albedoLevels[idx].a = 0.9f;

                    heightMap[idx].r = heightMap[idx].g = heightMap[idx].b = heightMap[idx].a = 1 - Mathf.Pow(0.15f, 3.0f);
                }
                else
                {
                    colors[x + width * y] = ColorScale(colorCurve.Evaluate((noise - 0.15f) * (1.0f / 0.85f)), new List<Color> {
                        new Color(229.0f/255.0f, 172.0f/255.0f, 114.0f/255.0f),
                        new Color(0.0f, 1.0f, 0.0f),
                        new Color(0.5f, 0.4f, 0.0f),
                        new Color(1.0f, 1.0f, 1.0f),
                        new Color(1.0f, 1.0f, 1.0f)
                    });

                    //colors[idx] = Color.green;
                    //colors[x + width * y].a = 0.9f;
                    albedoLevels[idx].r = colors[idx].r;
                    albedoLevels[idx].g = colors[idx].g;
                    albedoLevels[idx].b = colors[idx].b;
                    albedoLevels[idx].a = 0.2f;

                    heightMap[idx].r = heightMap[idx].g = heightMap[idx].b = heightMap[idx].a = 1 - Mathf.Pow(noise, 3.0f);
                }

            }
        }

        heightTex.SetPixels(heightMap);
        color.SetPixels(colors);
        albedo.SetPixels(albedoLevels);

        heightTex.Apply();
        color.Apply();
        albedo.Apply();

        var bumpTex = NormalMap(heightTex, 1.0f);
        material.EnableKeyword("_SPECGLOSSMAP");
        //material.EnableKeyword("_NORMALMAP");
        //material.EnableKeyword("_TANGENT_TO_WORLD");
        //material.EnableKeyword("UNITY_TANGENT_ORTHONORMALIZE");
        material.SetTexture("_MainTex", color);
        material.SetTexture("_BumpMap", bumpTex);
        material.SetTexture("_SpecGlossMap", albedo);

        var totalTime = Time.realtimeSinceStartup - startTime;
        Debug.Log("Generating planet took " + totalTime + "s");
    }
}
