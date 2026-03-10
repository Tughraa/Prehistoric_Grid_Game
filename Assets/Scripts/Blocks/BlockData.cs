using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class BlockData : ScriptableObject
{
    public TileBase blockTile;
    public string id = "undef";
    public string blockName = "Undefined Block";
    public bool hasCollision = true;
    public float toughness = 1f;
    public List<string> tags;
    public bool tickingBlock = false; //This can be an IBlockBehaviour {get;} instead
}
