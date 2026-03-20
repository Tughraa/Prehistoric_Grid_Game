using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntidoteBehaviour : IItemBehaviour
{
    
    public AntidoteBehaviour(){} //we might need it later on

    public void Use(EntityGeneral owner, ItemState state, Vector3 mousePos, float heldFor, Inventory inventory, int slot)
    {
        Debug.Log("use has been called for antidote");
        owner.GetComponent<EntityStatusEffects>().RemoveAllEffects();
        state.GetBehaviour<ConsumableItemBehaviour>().ItemUsed(inventory,slot);
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
}
