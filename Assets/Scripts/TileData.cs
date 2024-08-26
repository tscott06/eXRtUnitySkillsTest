using System;
using UnityEngine;

[Serializable]
public class TileData
{
    public string TileName;
    public GameObject Prefab;

    [SerializeField] private TileSockets sockets;

    //Modifer that can increase or decrease likehood of appearing from default of 1.
    //Rotations are counted as unique pieces by world generator (so weight is multiplied by 4)
    public float Weight = 1;

    [SerializeField] private bool allowRotatedVariants = true;
    private bool isRotatedVariant = false;
    private int rotationsToApplyToPrefab = 0;

    public TileData() { }

    public TileData(TileData OriginalTileData)
    {
        TileName = OriginalTileData.TileName;
        Prefab = OriginalTileData.Prefab;
        Sockets = OriginalTileData.sockets = OriginalTileData.Sockets;
    }

    public TileData(TileData originalTileData, TileRuleRotation rotateBy)
    {
      
        TileName = originalTileData.TileName;
        Prefab = originalTileData.Prefab;
     

        if(rotateBy != TileRuleRotation.None)
            this.isRotatedVariant = true;

        TileSockets origRules = originalTileData.sockets;

      

        switch (rotateBy)
        {
            case TileRuleRotation.None:
                Sockets = origRules;
                return;
               
            case TileRuleRotation.By90:
                Sockets = TileSockets.RotateRules90CW(origRules);
                break;

            case TileRuleRotation.By180:
                Sockets = TileSockets.RotateRules180CW(origRules);
                break;

            case TileRuleRotation.By270:
                Sockets = TileSockets.RotateRules270CW(origRules);
                break;


        }

        TileName += Enum.GetName(typeof(TileRuleRotation), rotateBy).Replace("By", "_");

        this.rotationsToApplyToPrefab = (int)rotateBy;
    }

    public TileSockets Sockets { get => sockets; set => sockets = value; }
    public bool AllowRotatedVariants { get => allowRotatedVariants; }
    public int RotationsToApplyToPrefab { get => rotationsToApplyToPrefab; }

    public void MarkAsRotatedVariant() => isRotatedVariant = true;
    public bool IsCompatibleWithAboveTile(TileData above) => this.Sockets.Up == above.Sockets.Down;
    public bool IsCompatibleWithBelowTile(TileData below) => this.Sockets.Down == below.Sockets.Up;
    public bool IsCompatibleWithRightTile(TileData right) => this.Sockets.Right == right.Sockets.Left;
    public bool IsComptibleWithLeftTile(TileData left) => this.Sockets.Left == left.Sockets.Right;

 
}

public enum TileRuleRotation
{
    None = 0,
    By90 = 90,
    By180 = 180,
    By270 = 270
}

/// <summary>
/// Defines rules for this tile;
/// What tiles can neighbor this tile, on a 2D plain where Unity's x axis is consider left/right and Unity; Z axis is considered up/down
/// </summary>
[Serializable]
public struct TileSockets
{
    public SocketTag Up;
    public SocketTag Right;
    public SocketTag Down;
    public SocketTag Left;


    public TileSockets(SocketTag up, SocketTag right, SocketTag down, SocketTag left)
    {
        Up = up;
        Right = right;
        Down = down;
        Left = left;
    }

    public static TileSockets RotateRules90CW(TileSockets orig)
    {
        TileSockets newRules = new TileSockets();
     
        newRules.Up = orig.Left;
        newRules.Right = orig.Up;
        newRules.Down = orig.Right;
        newRules.Left = orig.Down;

        return newRules;
    }

    public static TileSockets RotateRules180CW(TileSockets orig)
    {
        TileSockets newRules = new TileSockets();

        newRules.Up = orig.Down;
        newRules.Right = orig.Left;
        newRules.Down = orig.Up;
        newRules.Left = orig.Right;


        return newRules;
    }

    public static TileSockets RotateRules270CW(TileSockets orig)
    {
        TileSockets newRules = new TileSockets();

        newRules.Up = orig.Right;
        newRules.Right = orig.Down;
        newRules.Down = orig.Left;
        newRules.Left = orig.Up;

        return newRules;
    }
}

/// <summary>
/// Tiles each have 4 Sockets ()
/// 
/// </summary>
public enum SocketTag
{
    BasicGround = 0,
    Grass = 1,
    GrassTransition = 2,
    Wall = 3
}