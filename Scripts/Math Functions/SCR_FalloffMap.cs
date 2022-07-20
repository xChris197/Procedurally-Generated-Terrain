using UnityEngine;

public static class SCR_FalloffMap
{
    /// <summary>
    /// Generates and returns a Falloff Map
    /// </summary>
    /// <param name="width">Width of the Falloff Map</param>
    /// <param name="height">height of the Falloff Map</param>
    /// <param name="falloffCurve">Curve to evaluate the falloff samples</param>
    /// <returns>A 2D array of floats representing the Falloff Map</returns>
    public static float[,] GenerateFalloffMap(int width, int height, AnimationCurve falloffCurve)
    {
        float[,] falloffMap = new float[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float xValue = y / (float) width * 2 - 1;
                float yValue = x / (float) height * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(xValue), Mathf.Abs(yValue));
                falloffMap[x, y] = falloffCurve.Evaluate(value);
            }
        }

        return falloffMap;
    }
}
