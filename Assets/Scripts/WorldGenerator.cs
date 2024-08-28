using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Generates a world from a <see cref="TileSet"> via a basic wave function collaspe implementation
/// Tile are generated on a 2d plane, where Unity's x axis is consider left/right and Unity;s Z axis is considered up/down
/// </summary>
public class WorldGenerator : MonoBehaviour
{

    [SerializeField] private Vector2Int gridDimensions = new Vector2Int(20,20);

    [SerializeField] private TileSet tilesSet;

    //These tiles get placed first at specified location
    //This is quite hard coded, would ideally add rules to Tileset / have SpecialTileData class derive from TileData etc.
    //And add randmisation to postion of these points
    [Header("Special tiles")] 

    [SerializeField] private TileDataSO playerSpawnTile;
    [SerializeField] private Vector2Int playerSpawnPoint;

    [SerializeField] private TileDataSO enemySpawnTile;
    [SerializeField] private Vector2Int enemySpawnPoint;

    private TileData[,] tileGrid;

    /// <summary>
    /// The size of tiles currently being generated
    /// </summary>
    private float currentTileSize;

    Vector3 tileGridWorldOffset;

    private List<GameObject> spawnedGOs = new List<GameObject>();

    public int GridWidth { get => gridDimensions.x; set => gridDimensions.x = value; }
    public int GridHeight { get => gridDimensions.y; set => gridDimensions.y = value; }

    public int TileCount => gridDimensions.x * gridDimensions.y;



    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
    }


    public void GenerateGrid()
    {
        ClearCurrentGrid(); 

        tileGrid = new TileData[gridDimensions.x, gridDimensions.y];

        currentTileSize = tilesSet.TileSize;

        tileGridWorldOffset.x = gridDimensions.x * .5f * currentTileSize * -1;
        tileGridWorldOffset.z = gridDimensions.y * .5f * currentTileSize * -1;

        //Vector2 startGridPosition = (Vector2)transform.position + new Vector2(startXOffset, startYOffset);
        //Vector2 currentGridPostion = startGridPosition;

        Vector2Int startGridPosition = new Vector2Int(0, 0);
        Vector2Int currentGridPosition = startGridPosition;

        //Place special spawn tiles first. Ideally would take a less hard coded approach
        PlaceTile(playerSpawnTile.TileData, playerSpawnPoint);
        PlaceTile(enemySpawnTile.TileData, enemySpawnPoint);

        List<TileData> tileSetWithRotatedVariants = GenerateListWithRotatedVariants(tilesSet.GetTileDataList());

        //Could improve this by starting from centre....
        //Also no means of resolving conflicting nodes at this point, resulting in some errors in grass patches etc.
        //Plus ideally would want to find some way to garantee there are not impassable areas (e.g. draw path between spawn points)
        for (; currentGridPosition.x < gridDimensions.x; currentGridPosition.x++, currentGridPosition.y = 0)
        {
            for (; currentGridPosition.y < gridDimensions.y; currentGridPosition.y++)
            {
                //Skip if tile already occupied, e.g. spawn point or byprevious generation
                if (tileGrid[currentGridPosition.x, currentGridPosition.y] != null)
                    continue;

                TileData tileToUse = PickValidTile(currentGridPosition, tileSetWithRotatedVariants);
                PlaceTile(tileToUse, currentGridPosition);
            }
        }
    }

    private void PlaceTile(TileData tileData, Vector2Int currentGridPostion)
    {
        GameObject PrefabToSpawn = tileData.Prefab;

        float worldXPosition = tileGridWorldOffset.x + currentGridPostion.x * currentTileSize;
        float worldzPosition = tileGridWorldOffset.z + currentGridPostion.y * currentTileSize;

        Vector3 SpawnPoint = new Vector3(worldXPosition, 0, worldzPosition);

        int rotateToApply = tileData.RotationsToApplyToPrefab;

        Quaternion rotation = Quaternion.Euler(0, rotateToApply, 0);

        GameObject newInstance = Instantiate(PrefabToSpawn, SpawnPoint, rotation, transform);
        spawnedGOs.Add(newInstance);

        tileGrid[currentGridPostion.x, currentGridPostion.y] = tileData;
    }

    /// <summary>
    /// Takes the Tile list and added rotated variants if applicable
    /// </summary>
    /// <param name="originalTileset"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Tries to pick a valid tile to place at this cell
    /// </summary>
    /// <param name="currentGridPosition"></param>
    /// <param name="tileset"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Picks a TileData at random favouring those with higher weights 
    /// </summary>
    /// <param name="potentialTiles"></param>
    /// <returns></returns>
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
        //NOTE : since the current method for placeing tiles simply goes through rows from bottom up
        //checking up and right is redundant. However, improvements to the generation process (e.g. fixing errors, more random approach to placement)
        //would likely need all four checks

        //Remove any tiles not compatable with top socket
        if (TryGetNeighborTileData(currentGridPosition, Vector2Int.up, out TileData aboveTile))
            potentialTiles.RemoveAll(x => x.Sockets.Up != aboveTile.Sockets.Down);
       
        //Remove any tiles not compatable with right socket
        if (TryGetNeighborTileData(currentGridPosition, Vector2Int.right, out TileData rightTile))
            potentialTiles.RemoveAll(x => x.Sockets.Right != rightTile.Sockets.Left);

        //Remove any tiles not compatable with bottom socket
        if (TryGetNeighborTileData(currentGridPosition, Vector2Int.down, out TileData belowTile))
            potentialTiles.RemoveAll(x => x.Sockets.Down != belowTile.Sockets.Up);

        //Remove any tiles not compatable with left socket
        if (TryGetNeighborTileData(currentGridPosition, Vector2Int.left, out TileData leftTile))
            potentialTiles.RemoveAll(x => x.Sockets.Left != leftTile.Sockets.Right);

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
        return (positionToCheck.x >= 0 && positionToCheck.x < gridDimensions.x) && (positionToCheck.y >= 0 && positionToCheck.y < gridDimensions.y);
    }

    public void ClearCurrentGrid()
    {

        foreach (GameObject spawnedGO in spawnedGOs) { 
        
            Destroy(spawnedGO);
        }

        spawnedGOs.Clear();
    }


    private void OnValidate()
    {
        
    }
}
