using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinGenerator : INoiseGenerator {

    public int seed;

    private List<Vector3> gradientValues;

    public override float ValueAtPoint(Vector3 pos)
    {
        var originalRandomState = Random.state;
        Random.InitState(seed);

        int xi = Mathf.FloorToInt(pos.x);
        int yi = Mathf.FloorToInt(pos.y);
        int zi = Mathf.FloorToInt(pos.z);

        float x = pos.x - xi;
        float y = pos.y - yi;
        float z = pos.z - zi;

        float[] dots = new float[8];
        int cnt = 0;
        for (int xo = 0; xo <= 1; ++xo)
        {
            for (int yo = 0; yo <= 1; ++yo)
            {
                for (int zo = 0; zo <= 1; ++zo)
                {
                    int coordHash = (xi + xo) * 123 + (yi + yo) * 321 + (zi + zo) * 3;
                    var gradient = gradientValues[System.Math.Abs(coordHash) % 100];
                    var corner = new Vector3(xi + xo, yi + yo, zi + zo);
                    var distanceVector = corner - pos;
                    var dot = Vector3.Dot(gradient, distanceVector);
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

    // Use this for initialization
    void Awake()
    {
        if (seed == 0)
        {
            seed = (int)(Random.value * 1000000.0f);
        }

        var originalRandomState = Random.state;
        Random.InitState(seed);

        gradientValues = new List<Vector3>();
        for (int i = 0; i < 100; ++i)
        {
            gradientValues.Add(Random.onUnitSphere);
        }
        Random.state = originalRandomState;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
