using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExponentialOctaveFractalNoiseGenerator : INoiseGenerator {

    public INoiseGenerator noiseSource;
    public float exponentialDecay;
    public int numberOfOctaves;

    public override float ValueAtPoint(Vector3 p)
    {
        float multiplier = 1.0f;
        float scale = 1.0f;
        float result = 0.0f;
        for (int i = 0; i < numberOfOctaves; ++i)
        {
            result += noiseSource.ValueAtPoint(p * scale) * multiplier;
            scale *= 2.0f;
            multiplier *= exponentialDecay;
        }
        return result / 2.0f + 0.5f;
    }
}
