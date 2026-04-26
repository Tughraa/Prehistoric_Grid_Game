using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockState
{
    public BlockData blockData;
    public float brokenLevel = 0f;
    public float brokenMax = 5f;
    public Vector3Int blockPos;
    public List<IBlockBehaviour> behaviours = new List<IBlockBehaviour>();
    public bool tickBehave = false;

    BehaviourAdder behaviourAdder;
    
    public BlockState(BlockData inBlockData,Vector3Int inBlockPos,BehaviourAdder inBehaviourAdder, bool addBehaviours)
    {
        blockData = inBlockData;
        blockPos = inBlockPos;
        brokenMax = inBlockData.toughness;

        tickBehave = inBlockData.tickingBlock;
        
        behaviourAdder = inBehaviourAdder;
        if (addBehaviours)
        {behaviourAdder.AddBlockBehaviours(this);}
    }
    public bool BlockBreak(float amount, MapManager mapManager)
    {
        brokenLevel += amount;
        if (brokenLevel/brokenMax >= 1f)
        {
            //
            mapManager.RemoveBlock(blockPos,true);
            return true;
        }
        mapManager.SetBreakLevel(blockPos,brokenLevel/brokenMax);
        for (int i = behaviours.Count - 1; i >= 0; i--)
        {
            behaviours[i].OnBreak(mapManager,blockPos,this);
        }
        return false;
    }
    public void AddBehaviour(IBlockBehaviour behaviourToAdd)
    {
        behaviours.Add(behaviourToAdd);
    }

    public void OnPlaced(MapManager map, Vector3Int pos)
    {
        blockPos = pos;
        for (int i = behaviours.Count - 1; i >= 0; i--)
        {
            behaviours[i].OnPlaced(map, blockPos, this);
        }
    }

    public BlockState Clone(Vector3Int clonePos)
    {
        BlockState clonedBlock = new BlockState(blockData,blockPos,behaviourAdder,false);
        for (int i = 0; i < behaviours.Count; i++)
        {
            clonedBlock.AddBehaviour(behaviours[i]);
        }
        return clonedBlock;
    }
    public void OnRemoved(MapManager map, Vector3Int pos)
    {
        for (int i = behaviours.Count - 1; i >= 0; i--)
        {
            behaviours[i].OnRemoved(map, blockPos, this);
            //behaviours[i] = null; //The lack of this line creates a lingering problem
        }
    }

    public void Tick(MapManager map, float dt)
    {
        for (int i = behaviours.Count - 1; i >= 0; i--)
        {
            if (tickBehave)
            {
                behaviours[i].Tick(map, blockPos, this, dt);//
            }
        }
    }
    public bool HasBehaviour<T>() where T : IBlockBehaviour
    {
        for (int i = 0; i < behaviours.Count; i++)
        {
            if (behaviours[i] is T)
            {
                return true;
            }
        }
        return false;
    }
    public T GetBehaviour<T>() where T : class, IBlockBehaviour
    {
        for (int i = 0; i < behaviours.Count; i++)
        {
            if (behaviours[i] is T b)
            {
                return b;
            }
        }
        return null;
    }
}
