using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SCR_MeshGenerator : MonoBehaviour
{
    [SerializeField] private GameObject meshObj;
    [SerializeField] private Material forestMat;
    [SerializeField] private Material desertMat;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Vector3 playerSpawnOffset;
    private GameObject playerSpawned;

    [SerializeField] private GameObject terrainMesh;
    
    private Mesh mesh;

    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;

    private int vertIndex = 0;
    private int trisIndex = 0;

    private float perlinHeight = 0;

    [Header("Prop Placement Data")]
    private int _propDensity;
    [SerializeField] private int grassDensity;

    private List<GameObject> allPropsSpawned;
    private List<float> haltonSequencePoints;

    private RaycastHit hit;
    private List<int> propFactors;

    [SerializeField] private GameObject grassPrefab;
    [SerializeField] private GameObject deadGrassPrefab;

    [Header("Prop Manipulation")] 
    [SerializeField] private float scaleVariation;

    [SerializeField] private GameObject generationUI;
    [SerializeField] private Camera uiCam;

    /// <summary>
    /// Generates a terrain mesh
    /// Creates all vertices needed for the tris & manipulates the vertice heights (Y axis)
    /// </summary>
    /// <param name="noiseMap">Perlin values to manipulate vertice heights</param>
    /// <param name="falloffMap">Combines with the initial Noise Map to filter the terrain</param>
    /// <param name="noiseMultiplier">Multiplier for the perlin values to affect the vertice heights</param>
    /// <param name="heightCurve">Evaluates the heights and moves vertices up depending on the height value evaluated</param>
    /// <param name="terrainTypes">Allows access to the terrain height params</param>
    /// <param name="terrainBiome">Decides what material is applied to the terrain mesh </param>
    /// <param name="propDensity">How many times the method loops over prop instantiation</param>
    public void GenerateMesh(float[,] noiseMap, float[,] falloffMap, float noiseMultiplier, AnimationCurve heightCurve, SCR_TerrainData[] terrainTypes, SCR_MapGenerator.TerrainType terrainBiome, int propDensity)
    {
        int meshWidth = noiseMap.GetLength(0);
        int meshHeight = noiseMap.GetLength(1);
        float meshScale = meshWidth;
        meshObj.transform.localScale = new Vector3(meshScale, meshScale, meshScale);

        vertices = new Vector3[(meshWidth + 1) * (meshHeight + 1)];
        uvs = new Vector2[vertices.Length];
        
        haltonSequencePoints = SCR_HaltonSequence.GenerateSequence(noiseMap);
        allPropsSpawned = new List<GameObject>();
        _propDensity = propDensity;

        for (int z = 0, i = 0; z <= meshHeight; z++)
        {
            for (int x = 0; x <= meshWidth; x++)
            {
                if (x < meshWidth && z < meshHeight)
                {
                    perlinHeight = Mathf.Clamp01(heightCurve.Evaluate(noiseMap[x, z]) - falloffMap[x, z]);
                    perlinHeight *= noiseMultiplier;
                }

                if (x >= meshWidth && z >= meshHeight)
                {
                    perlinHeight = Mathf.Clamp01(heightCurve.Evaluate(noiseMap[x - 1, z - 1]) - falloffMap[x - 1, z - 1]);
                    perlinHeight *= noiseMultiplier;
                }

                vertices[i] = new Vector3(x, perlinHeight, z);
                uvs[i] = new Vector2(x / (float) meshWidth, z / (float) meshHeight);
                i++;
            }
        }

        AddTriangles(meshWidth, meshHeight);
        UpdateMesh();
        GenerateProps(meshScale, terrainTypes, terrainBiome);
    }

    /// <summary>
    /// Adds all the triangles together for the terrain mesh
    /// </summary>
    /// <param name="width">Width of the terrain</param>
    /// <param name="height">Height of the terrain</param>
    private void AddTriangles(int width, int height)
    {
        triangles = new int[width * height * 6];
        trisIndex = 0;
        vertIndex = 0;

        //Adapted from Brackeys, 2018
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                triangles[trisIndex + 0] = vertIndex + 0;
                triangles[trisIndex + 1] = vertIndex + width + 1;
                triangles[trisIndex + 2] = vertIndex + 1;
                triangles[trisIndex + 3] = vertIndex + 1;
                triangles[trisIndex + 4] = vertIndex + width + 1;
                triangles[trisIndex + 5] = vertIndex + width + 2;

                vertIndex++;
                trisIndex += 6;
            }
            vertIndex++;
            //End of adapted code
        }
    }

    /// <summary>
    /// Updates the mesh with all vertices, triangles, colliders and UVs
    /// </summary>
    private void UpdateMesh()
    {
        mesh = new Mesh();
        meshObj.GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        
        mesh.RecalculateBounds();
        meshObj.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    /// <summary>
    /// Generates all props across the mesh terrain based on the land type and iteration amount
    /// Sets the mesh terrain material
    /// </summary>
    /// <param name="meshScale">Scale of the terrain</param>
    /// <param name="terrainTypes">Different terrain types for the height of each</param>
    /// <param name="terrainType">Different terrain regions for different materials</param>
    private void GenerateProps(float meshScale, SCR_TerrainData[] terrainTypes, SCR_MapGenerator.TerrainType terrainType)
    {
        switch (terrainType)
        {
            case SCR_MapGenerator.TerrainType.Forest:
                meshObj.GetComponent<MeshRenderer>().material = forestMat;
                break;
            case SCR_MapGenerator.TerrainType.Desert:
                meshObj.GetComponent<MeshRenderer>().material = desertMat;
                break;
        }

        for (int i = 0; i < vertices.Length - 1; i++)
        {
            if (vertices[i].y <= 0)
            {
                continue;
            }
            
            if (Physics.Raycast(vertices[i] * meshScale, Vector3.down, out hit, Mathf.Infinity))
            {
                for (int j = 0; j < terrainTypes.Length; j++)
                {
                    if (vertices[i].y <= terrainTypes[j].terrainHeight)
                    {
                        int haltonSample = Random.Range(0, haltonSequencePoints.Count);

                        int propChoice = Random.Range(0, terrainTypes[j].propList.Length);
                        GameObject tempProp = Instantiate(terrainTypes[j].propList[propChoice], hit.point + new Vector3(haltonSequencePoints[haltonSample], 0f, haltonSequencePoints[haltonSample]), Quaternion.Euler(hit.normal));
                        Vector3 propScale = tempProp.transform.localScale;
                        float scaleTemp = Random.Range(propScale.x - scaleVariation, propScale.x + scaleVariation);
                        
                        if (tempProp.transform.CompareTag("NormalScale"))
                        {
                            scaleTemp = Random.Range(0.5f, 4f);
                            tempProp.transform.localScale = new Vector3(scaleTemp, scaleTemp, scaleTemp);;
                        }
                        else
                        {
                            tempProp.transform.localScale = new Vector3(scaleTemp, scaleTemp, scaleTemp);
                        }
                        
                        float rotationAmount = Random.Range(0f, 360f);
                        tempProp.transform.Rotate(Vector3.up * rotationAmount);
                        allPropsSpawned.Add(tempProp);
                        tempProp.transform.parent = terrainMesh.transform;
                        break;
                    }
                }
            }

            propFactors = SCR_FactorsFinder.FindAllFactorsOfANum((int)meshScale);

            if (_propDensity >= propFactors.Count)
            {
                _propDensity = propFactors.Count - 1;
            }

            i += propFactors[_propDensity];
        }
        GenerateGrass(meshScale, terrainTypes, terrainType);
    }

    /// <summary>
    /// Generates grass foliage separately
    /// </summary>
    /// <param name="meshScale">Scale of the mesh terrain</param>
    /// <param name="terrainData">Terrain data for the heights</param>
    /// <param name="terrainType">Different terrain regions for the type of grass to spawn</param>
    private void GenerateGrass(float meshScale, SCR_TerrainData[] terrainData, SCR_MapGenerator.TerrainType terrainType)
    {
        for (int i = 0; i < vertices.Length - 1; i++)
        {
            if (vertices[i].y <= 0)
            {
                continue;
            }

            if (Physics.Raycast(vertices[i] * meshScale, Vector3.down, out hit, Mathf.Infinity))
            {
                for (int j = 0; j < terrainData.Length; j++)
                {
                    if (vertices[i].y <= terrainData[j].terrainHeight)
                    {
                        int haltonSample = Random.Range(0, haltonSequencePoints.Count);
                        switch (terrainType)
                        {
                            case SCR_MapGenerator.TerrainType.Forest:
                                GameObject tempGrassProp = Instantiate(grassPrefab,
                                    hit.point + new Vector3(haltonSequencePoints[haltonSample], 0,
                                        haltonSequencePoints[haltonSample]), Quaternion.Euler(hit.normal));
                                Vector3 propScale = tempGrassProp.transform.localScale;
                                float scaleTemp = Random.Range(propScale.x - scaleVariation,
                                    propScale.x + scaleVariation);
                                tempGrassProp.transform.localScale = new Vector3(scaleTemp, scaleTemp, scaleTemp);

                                float rotationAmount = Random.Range(0f, 360f);
                                tempGrassProp.transform.Rotate(Vector3.up * rotationAmount);
                                allPropsSpawned.Add(tempGrassProp);
                                tempGrassProp.transform.parent = terrainMesh.transform;
                                break;
                            case SCR_MapGenerator.TerrainType.Desert:
                                GameObject tempDeadProp = Instantiate(deadGrassPrefab,
                                    hit.point + new Vector3(haltonSequencePoints[haltonSample], 0,
                                        haltonSequencePoints[haltonSample]), Quaternion.Euler(hit.normal));
                                Vector3 propDeadScale = tempDeadProp.transform.localScale;
                                float scaleDeadTemp = Random.Range(propDeadScale.x - scaleVariation,
                                    propDeadScale.x + scaleVariation);
                                tempDeadProp.transform.localScale =
                                    new Vector3(scaleDeadTemp, scaleDeadTemp, scaleDeadTemp);

                                float rotationDeadAmount = Random.Range(0f, 360f);
                                tempDeadProp.transform.Rotate(Vector3.up * rotationDeadAmount);
                                allPropsSpawned.Add(tempDeadProp);
                                tempDeadProp.transform.parent = terrainMesh.transform;
                                break;
                        }

                        break;
                    }
                }
            }

            if (grassDensity >= propFactors.Count)
            {
                grassDensity = propFactors.Count - 1;
            }

            i += propFactors[grassDensity];
        }

        //Spawns the player on an area where there are no props
        for (int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i].y <= 0 || vertices[i].y > 0.4)
            {
                continue;
            }

            if (Physics.Raycast(vertices[i] * meshScale, Vector3.down, out hit, Mathf.Infinity))
            {
                for (int j = 0; j < terrainData.Length; j++)
                {
                    if (vertices[i].y <= terrainData[j].terrainHeight)
                    {
                        if (hit.transform.tag != "Prop" && hit.transform.tag != "NormalScale")
                        {
                            GameObject tempPlayer = Instantiate(playerPrefab, hit.point + playerSpawnOffset,
                                Quaternion.identity);
                            playerSpawned = tempPlayer;
                            return;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Clears the terrain of all props and the player
    /// Unlocks the cursor
    /// </summary>
    public void ClearTerrain()
    {
        if (allPropsSpawned.Count > 0)
        {
            for (int i = 0; i < allPropsSpawned.Count; i++)
            {
                DestroyImmediate(allPropsSpawned[i]);
            }
        }
        
        Destroy(playerSpawned);
        SCR_CustomEvents.Player.OnSetPlayerSpawnState?.Invoke(false);
        meshObj.GetComponent<MeshFilter>().mesh = null;
        generationUI.SetActive(true);
        uiCam.enabled = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private GameObject GetPlayerReference()
    {
        return playerSpawned;
    }

    private void OnEnable()
    {
        SCR_CustomEvents.Player.OnGetPlayerReference += GetPlayerReference;
    }

    private void OnDisable()
    {
        SCR_CustomEvents.Player.OnGetPlayerReference -= GetPlayerReference;
    }
}
