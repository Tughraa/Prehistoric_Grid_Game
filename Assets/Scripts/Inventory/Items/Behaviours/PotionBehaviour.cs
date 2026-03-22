using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionBehaviour : IItemBehaviour
{
    IStatusEffect currentEffect;
    Color currentColor;
    
    public PotionBehaviour(IStatusEffect effect)
    {
        currentEffect = effect;
        currentColor = effect.GetColor;
    }

    public void Use(EntityGeneral owner, ItemState state, Vector3 mousePos, float heldFor, Inventory inventory, int slot)
    {
        owner.GetComponent<EntityStatusEffects>().AddEffect(currentEffect);
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
