using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DurabilityBehaviour : IItemBehaviour
{
    float maxDurability;
    public float currentDurability;

    public DurabilityBehaviour(float inMaxDurability)
    {
        maxDurability = inMaxDurability;
        currentDurability = maxDurability;
    }
    public void Use(EntityGeneral owner, ItemState state, Vector3 mousePos, float heldFor, Inventory inventory, int slot) {}
    public void RightClick(EntityGeneral owner, ItemState state, Vector3 mousePos, float heldFor, Inventory inventory, int slot) {}
    public void StartHolding(EntityGeneral owner, ItemState state, Inventory inventory, int slot) {}
    public void StopHolding(EntityGeneral owner, ItemState state, Inventory inventory, int slot) {}

    public void ItemUsed(EntityGeneral owner, ItemState state, Inventory inventory, int slot, float amount) //Also handle effects and maybe a showcase
    {
        Debug.Log("durability was: "+currentDurability);
        currentDurability -= amount;
        Debug.Log("durability is: "+currentDurability);
        if (currentDurability <= 0f)
        {
            inventory.ClearItemSlot(slot); //Do Some Effects Here l8er
            if (owner.entityType == "player")
            {
                owner.GetComponent<PlayerGeneral>().Announce("Item Broke! (imagine sound and effects)",1f,new Color(1f,0.9f,0.9f,0.8f));
            }
        }
        //UpdateVisual()
    }
    public float DurabilityFraction()
    {
        return currentDurability/maxDurability;
    }
}
