using UnityEngine;

public class SCR_MapGenerator : MonoBehaviour
{
    private enum MapType {Noise, Colour, Mesh, FallOff}
    public enum TerrainType {Forest, Desert}

    [SerializeField] private MapType mapType;
    [SerializeField] private TerrainType terrainType;

    [Header("Map Dimensions")]
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float scale;
    [SerializeField] private int seed;

    [Header("Forest Noise Additions")]
    [Range(0, 8)]
    [SerializeField] private int forestOctaves;
    [Range(0, 1)]
    [SerializeField] private float forestPersistence;
    [Range(2, 10)]
    [SerializeField] private float forestLacunarity;

    [SerializeField] private float forestHeightMultiplier;
    
    [Header("Desert Noise Additions")]
    [Range(0, 8)]
    [SerializeField] private int desertOctaves;
    [Range(0, 1)]
    [SerializeField] private float desertPersistence;
    [Range(2, 10)]
    [SerializeField] private float desertLacunarity;
    
    [SerializeField] private float desertHeightMultiplier;

    
    [SerializeField] private AnimationCurve heightCurve;
    [SerializeField] private AnimationCurve falloffCurve;
    
    [Header("Terrain Data")]
    [SerializeField] private SCR_TerrainData[] forestTerrainData;

    [SerializeField] private SCR_TerrainData[] desertTerrainData;

    [SerializeField] private SCR_MapDisplay mapDisplay;
    [SerializeField] private SCR_MeshGenerator meshGen;

    private float[,] heightMap;
    private float[,] falloffMap;
    private int propDensity;

    /// <summary>
    /// Was only used for testing, has no functionality in the final version
    /// </summary>
    private void Start()
    {
        switch (mapType)
        {
            case MapType.Noise:
                GenerateNoiseMap();
                break;
            case MapType.Colour:
                GenerateColourMap();
                break;
            /*case MapType.Mesh:
                GenerateMesh();
                break;*/
            default:
                Debug.LogWarning("No compatible cases defined");
                break;
        }
    }

    /// <summary>
    /// Generates and returns a Noise Map
    /// Draws out the Noise Map from the values created
    /// </summary>
    private void GenerateNoiseMap()
    {
        heightMap = SCR_Noise.GenerateNoise(width, height, seed, scale, forestOctaves, forestPersistence, forestLacunarity);
        mapDisplay.DrawNoiseMap(heightMap);
    }

    /// <summary>
    /// Generates and draws the colour map of the region depending on the map type
    /// </summary>
    private void GenerateColourMap()
    {
        falloffMap = SCR_FalloffMap.GenerateFalloffMap(width, height, falloffCurve);
        switch (terrainType)
        {
            case TerrainType.Forest:
                heightMap = SCR_Noise.GenerateNoise(width, height, seed, scale, forestOctaves, forestPersistence, forestLacunarity);
                mapDisplay.DrawColourMap(heightMap, falloffMap, forestTerrainData, false);
                break;
            case TerrainType.Desert:
                heightMap = SCR_Noise.GenerateNoise(width, height, seed, scale, desertOctaves, desertPersistence, desertLacunarity);
                mapDisplay.DrawColourMap(heightMap, falloffMap, desertTerrainData, false);
                break;
            default:
                Debug.Log("No compatible cases defined");
                break;
        }
    }

    //Only used for testing (Creates the Generate Mesh button in the inspector)
    public void GenerateMeshEditor()
    {
        GenerateMesh();
    }
    /// <summary>
    /// Generates the terrain mesh depending on the map type
    /// </summary>
    private void GenerateMesh()
    {
        falloffMap = SCR_FalloffMap.GenerateFalloffMap(width, height, falloffCurve);
        switch (terrainType)
        {
            case TerrainType.Forest:
                heightMap = SCR_Noise.GenerateNoise(width, height, seed, scale, forestOctaves, forestPersistence, forestLacunarity);
                meshGen.GenerateMesh(heightMap, falloffMap, forestHeightMultiplier, heightCurve, forestTerrainData, terrainType, propDensity);
                break;
            case TerrainType.Desert:
                heightMap = SCR_Noise.GenerateNoise(width, height, seed, scale, desertOctaves, desertPersistence, desertLacunarity);
                meshGen.GenerateMesh(heightMap, falloffMap, desertHeightMultiplier, heightCurve, desertTerrainData, terrainType, propDensity);
                break;
            default:
                Debug.LogWarning("No compatible cases defined");
                break;
        }
    }

    /// <summary>
    /// Sets a few mesh World parameters from other classes
    /// </summary>
    /// <param name="_seed">Sets the seed from the input of the player</param>
    /// <param name="_regionIndex">Sets what region to use from the dropdown UI</param>
    /// <param name="_propDensity">Sets the prop density from the density slider UI</param>
    public void SetWorldParams(int _seed, int _regionIndex, int _propDensity)
    {
        seed = _seed;
        propDensity = _propDensity;
        switch (_regionIndex)
        {
            case 0:
                terrainType = TerrainType.Forest;
                break;
            case 1:
                terrainType = TerrainType.Desert;
                break;
            default:
                Debug.Log("No compatible cases defined");
                break;
        }
        
        GenerateMesh();
    }
}