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
        if (owner.entityType == "player")
        {
            owner.GetComponent<PlayerGeneral>().Announce("All your effects have been cleared!",2f,new Color(0.9f,0.8f,0.8f,0.8f));
        }
        state.GetBehaviour<ConsumableItemBehaviour>().ItemUsed(owner,state,inventory,slot);
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
