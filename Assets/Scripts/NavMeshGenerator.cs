using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshGenerator : MonoBehaviourSingleton<NavMeshGenerator>
{
    [SerializeField] private GameObject navMeshRoot = null;

    [Header("Bake Settings")]
    [SerializeField] private float agentRadius;
    [SerializeField] private float agentHeight;
    [SerializeField, Range(0, 90)] private float maxSlope;
    [SerializeField] private float stepHeight;
    [SerializeField] private float dropHeight;
    [SerializeField] private float jumpDistance;
    [SerializeField] private float minRegionArea;

    [SerializeField] private bool overrideVoxelSize;
    [SerializeField] private float voxelSize;

    [SerializeField] private bool overrideTileSize;
    [SerializeField] private int tileSize;

    //private NavMeshData currentData;

    private List<GameObject> gameObjects = new List<GameObject>();
    private NavMeshData navMeshData;
    private NavMeshDataInstance navMeshInstance;
    private List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();


    public void SetNavMeshElements(List<GameObject> values)
    {
        gameObjects = values;
    }

    private void OnEnable()
    {
        navMeshData = new NavMeshData();
        navMeshInstance = NavMesh.AddNavMeshData(navMeshData);

        WorldGenerator.Instance.OnWorldGenerationComplete.AddListener(WorldBuildCompleteHnadler);
    }

    protected override void Awake()
    {
        base.Awake();

        if (navMeshRoot == null) { navMeshRoot = new GameObject("NavMeshRoot"); }
    }

    //This dependacy ain't great
    private void WorldBuildCompleteHnadler()
    {
        gameObjects = WorldGenerator.Instance.SpawnedGOs;
        BuildNavMesh();
    }

    public void BuildNavMesh()
    {
        //Debug.Log("Building nav mesh...");
        ClearData();

        int agentTypeCount = NavMesh.GetSettingsCount();
        if (agentTypeCount < 1) { return; }

        NavMeshBuildSettings navMeshBuildSettings = GetNavBuildSettings();
        List<NavMeshBuildSource> navMeshBuildSources = GetNavMeshBuildSources();

        //Debug.Log("Nav mesh retrieval complete");
        /*
        foreach (var source in navMeshBuildSources) {
            Debug.Log($"NavMeshBuildSource " + source.sourceObject.name);
        }
        */
        
        Bounds bounds = new Bounds(navMeshRoot.transform.position, new Vector3(999, 999, 999)); //ideally would've used bound from world generator
        NavMeshBuilder.UpdateNavMeshDataAsync(navMeshData, navMeshBuildSettings, navMeshBuildSources, bounds);

        NavMesh.AddNavMeshData(navMeshData);

        //Debug.Log("Built nav mesh...");
    }

    public void ClearData()
    {
       NavMesh.RemoveAllNavMeshData();
    }

    private NavMeshBuildSettings GetNavBuildSettings()
    {
        return  new NavMeshBuildSettings()
        {
            agentRadius = agentRadius,
            agentHeight = agentHeight,
            agentSlope = maxSlope,
            minRegionArea = minRegionArea,

            overrideVoxelSize = overrideVoxelSize,
            voxelSize = voxelSize,

            overrideTileSize = overrideTileSize,
            tileSize = tileSize,

            agentClimb = stepHeight
        };
    }

    private List<NavMeshBuildSource> GetNavMeshBuildSources()
    {
        List<NavMeshBuildSource> buildSources = new List<NavMeshBuildSource>();

        foreach(GameObject go in gameObjects)
        {
            TryGetNavMeshSourceFromGameObject(go, buildSources);
        }

        return buildSources;
    }

    private bool TryGetNavMeshSourceFromGameObject(GameObject go, List<NavMeshBuildSource> buildSources)
    {
        List<MeshFilter> meshFilters = go.GetComponentsInChildren<MeshFilter>().ToList();
        //List<Mesh> mesheFilter = go.GetComponentsInChildren<MeshFilter>().Select((x) => x.sharedMesh).ToList(); //This is ugly

        if (meshFilters == null || meshFilters.Count == 0)
        {
            Debug.LogError("No meshes found from " + go.name);
            return false;
        }

        foreach (MeshFilter meshFilter in meshFilters) 
        {
            NavMeshBuildSource src = new NavMeshBuildSource();

            src.shape = NavMeshBuildSourceShape.Mesh;
            src.sourceObject = meshFilter.sharedMesh;
            src.transform = meshFilter.transform.localToWorldMatrix;    
            src.area = 0;

            buildSources.Add(src);
        }

        return true;
    }
}
