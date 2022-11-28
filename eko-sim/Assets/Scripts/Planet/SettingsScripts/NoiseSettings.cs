using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseSettings
{
    public enum FilterType { Simple, Rigid };
    public FilterType filterType;

    public float strength = 1; //how strong it is
    public float roughness = 2; //how detailed it is
    public Vector3 center; //to move noise
    [Range(1, 8)]
    public int numLayers = 1; //how many layers
    public float persistence = 0.5f; //amplitude of each subsequent layer
    public float baseRoughness = 1; //base detail
    public float minValue; //minimum planet height
}
