using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//generating planet shape from generated noise
public class ShapeGenerator
{
    ShapeSettings settings;
    iNoiseFilter[] noiseFilters;
    public PlanetHeight elevationMinMax;
    public void UpdateSettings(ShapeSettings settings)
    {
        this.settings = settings;
        noiseFilters = new iNoiseFilter[settings.noiseLayers.Length];
        for(int i = 0; i < noiseFilters.Length; i++)
        {
            noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(settings.noiseLayers[i].noiseSettings);
        }
        elevationMinMax = new PlanetHeight();
    }
    public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere)
    { 
        float firstLayerValue = 0;
        float elevation = 0;

        if(noiseFilters.Length > 0)
        {
            firstLayerValue = noiseFilters[0].Evaluate(pointOnUnitSphere);
            if (settings.noiseLayers[0].enabled)
            {
                elevation = firstLayerValue;
            }
        }

        for(int i = 0; i < noiseFilters.Length; i++)
        {
            if (settings.noiseLayers[i].enabled)
            {
                float mask = (settings.noiseLayers[i].useFirstLayerAsMask) ? firstLayerValue : 1; //if using 1st layer as mask
                elevation += noiseFilters[i].Evaluate(pointOnUnitSphere) * mask;
            }
        }
        elevation = settings.planetRadius * (1 + elevation);
        elevationMinMax.AddValue(elevation);
        return pointOnUnitSphere * elevation;
    }
}
