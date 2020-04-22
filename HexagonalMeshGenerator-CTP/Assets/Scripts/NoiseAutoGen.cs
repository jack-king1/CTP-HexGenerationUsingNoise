using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ElevationHeights
{
    public float noiseValue;
    public int elevation;
    public Color colour;
}

public class NoiseAutoGen : MonoBehaviour
{
    public static NoiseAutoGen Instance;
    float noiseResolution;
    Vector2 noiseOffset;
    float noiseScale; // larger = more islands & lakes
    public ElevationHeights[] evh;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("Instance Set");

        }
        noiseResolution = 0.01f;
        noiseOffset = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
        noiseScale = 0.01f; // larger = more islands & lakes
    }

    public float ElevationNoise(ref HexCell cell, float count)
    {
        float noise = Mathf.PerlinNoise((cell.coordinates.X / count) * 150, (cell.coordinates.Z / count) * 150);
        if (noise < 0)
        {
            noise = 0;
        }
        return noise;
    }
}
