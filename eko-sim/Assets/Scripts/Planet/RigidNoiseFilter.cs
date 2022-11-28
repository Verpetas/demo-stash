using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//creates sharp terrain
public class RigidNoiseFilter : iNoiseFilter
{
    Noise noise = new Noise();
    NoiseSettings settings;

    public RigidNoiseFilter(NoiseSettings settings)
    {
        this.settings = settings;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0;
        float frequency = settings.baseRoughness;
        float amplitude = 1;
        float weight = 1;
        for (int i = 0; i < settings.numLayers; i++)
        {
            float v = 1-Mathf.Abs(noise.Evaluate(point * frequency + settings.center)); //Invert and absolute value of noise
            v *= v; //pow 2
            v *= weight;
            weight = v; //making details at top more detailed
            noiseValue += v * amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistence;

        }
        noiseValue = Mathf.Max(0, noiseValue - settings.minValue);
        return noiseValue * settings.strength;
    }
}
