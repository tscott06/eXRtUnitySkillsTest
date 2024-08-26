using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Generates a world from a <see cref="TileSet"> via a basic wave function collaspe implementation
/// Tile are generated on a 2d plane, where Unity's x axis is consider left/right and Unity;s Z axis is considered up/down
/// </summary>
public class WorldGenerator : MonoBehaviour
{

    [SerializeField] private int gridWidth = 20;
    [SerializeField] private int gridHeight = 20;

    [SerializeField] private TileSet tilesSet;

    private TileData[,] tileGrid;
    //private Queue<Vector2Int> collaspeQueue;

    private List<GameObject> spawnedGOs = new List<GameObject>();

    /*
    //List of offsets that gets looped through when checking neighbors in grid
    private Vector2Int[] offsets = new Vector2Int[]
    {
        new Vector2Int(0, 1), // PosZ (up from this tile in grid)
        new Vector2Int(0, -1), // NegZ (down from this tile in grid)
        new Vector2Int(1, 0), // PosX (right from this tile in grid)
        new Vector2Int(-1, 0),// nEGx (left from this tile in grid)
    };
    */

    public int GridWidth { get => gridWidth; set => gridWidth = value; }
    public int GridHeight { get => gridHeight; set => gridHeight = value; }

    public int TileCount => gridWidth * gridHeight;


    //private Dictionary<Vector2Int, GameObject> spawnedGODictionary = new Dictionary<Vector2Int, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();


    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            ClearCurrentGrid();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateGrid();
        }
    }

    public void GenerateGrid()
    {
        ClearCurrentGrid(); 

        tileGrid = new TileData[gridWidth, gridHeight];

        float tileSize = tilesSet.TileSize;

        float startXOffset = gridWidth * .5f * tileSize * -1;
        float startZOffset = gridHeight * .5f * tileSize * -1;

        //Vector2 startGridPosition = (Vector2)transform.position + new Vector2(startXOffset, startYOffset);
        //Vector2 currentGridPostion = startGridPosition;

        Vector2Int startGridPosition = new Vector2Int(0, 0);
        Vector2Int currentGridPostion = startGridPosition;

        List<TileData> tileSetWithRotatedVariants = GenerateListWithRotatedVariants(tilesSet.GetTileDataList());

        //Could improve this by starting from centre....
        for (; currentGridPostion.x < gridWidth; currentGridPostion.x++, currentGridPostion.y = 0)
        {
            for (; currentGridPostion.y < gridHeight; currentGridPostion.y++)
            {
                TileData tileToUse = PickValidTile(currentGridPostion, tileSetWithRotatedVariants);

                GameObject PrefabToSpawn = tileToUse.Prefab;

                float worldXPosition = startXOffset + currentGridPostion.x * tileSize;
                float worldzPosition = startZOffset + currentGridPostion.y * tileSize;

                Vector3 SpawnPoint = new Vector3(worldXPosition, 0, worldzPosition);

                int rotateToApply = tileToUse.RotationsToApplyToPrefab;

                Quaternion rotation = Quaternion.Euler(0, rotateToApply, 0);

                GameObject newInstance = Instantiate(PrefabToSpawn, SpawnPoint, rotation, transform);
                spawnedGOs.Add(newInstance);

                tileGrid[currentGridPostion.x, currentGridPostion.y] = tileToUse;
            }
        }
    }


    private List<TileData> GenerateListWithRotatedVariants(List<TileData> originalTileset)
    {
        List<TileData> updatedTileSet = new List<TileData>();
        updatedTileSet.AddRange(originalTileset);

        foreach (TileData tileData in originalTileset)
        {
            if (tileData.AllowRotatedVariants)
            {
                TileData[] rotateVariants = new TileData[3];

                rotateVariants[0] = new TileData(tileData, TileRuleRotation.By90);
                rotateVariants[1] = new TileData(tileData, TileRuleRotation.By180);
                rotateVariants[2] = new TileData(tileData, TileRuleRotation.By270);

                updatedTileSet.AddRange(rotateVariants);
            }
        }

        return updatedTileSet;
    }

    private TileData PickValidTile(Vector2Int currentGridPosition, List<TileData> tileset)
    {
        List<TileData> potentialTiles = new List<TileData>(tileset);
        CollapsePotentialTiles(currentGridPosition, potentialTiles);

        TileData tileToUse = null;

        if (potentialTiles.Count == 0)
        {
            tileToUse = tilesSet.FallBack.TileData;
            //Debug.Log("Using fallback tile");
        }
        else
        {
            tileToUse = GetTileByWeightedRandom(potentialTiles);
        }

        return tileToUse;
    }

    private TileData GetTileByWeightedRandom(List<TileData> potentialTiles)
    {
        List<float> weights = potentialTiles.Select(x => x.Weight).ToList();

        float p = UnityEngine.Random.Range(0f, (float)weights.Sum());

        TileData tileToUse = null;

        foreach (TileData tileData in potentialTiles) 
        { 
            if(p <= tileData.Weight)
            {
                tileToUse = tileData;
                break;
            }

            p -= tileData.Weight;
        }

        return tileToUse;
    }

    /// <summary>
    /// Collapses the passes in TileList by removing tiles that cannot connect to the current tile
    /// </summary>
    /// <param name="currentGridPosition"></param>
    /// <param name="potentialTiles"></param>
    private void CollapsePotentialTiles(Vector2Int currentGridPosition, List<TileData> potentialTiles)
    {
        Debug.Log($"Current grid posit {currentGridPosition}");

        //Remove any tiles not compatable with top socket
        if (TryGetNeighborTileData(currentGridPosition, Vector2Int.up, out TileData aboveTile))
        {
            Debug.Log("Checking up nieghbor");
            //potentialTiles.RemoveAll(x => !x.IsCompatibleWithAboveTile(aboveTile));
            potentialTiles.RemoveAll(x => x.Sockets.Up != aboveTile.Sockets.Down);
        }

        //Remove any tiles not compatable with right socket
        if (TryGetNeighborTileData(currentGridPosition, Vector2Int.right, out TileData rightTile))
        {
            Debug.Log("Checking right nieghbor");
            //potentialTiles.RemoveAll(x => !x.IsCompatibleWithRightTile(rightTile));
            potentialTiles.RemoveAll(x => x.Sockets.Right != rightTile.Sockets.Left);
        }

        //Remove any tiles not compatable with bottom socket
        if (TryGetNeighborTileData(currentGridPosition, Vector2Int.down, out TileData belowTile))
        {
            Debug.Log("Checking down nieghbor");
            //potentialTiles.RemoveAll(x => !x.IsCompatibleWithBelowTile(belowTile));
            potentialTiles.RemoveAll(x => x.Sockets.Down != belowTile.Sockets.Up);
        }

        //Remove any tiles not compatable with left socket
        if (TryGetNeighborTileData(currentGridPosition, Vector2Int.left, out TileData leftTile))
        {
            Debug.Log("Checking left nieghbor");
            //potentialTiles.RemoveAll(x => !x.IsCompatibleWithBelowTile(belowTile));
            potentialTiles.RemoveAll(x => x.Sockets.Left != leftTile.Sockets.Right);
        }

        Debug.Log("Match count = " + potentialTiles.Count);

    }

    private bool TryGetNeighborTileData(Vector2Int currentGridPosition, Vector2Int offset, out TileData neighborTile)
    {
        Vector2Int neighborCoord = currentGridPosition + offset;
        neighborTile = null;

        if (!IsInsideGrid(neighborCoord))
            return false;

        neighborTile = tileGrid[neighborCoord.x, neighborCoord.y];

        return neighborTile != null;
    }

    private bool IsInsideGrid(Vector2Int positionToCheck)
    {
        return (positionToCheck.x >= 0 && positionToCheck.x < gridWidth) && (positionToCheck.y >= 0 && positionToCheck.y < gridHeight);
    }

    public void ClearCurrentGrid()
    {

        foreach (GameObject spawnedGO in spawnedGOs) { 
        
            Destroy(spawnedGO);
        }

        spawnedGOs.Clear();
    }

}
