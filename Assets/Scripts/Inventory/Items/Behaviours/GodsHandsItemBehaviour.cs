using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodsHandsItemBehaviour : IItemBehaviour
{
    
    public GodsHandsItemBehaviour()
    {
        
    }

    
    public void Use(EntityGeneral owner, ItemState state, Vector3 mousePos, float heldFor, Inventory inventory, int slot)
    {
        
    }
    public void RightClick(EntityGeneral owner, ItemState state, Vector3 mousePos, float heldFor, Inventory inventory, int slot)
    {
        
    }
    public void StartHolding(EntityGeneral owner, ItemState state, Inventory inventory, int slot)
    {
        owner.mapManager.gameObject.GetComponent<GodsHands>().MakeGodReal(true);
    }
    public void StopHolding(EntityGeneral owner, ItemState state, Inventory inventory, int slot)
    {
        owner.mapManager.gameObject.GetComponent<GodsHands>().MakeGodReal(false);
    }
}
