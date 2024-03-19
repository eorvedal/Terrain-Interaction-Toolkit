using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using booger;
using UnityEngine.SceneManagement;

public class TerrainDetailConverter : MonoBehaviour
{
    public int[] detailLayersToReplace;
    public float threshold = 0.5f;
    public List<int> texturesToExclude = new List<int>(); // Public list to hold texture indices to be excluded
    private Terrain terrain;
    private TerrainData terrainData;
    private Transform detailContainer;
    private Transform treeContainer;
    private List<int> ReplacedLayers = new List<int>();
    public float textureThreshold = 0.5f;
    public GameObject DetailPlaceholder = null;
    private int[,,] detailBackup;
    private List<int> ReplacedTrees = new List<int>();

    private void Start()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        terrain = GetComponent<Terrain>();
        if (terrain != null)
        {
            BackupDetailMap();
        }
        else
        {
            Debug.LogError("Terrain component not found!");
        }
    }

    private void BackupDetailMap()
    {
        TerrainData terrainData = terrain.terrainData;
        int detailWidth = terrainData.detailWidth;
        int detailHeight = terrainData.detailHeight;
        int numLayers = terrainData.detailPrototypes.Length;

        detailBackup = new int[detailWidth, detailHeight, numLayers];
        for (int x = 0; x < detailWidth; x++)
        {
            for (int y = 0; y < detailHeight; y++)
            {
                for (int layer = 0; layer < numLayers; layer++)
                {
                    detailBackup[x, y, layer] = terrainData.GetDetailLayer(x, y, 1, 1, layer)[0, 0];
                }
            }
        }
    }

    private void RestoreDetailMap()
    {
        TerrainData terrainData = terrain.terrainData;
        int detailWidth = terrainData.detailWidth;
        int detailHeight = terrainData.detailHeight;
        int numLayers = terrainData.detailPrototypes.Length;

        for (int x = 0; x < detailWidth; x++)
        {
            for (int y = 0; y < detailHeight; y++)
            {
                for (int layer = 0; layer < numLayers; layer++)
                {
                    int[,] detailLayer = new int[1, 1];
                    detailLayer[0, 0] = detailBackup[x, y, layer];
                    terrainData.SetDetailLayer(x, y, layer, detailLayer);
                }
            }
        }
    }

    private void OnSceneUnloaded(Scene current)
    {
        RestoreDetailMap();
    }

    private void OnApplicationQuit()
    {
        RestoreDetailMap();
    }

    [ContextMenu("Single terrain convert details to GameObjects Batched")]
    public async Task DoTheNewConversionBatched()
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;
        if (detailContainer == null)
        {
            detailContainer = new GameObject("DetailContainer").transform;
            detailContainer.parent = transform;
        }
        await NewConvertDetailsToGameObjectsBatched();
    }
    [ContextMenu("Single terrain convert details to GameObjects")]
    public async Task DoTheNewConversion()
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;
        if (detailContainer == null)
        {
            detailContainer = new GameObject("DetailContainer").transform;
            detailContainer.parent = transform;
        }
        NewConvertDetailsToGameObjects();
    }
    public async Task DoTheNewConversionTrees()
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;
        if (treeContainer == null)
        {
            treeContainer = new GameObject("TreeContainer").transform;
            treeContainer.parent = transform;
        }
        NewConvertTreesToGameObjects();
    }

    public async Task DoThePlaceholderConversion(int[] newDetailLayersToReplace, float newThreshold, GameObject placeHolder, int GlobalRange)
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;
        if (detailContainer == null)
        {
            detailContainer = new GameObject("DetailContainer").transform;
            detailContainer.parent = transform;
        }
        ConvertDetailsToPlaceholders(newDetailLayersToReplace, newThreshold, placeHolder, GlobalRange);
    }

    public async Task DoTheNewConversion(int[] newDetailLayersToReplace, float newThreshold)
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;
        if (detailContainer == null)
        {
            detailContainer = new GameObject("DetailContainer").transform;
            detailContainer.parent = transform;
        }
        NewConvertDetailsToGameObjects(newDetailLayersToReplace, newThreshold);
    }
    public async Task DoTheNewConversionBatched(int[] newDetailLayersToReplace, float newThreshold)
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;
        if (detailContainer == null)
        {
            detailContainer = new GameObject("DetailContainer").transform;
            detailContainer.parent = transform;
        }
        NewConvertDetailsToGameObjectsBatched(newDetailLayersToReplace, newThreshold);
    }

    public async Task RemoveSpawnedObjects()
    {
        DestroyImmediate(detailContainer.gameObject);
    }

    public async Task RemoveSpawnedTrees()
    {
        DestroyImmediate(treeContainer.gameObject);
    }

    public async Task RemoveOriginalDetails()
    {
        int width = terrainData.detailWidth;
        int height = terrainData.detailHeight;

        foreach (int layerIndex in ReplacedLayers)
        {
            int[,] originalLayer = terrainData.GetDetailLayer(0, 0, width, height, layerIndex);
            int[,] newDetailLayer = new int[width, height]; // Create a new detail layer array

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (originalLayer[x, y] > threshold)
                    {
                        // Set density of the original layer to 0
                        newDetailLayer[x, y] = 0;
                    }
                }
            }

            // Update the detail layer
            terrainData.SetDetailLayer(0, 0, layerIndex, newDetailLayer);
        }
    }

    public async Task RemoveOriginalDetails(int[] AdditionalLayers)
    {
        int width = terrainData.detailWidth;
        int height = terrainData.detailHeight;

        foreach (int layerIndex in ReplacedLayers)
        {
            int[,] originalLayer = terrainData.GetDetailLayer(0, 0, width, height, layerIndex);
            int[,] newDetailLayer = new int[width, height]; // Create a new detail layer array

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (originalLayer[x, y] > threshold)
                    {
                        // Set density of the original layer to 0
                        newDetailLayer[x, y] = 0;
                    }
                }
            }

            // Update the detail layer
            terrainData.SetDetailLayer(0, 0, layerIndex, newDetailLayer);
        }
        foreach (int layerIndex in AdditionalLayers)
        {
            int[,] originalLayer = terrainData.GetDetailLayer(0, 0, width, height, layerIndex);
            int[,] newDetailLayer = new int[width, height]; // Create a new detail layer array

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (originalLayer[x, y] > threshold)
                    {
                        // Set density of the original layer to 0
                        newDetailLayer[x, y] = 0;
                    }
                }
            }

            // Update the detail layer
            terrainData.SetDetailLayer(0, 0, layerIndex, newDetailLayer);
        }

    }
    public async Task RemoveAdditionalDetails(int[] AdditionalLayers)
    {
        int width = terrainData.detailWidth;
        int height = terrainData.detailHeight;

        foreach (int layerIndex in AdditionalLayers)
        {
            int[,] originalLayer = terrainData.GetDetailLayer(0, 0, width, height, layerIndex);
            int[,] newDetailLayer = new int[width, height]; // Create a new detail layer array

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (originalLayer[x, y] > threshold)
                    {
                        // Set density of the original layer to 0
                        newDetailLayer[x, y] = 0;
                    }
                }
            }

            // Update the detail layer
            terrainData.SetDetailLayer(0, 0, layerIndex, newDetailLayer);
        }

    }

    public async Task Reset_Data()
    {
        ReplacedLayers.Clear();
        ReplacedTrees.Clear();
        detailContainer = null;
        treeContainer = null;
    }

    private void NewConvertDetailsToGameObjects()
    {
        int width = terrainData.detailWidth;
        int height = terrainData.detailHeight;
        DetailPrototype[] detailPrototypes = terrainData.detailPrototypes;
        int detailPatchCount = terrainData.detailPatchCount;
        // Iterate over all patches
        for (int patchY = 0; patchY < detailPatchCount; patchY++)
        {
            for (int patchX = 0; patchX < detailPatchCount; patchX++)
            {
                foreach (int layerIndex in detailLayersToReplace)
                {
                    // Compute detail instance transforms for the current patch and layer
                    Bounds bounds;
                    DetailInstanceTransform[] transforms = terrainData.ComputeDetailInstanceTransforms(patchX, patchY, layerIndex, threshold, out bounds);

                    // Iterate over computed transforms and instantiate objects
                    foreach (DetailInstanceTransform transform_ in transforms)
                    {
                        Vector3 position = new Vector3(terrain.transform.position.x + transform_.posX, terrain.transform.position.y + transform_.posY, terrain.transform.position.z + transform_.posZ); // Position of the detail object
                        Quaternion rotation = Quaternion.Euler(0, transform_.rotationY * Mathf.Rad2Deg, 0); // Rotation of the detail object
                        Vector3 scale = new Vector3(transform_.scaleXZ, transform_.scaleY, transform_.scaleXZ); // Assuming the scale is uniform

                        // Instantiate new object with random rotation and scale
                        GameObject newObject = Instantiate(detailPrototypes[layerIndex].prototype, position, rotation);
                        newObject.transform.localScale = scale; // Apply the calculated scale
                        newObject.transform.parent = detailContainer;
                    }

                    // Track replaced layers
                    ReplacedLayers.Add(layerIndex);
                }
            }
        }
    }

    private void NewConvertDetailsToGameObjects(int[] detailLayersToReplace, float threshold)
    {
        int width = terrainData.detailWidth;
        int height = terrainData.detailHeight;
        DetailPrototype[] detailPrototypes = terrainData.detailPrototypes;
        int detailPatchCount = terrainData.detailPatchCount;

        // Iterate over all patches
        for (int patchY = 0; patchY < detailPatchCount; patchY++)
        {
            for (int patchX = 0; patchX < detailPatchCount; patchX++)
            {
                foreach (int layerIndex in detailLayersToReplace)
                {
                    // Compute detail instance transforms for the current patch and layer
                    Bounds bounds;
                    DetailInstanceTransform[] transforms = terrainData.ComputeDetailInstanceTransforms(patchX, patchY, layerIndex, threshold, out bounds);

                    // Iterate over computed transforms and instantiate objects
                    foreach (DetailInstanceTransform transform_ in transforms)
                    {
                        Vector3 position = new Vector3(terrain.transform.position.x + transform_.posX, terrain.transform.position.y + transform_.posY, terrain.transform.position.z + transform_.posZ); // Position of the detail object
                        Quaternion rotation = Quaternion.Euler(0, transform_.rotationY * Mathf.Rad2Deg, 0); // Rotation of the detail object
                        Vector3 scale = new Vector3(transform_.scaleXZ, transform_.scaleY, transform_.scaleXZ); // Assuming the scale is uniform

                        // Instantiate new object with random rotation and scale
                        GameObject newObject = Instantiate(detailPrototypes[layerIndex].prototype, position, rotation);
                        newObject.transform.localScale = scale; // Apply the calculated scale
                        newObject.transform.parent = detailContainer;
                    }

                    // Track replaced layers
                    ReplacedLayers.Add(layerIndex);
                }
            }
        }
    }
    private void ConvertDetailsToPlaceholders(int[] detailLayersToReplace, float threshold, GameObject DetailPlaceholder, int GlobalRange)
    {
        int width = terrainData.detailWidth;
        int height = terrainData.detailHeight;
        DetailPrototype[] detailPrototypes = terrainData.detailPrototypes;
        int detailPatchCount = terrainData.detailPatchCount;

        // Iterate over all patches
        for (int patchY = 0; patchY < detailPatchCount; patchY++)
        {
            for (int patchX = 0; patchX < detailPatchCount; patchX++)
            {
                foreach (int layerIndex in detailLayersToReplace)
                {
                    // Compute detail instance transforms for the current patch and layer
                    Bounds bounds;
                    DetailInstanceTransform[] transforms = terrainData.ComputeDetailInstanceTransforms(patchX, patchY, layerIndex, threshold, out bounds);

                    // Iterate over computed transforms and instantiate objects
                    foreach (DetailInstanceTransform transform_ in transforms)
                    {
                        Vector3 position = new Vector3(terrain.transform.position.x + transform_.posX, terrain.transform.position.y + transform_.posY, terrain.transform.position.z + transform_.posZ); // Position of the detail object
                        Quaternion rotation = Quaternion.Euler(0, transform_.rotationY * Mathf.Rad2Deg, 0); // Rotation of the detail object
                        Vector3 scale = new Vector3(transform_.scaleXZ, transform_.scaleY, transform_.scaleXZ); // Assuming the scale is uniform
                        // Let's try the actual rotation first...
                        GameObject newObject = Instantiate(DetailPlaceholder, position, rotation);
                        // Instantiate new object with random rotation and scale
                        //GameObject newObject = Instantiate(detailPrototypes[layerIndex].prototype, position, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
                        LayerTag layerTag = newObject.GetComponent<LayerTag>();
                        if (layerTag == null)
                            layerTag = newObject.AddComponent<LayerTag>();
                        layerTag.DetailLayer = layerIndex;
                        layerTag.DetailTerrain = terrain;
                        if (GlobalRange > 0)
                            layerTag.range = GlobalRange;
                        newObject.transform.localScale = scale; // Apply the calculated scale
                        newObject.transform.parent = detailContainer;
                    }
                }
            }
        }
    }

    private async Task NewConvertDetailsToGameObjectsBatched()
    {
        int width = terrainData.detailWidth;
        int height = terrainData.detailHeight;
        DetailPrototype[] detailPrototypes = terrainData.detailPrototypes;
        int detailPatchCount = terrain.terrainData.detailPatchCount;

        int batchSizeX = 10; // Define the size of each processing batch
        int batchSizeY = 10;

        for (int patchY = 0; patchY < detailPatchCount; patchY += batchSizeY)
        {
            for (int patchX = 0; patchX < detailPatchCount; patchX += batchSizeX)
            {
                foreach (int layerIndex in detailLayersToReplace)
                {
                    // Compute detail instance transforms for the current patch and layer
                    Bounds bounds;
                    DetailInstanceTransform[] transforms = terrainData.ComputeDetailInstanceTransforms(patchX, patchY, layerIndex, threshold, out bounds);

                    // Iterate over computed transforms and instantiate objects
                    foreach (DetailInstanceTransform transform_ in transforms)
                    {
                        Vector3 position = new Vector3(terrain.transform.position.x + transform_.posX, terrain.transform.position.y + transform_.posY, terrain.transform.position.z + transform_.posZ); // Position of the detail object
                        Quaternion rotation = Quaternion.Euler(0, transform_.rotationY * Mathf.Rad2Deg, 0); // Rotation of the detail object
                        Vector3 scale = new Vector3(transform_.scaleXZ, transform_.scaleY, transform_.scaleXZ); // Assuming the scale is uniform

                        // Instantiate new object with random rotation and scale
                        GameObject newObject = Instantiate(detailPrototypes[layerIndex].prototype, position, rotation);
                        newObject.transform.localScale = scale; // Apply the calculated scale
                        newObject.transform.parent = detailContainer;
                    }

                    // Track replaced layers
                    ReplacedLayers.Add(layerIndex);
                }
                await Task.Yield(); // Yield execution to the next frame
            }
        }
    }
    private async Task NewConvertDetailsToGameObjectsBatched(int[] detailLayersToReplace, float threshold)
    {
        int width = terrainData.detailWidth;
        int height = terrainData.detailHeight;
        DetailPrototype[] detailPrototypes = terrainData.detailPrototypes;
        int detailPatchCount = terrain.terrainData.detailPatchCount;

        int batchSizeX = 10; // Define the size of each processing batch
        int batchSizeY = 10;

        for (int patchY = 0; patchY < detailPatchCount; patchY += batchSizeY)
        {
            for (int patchX = 0; patchX < detailPatchCount; patchX += batchSizeX)
            {
                foreach (int layerIndex in detailLayersToReplace)
                {
                    // Compute detail instance transforms for the current patch and layer
                    Bounds bounds;
                    DetailInstanceTransform[] transforms = terrainData.ComputeDetailInstanceTransforms(patchX, patchY, layerIndex, threshold, out bounds);

                    // Iterate over computed transforms and instantiate objects
                    foreach (DetailInstanceTransform transform_ in transforms)
                    {
                        Vector3 position = new Vector3(terrain.transform.position.x + transform_.posX, terrain.transform.position.y + transform_.posY, terrain.transform.position.z + transform_.posZ); // Position of the detail object
                        Quaternion rotation = Quaternion.Euler(0, transform_.rotationY * Mathf.Rad2Deg, 0); // Rotation of the detail object
                        Vector3 scale = new Vector3(transform_.scaleXZ, transform_.scaleY, transform_.scaleXZ); // Assuming the scale is uniform

                        // Instantiate new object with random rotation and scale
                        GameObject newObject = Instantiate(detailPrototypes[layerIndex].prototype, position, rotation);
                        newObject.transform.localScale = scale; // Apply the calculated scale
                        newObject.transform.parent = detailContainer;
                    }

                    // Track replaced layers
                    ReplacedLayers.Add(layerIndex);
                }
                await Task.Yield(); // Yield execution to the next frame
            }
        }
    }

    private Vector3 GetSpawnPosition(int x, int y, int width, int height)
    {
        // Calculate the world position based on the terrain dimensions
        Vector3 position = new Vector3(
            terrain.transform.position.x + x * (terrainData.size.x / width),
            terrainData.GetHeight(x, y),
            terrain.transform.position.z + y * (terrainData.size.z / height)
        );

        return position;
    }

    async Task NewConvertTreesToGameObjects()
    {
        int width = terrainData.heightmapResolution; // Adjust as needed for tree conversion
        int height = terrainData.heightmapResolution; // Adjust as needed for tree conversion
        TreePrototype[] treePrototypes = terrainData.treePrototypes;

        // Iterate over all trees
        foreach (TreeInstance treeInstance in terrainData.treeInstances)
        {
            int prototypeIndex = treeInstance.prototypeIndex;

            // Get tree position in world space
            Vector3 position = Vector3.Scale(treeInstance.position, terrainData.size) + terrain.transform.position;

            // Get tree rotation
            Quaternion rotation = Quaternion.Euler(0f, treeInstance.rotation * Mathf.Rad2Deg, 0f);
            // Get tree scale
            Vector3 scale = new Vector3(treeInstance.widthScale, treeInstance.heightScale, treeInstance.widthScale);

            // Instantiate new tree object
            GameObject newObject = Instantiate(treePrototypes[prototypeIndex].prefab, position, rotation);
            newObject.transform.localScale = scale; // Apply the calculated scale
            newObject.transform.parent = treeContainer;

            // Track replaced layers (if needed)
            ReplacedTrees.Add(prototypeIndex);
        }
    }
    public async Task RemoveOriginalTrees()
    {
        TreeInstance[] treeInstances = terrainData.treeInstances;

        foreach (TreeInstance treeInstance in treeInstances)
        {
            // Check if this tree instance was replaced
            if (ReplacedTrees.Contains(treeInstance.prototypeIndex))
            {
                // Remove the tree instance
                RemoveTreeInstance(treeInstance);
            }
        }

        // Refresh the terrain to apply changes
        terrain.Flush();
    }

    private void RemoveTreeInstance(TreeInstance treeInstance)
    {
        List<TreeInstance> updatedTreeInstances = new List<TreeInstance>(terrainData.treeInstances);
        updatedTreeInstances.Remove(treeInstance);
        terrainData.treeInstances = updatedTreeInstances.ToArray();
    }
}


[System.Serializable]
public class SerializableTerrainData
{
    public int detailResolution;
    public int detailResolutionPerPatch;
    public int detailWidth;
    public int detailHeight;
    public SerializableDetailLayer[] detailLayers;

    public SerializableTerrainData(TerrainData terrainData)
    {
        detailResolution = terrainData.detailResolution;
        detailResolutionPerPatch = terrainData.detailResolutionPerPatch;
        detailWidth = terrainData.detailWidth;
        detailHeight = terrainData.detailHeight;

        detailLayers = new SerializableDetailLayer[terrainData.detailPrototypes.Length];
        for (int i = 0; i < terrainData.detailPrototypes.Length; i++)
        {
            detailLayers[i] = new SerializableDetailLayer(terrainData.GetDetailLayer(0, 0, detailWidth, detailHeight, i));
        }
    }

    public TerrainData ToTerrainData()
    {
        TerrainData terrainData = new TerrainData();
        terrainData.SetDetailResolution(detailResolution, detailResolutionPerPatch);

        foreach (var detailLayer in detailLayers)
        {
            terrainData.SetDetailLayer(0, 0, detailLayer.layer, detailLayer.detailLayer);
        }

        return terrainData;
    }
}

[System.Serializable]
public class SerializableDetailLayer
{
    public int layer;
    public int[,] detailLayer;

    public SerializableDetailLayer(int[,] detailLayer)
    {
        this.detailLayer = detailLayer;
    }
}
