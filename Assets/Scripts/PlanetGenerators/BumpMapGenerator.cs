using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumpMapGenerator : GeneratePlanet
{

    public INoiseGenerator noiseSource;

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

        normalTexture = new Texture2D(source.width, source.height, TextureFormat.RGBA32, true);
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

                byte r = 127;
                byte g = 127;
                
                //normalTexture.SetPixel(x, y, new Color(xDelta, yDelta, 1.0f, yDelta));
                normalTexture.SetPixel(x, y, new Color32(g, g, g, r));
            }
        }
        //normalTexture.LoadRawTextureData(textureData);
        normalTexture.Apply();

        //Code for exporting the image to assets folder
        //System.IO.File.WriteAllBytes("Assets/NormalMap.png", normalTexture.EncodeToPNG());

        return normalTexture;
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
                if (noise < 0.5f)
                {
                    colors[idx] = Color.blue;
                    //colors[x + width * y].a = 0.4f;
                    albedoLevels[idx].r = 0.0f;
                    albedoLevels[idx].g = 0.0f;
                    albedoLevels[idx].b = 1.0f;
                    albedoLevels[idx].a = 0.9f;
                }
                else
                {
                    colors[idx] = Color.green;
                    //colors[x + width * y].a = 0.9f;
                    albedoLevels[idx].r = 0.0f;
                    albedoLevels[idx].g = 1.0f;
                    albedoLevels[idx].b = 0.0f;
                    albedoLevels[idx].a = 0.2f;
                }

                heightMap[idx].r = noise;
                heightMap[idx].g = noise;
                heightMap[idx].b = noise;
                heightMap[idx].a = noise;
            }
        }

        heightTex.SetPixels(heightMap);
        color.SetPixels(colors);
        albedo.SetPixels(albedoLevels);

        heightTex.Apply();
        color.Apply();
        albedo.Apply();

        //material.EnableKeyword("_SPECGLOSSMAP");
        material.EnableKeyword("_NORMALMAP");
        material.SetTexture("_MainTex", color);
        material.SetTexture("_BumpMap", NormalMap(heightTex, 1.0f));
        material.SetTexture("_SpecGlossMap", albedo);

        var totalTime = Time.realtimeSinceStartup - startTime;
        Debug.Log("Generating planet took " + totalTime + "s");
    }
}
