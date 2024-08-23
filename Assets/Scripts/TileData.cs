using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "NewTileData", menuName = "TileData")]
[Serializable]
public class TileData : ScriptableObject
{
    public string TileName;
    public GameObject Prefab;
    [Range(0,1)] public float Weight = 1;

    public TileRules Rules;

    private void OnValidate()
    {
        if(TileName == null || TileName == string.Empty)
            TileName = this.name;
    }
}

[Serializable]
public class TileRules
{
    public ValidNeighborsList Up;
    public ValidNeighborsList Right;
    public ValidNeighborsList Down;
    public ValidNeighborsList Left;

    public bool CanRotate = true;
    
    //[Space]
    //public ValidNeighborsList Above;
    //public ValidNeighborsList Below;

}

[Serializable]
public class ValidNeighborsList {

    [SerializeField] private List<TileData> validNeighbors;
    //[SerializeField] private bool allowEmpty = true;

    public List<TileData> ValidNeighbors { get => validNeighbors;}
}