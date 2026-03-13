using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class MapManager : MonoBehaviour
{
    public Tilemap blockLayer;
    public Tilemap breakLayer;
    public Dictionary<Vector3Int, BlockState> allBlocks = new Dictionary<Vector3Int, BlockState>();

    public event Action<Vector3Int, BlockData> BlockPlaced;
    public event Action<Vector3Int, BlockData> BlockRemoved;

    public BlockData blockToPlace;
    public BlockData blockToReadDef;

    public BoundsInt readBounds;
    public TileBase[] breakLevelTiles;
    public List<BlockState> blocksToTick = new List<BlockState>();

    public AllSystems allSystems;

    void Awake()
    {
        ReadMapBlocks();
    }

    void Start()
    {
        //ReadMapBlocks();
    }
    void Update()
    {
        TickBlocks();
    }

    
    public bool HasBlock(Vector3Int pos)
    {
        foreach (var key in allBlocks.Keys) //this is too much computation for a common function.
        {
            if (key == pos)
            {
                return true;
            }
        }
        if (blockLayer.HasTile(pos))
        {
            return true;
        }
        return false;
    }

    public void PlaceBlock(Vector3Int pos, BlockData blockData) //Add FX boolean
    {
        BlockState newState = new BlockState(blockData,pos,allSystems.behaviourAdder,true);
        PlaceBlockWithState(pos,newState);
    }
    public void PlaceBlockWithState(Vector3Int pos,BlockState blockState)
    {
        if (HasBlock(pos))
        {
            RemoveBlock(pos,false);
        }
        blockLayer.SetTile(pos, blockState.blockData.blockTile);
        blockState.blockPos = pos;
        allBlocks.Add(pos,blockState);
        if (blockState.blockData.tickingBlock)
            {blocksToTick.Add(blockState);}
        //Debug.Log("there are :"+blockState.behaviours.Count+" behaviors");
        blockState.OnPlaced(this,pos);
        BlockPlaced?.Invoke(pos, blockState.blockData);
        //Debug.Log(allBlocks[pos].behaviour);
    }

    public void RemoveBlock(Vector3Int pos, bool withAnim) //Add FX boolean to trigger particles and soundFX
    {
        if (HasBlock(pos))
        {
            blockLayer.SetTile(pos, null); //Remove from tiles
            breakLayer.SetTile(pos, null); //Remove the break thing
            BlockRemoved?.Invoke(pos, allBlocks[pos].blockData); //notify the listeners

            
            blocksToTick.Remove(allBlocks[pos]); //Remove from blocksToTick
            allBlocks[pos].OnRemoved(this,pos); //Report to the BlockState
            allBlocks.Remove(pos);              //Remove from the dictionary
        }
        else
        {
            //No block to remove
        }
    }

    public BlockState GetBlock(Vector3Int pos) //Change next to blockstate
    {
        if (HasBlock(pos))
        {
            return allBlocks[pos];
        }
        else
        {
            return null;//No block to get
            //throw;
        }
    }

    public void MoveBlock(Vector3Int fromB, Vector3Int toB)
    {
        if (HasBlock(fromB) == false) //no block at from, removes at toB
        {
            RemoveBlock(toB,false);
            return;
        }
        if (HasBlock(toB)) //there's already a block at toB, replace it with fromB
        {
            RemoveBlock(toB,false);
        }
        
        allBlocks.TryGetValue(fromB,out BlockState blockToMove);
        allBlocks.Remove(fromB);
        allBlocks[toB] = blockToMove;

        breakLayer.SetTile(fromB,null);//doesnt remove it for some reaason
        blockLayer.SetTile(fromB,null);
        blockLayer.SetTile(toB,blockToMove.blockData.blockTile); //Place it w/ the breakness level

        blockToMove.OnPlaced(this,toB);
    }

    public void ReadMapBlocks()
    {
        foreach (var pos in blockLayer.cellBounds.allPositionsWithin)
        {
            TileBase tile = blockLayer.GetTile(pos);
            if (tile == null) 
            {continue;}

            //BlockType type = tileToBlockMap[tile];
            if (allBlocks.ContainsKey(pos))
            {
                continue;
            }
            blockLayer.SetTile(pos, null); 
            PlaceBlock(pos, blockToPlace); //only places this block for now, we can update with a secondary dictionary
        }
    }

    public void SetBreakLevel(Vector3Int pos, float brokenPercentage)
    {
        float divider = 1/breakLevelTiles.Length;
        if (brokenPercentage <= divider)
        {
            breakLayer.SetTile(pos, null);
        }
        else
        {
            int breakSpriteIndex = Mathf.FloorToInt((brokenPercentage-divider)*breakLevelTiles.Length);
            if (breakSpriteIndex > breakLevelTiles.Length-1)
            {
                Debug.Log("something's wrong, this block should've been broken by the State script");
                return;
            }
            breakLayer.SetTile(pos, breakLevelTiles[breakSpriteIndex]);
        }
    }
    public void TickBlocks()
    {
        for (int i = blocksToTick.Count - 1; i >= 0; i--)
        {
            blocksToTick[i].Tick(this,Time.deltaTime);
        }
    }
    public Vector3Int FloatToGridPos(Vector3 positionToGrid)
    {
        return new Vector3Int(Mathf.RoundToInt(positionToGrid.x),Mathf.RoundToInt(positionToGrid.y),0);
    }
}
