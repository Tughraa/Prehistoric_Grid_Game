using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBreakBehaviour : IItemBehaviour
{
    float pushBackAmount = 200f;
    float blockbreakAmount = 0.1f;
    public RockBreakBehaviour()
    {
        //pushBackAmount
    }
    public void Use(EntityGeneral owner, ItemState state, Vector3 mousePos, float heldFor, Inventory inventory, int slot)
    {
        
    }
    public void RightClick(EntityGeneral owner, ItemState state, Vector3 mousePos, float heldFor, Inventory inventory, int slot)
    {
        
    }
    public void StartHolding(EntityGeneral owner, ItemState state, Inventory inventory, int slot)
    {

    }
    public void StopHolding(EntityGeneral owner, ItemState state, Inventory inventory, int slot)
    {

    }
    public void CantThrow(EntityGeneral owner, Vector3 usePosition)
    {
        //inventory.ClearItemSlot(slot);
        MapManager map = owner.mapManager;
        Vector3Int checkPosGrid = map.FloatToGridPos(usePosition);
        if (map.HasBlock(checkPosGrid))
        {
            BlockState hitBlock = map.GetBlock(checkPosGrid);
            hitBlock.BlockBreak(blockbreakAmount,map);
        }
        owner.Knockback(owner.transform.position-usePosition,pushBackAmount);
    }
}
