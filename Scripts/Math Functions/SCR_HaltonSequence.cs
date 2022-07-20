using System.Collections.Generic;
using UnityEngine.Rendering;

public static class SCR_HaltonSequence
{
    /// <summary>
    /// Generates and returns a Halton Sequence
    /// </summary>
    /// <param name="heightMap">Used for the maps height and width</param>
    /// <returns>A list of Halton float values</returns>
    public static List<float> GenerateSequence(float[,] heightMap)
    {
        List<float> haltonSequencePoints = new List<float>();

        int mapHeight = heightMap.GetLength(0);
        int mapWidth = heightMap.GetLength(1);
        for (int y = 0; y < mapHeight; y++)
        {
            float haltonPoint = HaltonSequence.Get(y, mapHeight);
            haltonSequencePoints.Add(haltonPoint);
        }
        return haltonSequencePoints;
    }
}
