using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratePlanet : MonoBehaviour {

    public Material material;

    private List<Vector3> gradientValues;

    private float ComputePerlin3D(Vector3 pos)
    {
        var originalRandomState = Random.state;
        Random.InitState(123456);

        if (gradientValues == null)
        {
            gradientValues = new List<Vector3>();
            for (int i = 0; i < 100; ++i)
            {
                gradientValues.Add(Random.onUnitSphere);
            }
        }

        int xi = Mathf.FloorToInt(pos.x);
        int yi = Mathf.FloorToInt(pos.y);
        int zi = Mathf.FloorToInt(pos.z);

        float x = pos.x - xi;
        float y = pos.y - yi;
        float z = pos.z - zi;

        //float runningTotal = 0.0f;
        float[] dots = new float[8];
        int cnt = 0;
        for (int xo = 0; xo <= 1; ++xo)
        {
            for (int yo = 0; yo <= 1; ++yo)
            {
                for (int zo = 0; zo <= 1; ++zo)
                {
                    int coordHash = (xi+xo) * 123 + (yi+yo) * 321 + (zi+zo) * 3;
                    var gradient = gradientValues[System.Math.Abs(coordHash) % 100];
                    var corner = new Vector3(xi+xo, yi+yo, zi+zo);
                    var distanceVector = corner - pos;
                    var dot = Vector3.Dot(gradient, distanceVector);
                    //tal += dot * Vector3.Magnitude(distanceVector);
                    dots[cnt] = dot;
                    cnt++;
                }
            }
        }

        /*
         *      3 --- 7
         *    / |   / |
         *   1 --- 5  |
         *   |  2 -|- 6
         *   | /   | /
         *   0 --- 4
         *   
         *   Z  Y
         *   ^ /
         *   |/
         *   o--> X
         */
        var floorValue = Mathf.Lerp(
            Mathf.Lerp(dots[0], dots[4], x),
            Mathf.Lerp(dots[2], dots[6], x),
            y
        );
        var ceilingValue = Mathf.Lerp(
            Mathf.Lerp(dots[1], dots[5], x),
            Mathf.Lerp(dots[3], dots[7], x),
            y
        );
        var result = Mathf.Lerp(floorValue, ceilingValue, z);

        Random.state = originalRandomState;
        return result;
    }

    private float ComputePerlinOctave(Vector3 p, int seed, float scale)
    {
        return ComputePerlin3D(p * scale + new Vector3(seed * 1.0f, 0, 0)) / 2.0f + 0.5f;
    }

	// Use this for initialization
	void Start () {
        var startTime = Time.realtimeSinceStartup;

        int width = 512;
        int height = 512;
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

                var cloud = ComputePerlinOctave(pixelSpaceCoords, 1, scale);

                //var noise = Mathf.PerlinNoise((float)x / 3.0f, (float)y / 3.0f);
                var noise = ComputePerlinOctave(pixelSpaceCoords, 0, scale);
                //Debug.Log(noise);
                //color.SetPixel(x, y, noise > 0.5 ? Color.blue : Color.green);
                if (noise > 0.5)
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
                }
                //colors[x + width * y].r = 1.0f;// = Color.white;
                /*if (cloud > 0.5)
                {
                    colors[x + width * y].r = 1.0f;
                    colors[x + width * y].g = 1.0f;
                    colors[x + width * y].b = 1.0f;
                    colors[x + width * y].a = 0.0f;

                    albedoLevels[x + width * y].r = 1.0f;
                    albedoLevels[x + width * y].g = 1.0f;
                    albedoLevels[x + width * y].b = 1.0f;
                    albedoLevels[x + width * y].a = 0.0f;
                }*/

                heightMap[x, y] = noise;
                //color.SetPixel(x, y, ComputePerlin3D(new Vector3(x/3.0f, y/3.0f, 3.0f)) > 0.0 ? Color.red : Color.green);
                //color.SetPixel(x, y, x%10 == 0 || y%10 == 0 ? Color.red : Color.green);
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
