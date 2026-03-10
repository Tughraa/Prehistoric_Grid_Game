using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPlacerBehaviour : IItemBehaviour
{
    public BlockData blockToPlace;
    public BlockPlacerBehaviour(BlockData inBlockToPlace)
    {
        blockToPlace = inBlockToPlace;
    }

    public void Use(EntityGeneral owner, ItemState state, Vector3 mousePos, float heldFor, Inventory inventory, int slot)
    {
        MapManager mapManager = owner.allSystems.mapManager;
        FireSystem fireSystem = owner.allSystems.fireSystem;
        Vector3Int blockPos = mapManager.FloatToGridPos(mousePos);
        if (mapManager.HasBlock(blockPos) == false) //if there's no block
        {
            mapManager.PlaceBlock(blockPos,blockToPlace);
            state.GetBehaviour<ConsumableItemBehaviour>().ItemUsed(inventory,slot);
        }
        //THIS MAKES IT WORK FOR FIRE BUT FIRE ONLY!!
        else if (fireSystem.BlockFireInteractable(mapManager.GetBlock(blockPos).blockData))
        {
            fireSystem.FireUpBlock(blockPos);
            state.GetBehaviour<ConsumableItemBehaviour>().ItemUsed(inventory,slot);
        }
    }
    public void RightClick(EntityGeneral owner, ItemState state, Vector3 mousePos, float heldFor, Inventory inventory, int slot)
    {
        
    }
    public void StartHolding(EntityGeneral owner, ItemState state, Inventory inventory, int slot)
    {
        //Debug.Log("started holding: "+state.itemData.id);
    }
    public void StopHolding(EntityGeneral owner, ItemState state, Inventory inventory, int slot)
    {
        
        //Debug.Log("stopped holding: "+state.itemData.id);
    }
}
