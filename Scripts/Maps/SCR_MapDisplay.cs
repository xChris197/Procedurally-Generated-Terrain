using UnityEngine;

public class SCR_MapDisplay : MonoBehaviour
{
    [SerializeField] private Renderer mapRend;
    [SerializeField] private MeshRenderer meshRend;
    
    /// <summary>
    /// Lerps each colour map element to get a colour between black (0) and white (1)
    /// Sets a texture using the new colour map to create the Noise Map
    /// </summary>
    /// <param name="noiseMap">Previously generated noise map for the width and length of the maps</param>
    public void DrawNoiseMap(float[,] noiseMap)
    {
        int mapWidth = noiseMap.GetLength(0);
        int mapHeight = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(mapWidth, mapHeight);
        texture.filterMode = FilterMode.Point;
        Color[] colourMap = new Color[mapWidth * mapHeight];

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                colourMap[y * mapWidth + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }
        texture.SetPixels(colourMap);
        texture.Apply();
        mapRend.sharedMaterial.mainTexture = texture;
    }

    /// <summary>
    /// Loops through each terrain type and sets a colour within the colour map
    /// Generates texture from the colour map to be applied
    /// </summary>
    /// <param name="noiseMap">Previously generated noise map</param>
    /// <param name="terrainTypes">List of terrain types containing several height and colour values</param>
    /// <param name="bMeshMaterial">Flag to render either the colour map or the mesh map</param>
    public void DrawColourMap(float[,] noiseMap, float[,] falloffMap, SCR_TerrainData[] terrainTypes, bool bMeshMaterial)
    {
        int mapWidth = noiseMap.GetLength(0);
        int mapHeight = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(mapWidth, mapHeight);
        texture.filterMode = FilterMode.Point;
        Color[] colourMap = new Color[mapWidth * mapHeight];

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                
                float heightIndex = noiseMap[x, y];

                for (int i = 0; i < terrainTypes.Length; i++)
                {
                    if (heightIndex <= terrainTypes[i].terrainHeight)
                    {
                        colourMap[y * mapWidth + x] = terrainTypes[i].terrainColour;
                        break;
                    }
                }
            }
        }

        texture.SetPixels(colourMap);
        texture.Apply();

        if (!bMeshMaterial)
        {
            mapRend.sharedMaterial.mainTexture = texture;
        }
        else
        {
            meshRend.sharedMaterial.mainTexture = texture;
        }
    }

    /// <summary>
    /// Draws the falloff map colours onto the texture plane
    /// </summary>
    /// <param name="falloffMap">The 2D array of floats for the falloff map</param>
    public void DrawFalloffMap(float[,] falloffMap)
    {
        int mapWidth = falloffMap.GetLength(0);
        int mapHeight = falloffMap.GetLength(1);

        Texture2D texture = new Texture2D(mapWidth, mapHeight);
        texture.filterMode = FilterMode.Point;
        Color[] colourMap = new Color[mapWidth * mapHeight];

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                colourMap[y * mapWidth + x] = Color.Lerp(Color.black, Color.white, falloffMap[x, y]);
            }
        }
        texture.SetPixels(colourMap);
        texture.Apply();
        mapRend.sharedMaterial.mainTexture = texture;
    }
}
