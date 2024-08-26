using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// A set of tiles that can be used by <see cref="WorldGenerator"/> to generate a world
/// </summary>
[CreateAssetMenu(fileName = "NewTileSet", menuName = "Tiles/TileSet")]
[Serializable]
public class TileSet : ScriptableObject
{
    [SerializeField] private string tileSetName;
    [SerializeField] private float tileSize = 1;

    [Space]

    [SerializeField] private TileDataSO fallBack;
    [SerializeField] private List<TileDataSO> tiles;

    public List<TileData> GetTileDataList()
    {
        return tiles.Select((x) => x.TileData).ToList();
    }

    public List<TileDataSO> Tiles { get => tiles; }
    public string TileSetName { get => tileSetName;}
    public float TileSize { get => tileSize; }
    public TileDataSO FallBack { get => fallBack; }

    private void OnValidate()
    {
        if (tileSetName == null || tileSetName == string.Empty)
        {
            tileSetName = name;
        }

        tiles = tiles.Distinct().ToList(); 
    }
}
