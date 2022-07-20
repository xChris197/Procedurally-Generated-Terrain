using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SCR_Noise
{
    /// <summary>
    /// Function to generate a height map with several variable to influence the final result
    /// </summary>
    /// <param name="width">Width of the map</param>
    /// <param name="height">Height of the map</param>
    /// <param name="seed">Change the result</param>
    /// <param name="scale">Changes the view distance</param>
    /// <param name="octaves">Changes the level of detail</param>
    /// <param name="persistence">How much the octaves contribute to the height shape</param>
    /// <param name="lacunarity">How much detail is added to the octaves</param>
    /// <returns>A fully generated height map</returns>
    public static float[,] GenerateNoise(int width, int height, int seed, float scale, int octaves, float persistence, float lacunarity)
    {
        float[,] heightMap = new float[width, height];

        float heightMinValue = float.MaxValue;
        float heightMaxValue = float.MinValue;

        Vector2[] seedOffsets = new Vector2[octaves];
        float offsetX;
        float offsetY;

        for (int i = 0; i < octaves; i++)
        {
            if (seed == 0)
            {
                offsetX = Random.Range(-100000f, 100000f) + seed;
                offsetY = Random.Range(-100000f, 100000f) + seed;
            }
            else if (seed > 100000)
            {
                seed = 100000;
                offsetX = seed;
                offsetY = seed;
            }
            else if (seed < -100000)
            {
                seed = -100000;
                offsetX = seed;
                offsetY = seed;
            }
            else
            {
                offsetX = seed;
                offsetY = seed;
            }
            
            seedOffsets[i] = new Vector2(offsetX, offsetY);
        }

        //Avoids a "Division by 0" error
        if (scale == 0)
        {
            scale = 0.1f;
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float frequency = 1;
                float amplitude = 1;
                float perlinHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float xSample = (x / scale) * frequency + seedOffsets[i].x * frequency;
                    float ySample = (y / scale) * frequency + seedOffsets[i].y * frequency;
                    float perlinSample = Mathf.PerlinNoise(xSample, ySample) * 2 - 1;
                    perlinHeight += perlinSample * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }
                //Adapted from Lague, 2016
                if (perlinHeight > heightMaxValue)
                {
                    heightMaxValue = perlinHeight;
                }
                else if (perlinHeight < heightMinValue)
                {
                    heightMinValue = perlinHeight;
                }

                heightMap[x, y] = perlinHeight;
            }
        }
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                heightMap[x, y] = Mathf.InverseLerp(heightMinValue, heightMaxValue, heightMap[x, y]);
            }
        }
        //End of adapted code
        return heightMap;
    }
}