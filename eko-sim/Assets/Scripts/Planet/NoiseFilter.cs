using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//editing noise
public class NoiseFilter: iNoiseFilter
{
    Noise noise = new Noise();
    NoiseSettings settings;

    public NoiseFilter(NoiseSettings settings)
    {
        this.settings = settings;
    }

    public float Evaluate(Vector3 point)
    {
        // base player parameters
        float noiseValue = 0;
        float frequency = settings.baseRoughness;
        float amplitude = 1;

        for(int i = 0; i < settings.numLayers; i++)
        {
            float v = noise.Evaluate(point * frequency + settings.center);
            noiseValue += (v + 1) * 0.5f * amplitude;
            //editing values for additional layers
            frequency *= settings.roughness;
            amplitude *= settings.persistence;

        }
        noiseValue = Mathf.Max(0, noiseValue - settings.minValue); //applying minimum height
        return noiseValue * settings.strength;
    }
}
