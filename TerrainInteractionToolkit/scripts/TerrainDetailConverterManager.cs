using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class DetailLayerGroup
{
    public int[] DetailGroupLayers;
    public float GroupThreshold;
    public int GlobalRange;
    public GameObject GroupPlaceholder;
}

public class TerrainDetailConverterManager : MonoBehaviour
{
    [Header("Runtime Generation - uses prefab placeholders")]
    public bool RuntimeGeneration = false;
    public DetailLayerGroup[] layerGroups;
    public bool UseGridBasedCulling = false;
    public float cullingDistance = 50.0f; // Set loadUnloadDistance to 1 for a 3x3 grid
    public float cullingInterval = 3.0f;
    private List<GameObject> PlaceholderGroups = new List<GameObject>();
    //public float loadUnloadDistance = 10f; // Adjust as needed
    [SpaceArea]
    [Header("In-Editor Methods - uses TerrainData prototypes")]
    public int[] newDetailLayersToReplace;
    public int[] additionalLayersToRemove;
    public float newThreshold = 1.0f;
    public List<int> texturesToExclude = new List<int>(); // Public list to hold texture indices to be excluded
    public GameObject placeholder;
    public int GlobalRange = 1;
    private Transform playerTransform;
    private Coroutine cullingCoroutine;

    private void Start()
    {
        if (RuntimeGeneration)
        {
            SetupPlaceholdersNoProgress();
            if (UseGridBasedCulling)
            {
                playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
                cullingCoroutine = StartCoroutine(CullPlaceholdersPeriodically());
            }
        }
    }
    [ContextMenu("Objects/Convert Terrain Details to GameObjects (New Method)")]
    public async void NewConvertTerrainDetails()
    {
        TerrainDetailConverter[] converters = FindObjectsOfType<TerrainDetailConverter>();
        float progress = 0f;
        float increment = 1f / converters.Length;

        foreach (TerrainDetailConverter converter in converters)
        {
            await converter.DoTheNewConversion();
            progress += increment;
            EditorUtility.DisplayProgressBar("Converting Terrain Details", "Progress...", progress);
        }

        EditorUtility.ClearProgressBar();
    }

    [ContextMenu("Objects/Convert Customized Terrain Details to GameObjects (New Method)")]
    public async void NewConvertCustomTerrainDetails()
    {
        TerrainDetailConverter[] converters = FindObjectsOfType<TerrainDetailConverter>();
        float progress = 0f;
        float increment = 1f / converters.Length;

        foreach (TerrainDetailConverter converter in converters)
        {
            await converter.DoTheNewConversion(newDetailLayersToReplace, newThreshold);
            progress += increment;
            EditorUtility.DisplayProgressBar("Converting Terrain Details", "Progress...", progress);
        }

        EditorUtility.ClearProgressBar();
    }
    [ContextMenu("Objects/Setup Customized Terrain Details Placeholders")]
    public async void SetupPlaceholders()
    {
        TerrainDetailConverter[] converters = FindObjectsOfType<TerrainDetailConverter>();
        float progress = 0f;
        float increment = 1f / converters.Length;

        foreach (TerrainDetailConverter converter in converters)
        {
            await converter.DoThePlaceholderConversion(newDetailLayersToReplace, newThreshold, placeholder, GlobalRange);
            progress += increment;
            EditorUtility.DisplayProgressBar("Converting Terrain Details", "Progress...", progress);
        }

        EditorUtility.ClearProgressBar();
    }
    public async void SetupPlaceholdersNoProgress()
    {
        TerrainDetailConverter[] converters = FindObjectsOfType<TerrainDetailConverter>();

        foreach (TerrainDetailConverter converter in converters)
        {
            foreach (DetailLayerGroup dlg in layerGroups)
            {
                await converter.DoThePlaceholderConversion(dlg.DetailGroupLayers, dlg.GroupThreshold, dlg.GroupPlaceholder, (int)dlg.GroupThreshold, PlaceholderGroups);
            }
            //await converter.DoThePlaceholderConversion(newDetailLayersToReplace, newThreshold, placeholder, GlobalRange);
        }

    }

    [ContextMenu("Details/Remove Original Terrain Details")]
    public async void RemoveTerrainDetails()
    {
        TerrainDetailConverter[] converters = FindObjectsOfType<TerrainDetailConverter>();
        float progress = 0f;
        float increment = 1f / converters.Length;

        foreach (TerrainDetailConverter converter in converters)
        {
            await converter.RemoveOriginalDetails();
            progress += increment;
            EditorUtility.DisplayProgressBar("Removing Terrain Details", "Progress...", progress);
        }

        EditorUtility.ClearProgressBar();
    }
    [ContextMenu("Details/Remove Original And Additional Terrain Details")]
    public async void RemoveOriginalAndAdditionalTerrainDetails()
    {
        TerrainDetailConverter[] converters = FindObjectsOfType<TerrainDetailConverter>();
        float progress = 0f;
        float increment = 1f / converters.Length;

        foreach (TerrainDetailConverter converter in converters)
        {
            await converter.RemoveOriginalDetails(additionalLayersToRemove);
            progress += increment;
            EditorUtility.DisplayProgressBar("Removing Terrain Details", "Progress...", progress);
        }

        EditorUtility.ClearProgressBar();
    }
    [ContextMenu("Details/Remove Only Additional Terrain Details")]
    public async void RemoveAdditionalTerrainDetails()
    {
        TerrainDetailConverter[] converters = FindObjectsOfType<TerrainDetailConverter>();
        float progress = 0f;
        float increment = 1f / converters.Length;

        foreach (TerrainDetailConverter converter in converters)
        {
            await converter.RemoveAdditionalDetails(additionalLayersToRemove);
            progress += increment;
            EditorUtility.DisplayProgressBar("Removing Terrain Details", "Progress...", progress);
        }

        EditorUtility.ClearProgressBar();
    }


    [ContextMenu("Objects/Remove Spawned GameObjects")]
    public async void RemoveSpawnedGameObjects()
    {
        TerrainDetailConverter[] converters = FindObjectsOfType<TerrainDetailConverter>();
        float progress = 0f;
        float increment = 1f / converters.Length;

        foreach (TerrainDetailConverter converter in converters)
        {
            await converter.RemoveSpawnedObjects();
            progress += increment;
            EditorUtility.DisplayProgressBar("Removing Spawn Objects", "Progress...", progress);
        }

        EditorUtility.ClearProgressBar();
    }

    [ContextMenu("Trees/Place Prototypes at Tree Instances")]
    public async void ReplaceTreesWithGameObjects()
    {
        TerrainDetailConverter[] converters = FindObjectsOfType<TerrainDetailConverter>();
        float progress = 0f;
        float increment = 1f / converters.Length;

        foreach (TerrainDetailConverter converter in converters)
        {
            await converter.DoTheNewConversionTrees();
            progress += increment;
            EditorUtility.DisplayProgressBar("Replacing Tree Instances", "Progress...", progress);
        }

        EditorUtility.ClearProgressBar();
    }

    [ContextMenu("Trees/Remove Original Tree Instances")]
    public async void RemoveReplacedTrees()
    {
        TerrainDetailConverter[] converters = FindObjectsOfType<TerrainDetailConverter>();
        float progress = 0f;
        float increment = 1f / converters.Length;

        foreach (TerrainDetailConverter converter in converters)
        {
            await converter.RemoveOriginalTrees();
            progress += increment;
            EditorUtility.DisplayProgressBar("Replacing Tree Instances", "Progress...", progress);
        }

        EditorUtility.ClearProgressBar();
    }
    [ContextMenu("Trees/Remove Spawned Tree GameObjects")]
    public async void RemoveSpawnedTrees()
    {
        TerrainDetailConverter[] converters = FindObjectsOfType<TerrainDetailConverter>();
        float progress = 0f;
        float increment = 1f / converters.Length;

        foreach (TerrainDetailConverter converter in converters)
        {
            await converter.RemoveSpawnedTrees();
            progress += increment;
            EditorUtility.DisplayProgressBar("Removing Spawn Objects", "Progress...", progress);
        }

        EditorUtility.ClearProgressBar();
    }


    [ContextMenu("Clear Cache")]
    public async void ResetData()
    {
        TerrainDetailConverter[] converters = FindObjectsOfType<TerrainDetailConverter>();
        float progress = 0f;
        float increment = 1f / converters.Length;

        foreach (TerrainDetailConverter converter in converters)
        {
            await converter.Reset_Data();
            progress += increment;
            EditorUtility.DisplayProgressBar("Clearing Converted Layer Data", "Progress...", progress);
        }

        EditorUtility.ClearProgressBar();
    }


    private IEnumerator CullPlaceholdersPeriodically()
    {
        if (RuntimeGeneration && UseGridBasedCulling)
        {
            while (true)
            {
                yield return new WaitForSeconds(cullingInterval);

                // Perform culling logic
                CullPlaceholders();
            }
        }
    }
    private void CullPlaceholders()
    {
        foreach (GameObject parent in PlaceholderGroups)
        {
            float distanceToPlayer = Vector3.Distance(parent.transform.position, playerTransform.transform.position);

            if (distanceToPlayer > cullingDistance)
            {
                // Deactivate all placeholders in this cell
                parent.SetActive(false);
            }
            else
            {
                // Activate all placeholders in this cell
                parent.SetActive(true);
            }
        }
    }
}
