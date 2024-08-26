using UnityEngine;
using UnityEngine.Tilemaps;
/// <summary>
/// Holds data for tile that can be used as part of a <see cref="TileSet"/>.
/// 
/// Simple implementation relying on default inspector. In a fuller realisation would make
/// a custom inspector to make setting compatable tiles less time consuming and/or use
/// a more automated approach for setting compatable neighbors
/// </summary>
[CreateAssetMenu(fileName = "NewTileData", menuName = "Tiles/TileData")]
public class TileDataSO : ScriptableObject
{
    [SerializeField] TileData tileData;

    public TileData TileData { get => tileData; }

    private void SetTileNameToSONameIfEmpty()
    {
        if (tileData.TileName == null || tileData.TileName == string.Empty)
            tileData.TileName = this.name;
    }

    private void Reset()
    {
        tileData = new TileData();

        if (this != null)
            SetTileNameToSONameIfEmpty();
    }

    private void OnValidate()
    {
        SetTileNameToSONameIfEmpty();
    }
}
