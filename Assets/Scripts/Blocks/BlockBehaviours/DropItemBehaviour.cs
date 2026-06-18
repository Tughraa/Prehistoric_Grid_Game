using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemBehaviour : IBlockBehaviour
{
    string entityToDrop;
    int itemLeft;
    public DropItemBehaviour(string inEntityToDrop, int howMany)
    {
        entityToDrop = inEntityToDrop;
        itemLeft = howMany;
    }
    public void OnPlaced(MapManager map, Vector3Int pos, BlockState state)
    {
        
    }
    public void OnRemoved(MapManager map, Vector3Int pos, BlockState state)
    {

    }
    
    public void OnBreak(MapManager map, Vector3Int pos, BlockState state)
    {
        if (itemLeft > 0)
        {
            //GameObject.Instantiate(entityToDrop,pos,Quaternion.identity);
            map.allSystems.entitySummonSystem.SummonEntityFabOnName("throw_rock",pos);
            itemLeft--;
        }
    }
    public void Tick(MapManager map, Vector3Int pos, BlockState state, float dt)
    {
        
    }
}
