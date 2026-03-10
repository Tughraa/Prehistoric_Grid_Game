using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableItemBehaviour : IItemBehaviour //A problem with this approach is that it doesn't understand if the item's actually used
{
    public ConsumableItemBehaviour() //Right now nothing but maybe sound effects or anim in the futures
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

    }
    public void StopHolding(EntityGeneral owner, ItemState state, Inventory inventory, int slot)
    {

    }
    public void ItemUsed(Inventory inventory, int slot)
    {
        inventory.ClearItemSlot(slot);
    }
}
