using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemState
{
    public EntityGeneral ownerEntity;
    public List<IItemBehaviour> behaviours = new List<IItemBehaviour>();
    public AllSystems allSystems;
    public ItemData itemData;
    
    public ItemState(ItemData inItemData, EntityGeneral owner, AllSystems inAllSystems, bool addBehaviours)
    {
        itemData = inItemData;
        ownerEntity = owner;
        allSystems = inAllSystems;
        if (addBehaviours)
        {allSystems.behaviourAdder.AddItemBehaviours(this);}
    }

    //BehaviourCalling
    public void ItemUse(EntityGeneral owner, Vector3 mousePos, float heldFor, Inventory inventory, int slot)
    {
        if (behaviours.Count <= 0)
        {
            return;
        }
        for (int i = behaviours.Count - 1; i >= 0; i--)
        {
            behaviours[i].Use(ownerEntity, this, mousePos, heldFor, inventory, slot);
        }
    }
    public void ItemRightClick(EntityGeneral owner, Vector3 mousePos, float heldFor, Inventory inventory, int slot)
    {
        if (behaviours.Count <= 0)
        {
            return;
        }
        for (int i = behaviours.Count - 1; i >= 0; i--)
        {
            behaviours[i].RightClick(ownerEntity, this, mousePos, heldFor, inventory, slot);
        }
    }
    public void ItemStartHolding(EntityGeneral owner, Inventory inventory, int slot)
    {
        if (behaviours.Count <= 0)
        {
            return;
        }
        for (int i = behaviours.Count - 1; i >= 0; i--)
        {
            behaviours[i].StartHolding(owner, this, inventory, slot);
        }
    }
    public void ItemStopHolding(EntityGeneral owner, Inventory inventory, int slot)
    {
        if (behaviours.Count <= 0)
        {
            return;
        }
        for (int i = behaviours.Count - 1; i >= 0; i--)
        {
            behaviours[i].StopHolding(owner, this, inventory, slot);
        }
    }
    
    //BehaviourManaging
    public void AddBehaviour(IItemBehaviour behaviourToAdd)
    {
        behaviours.Add(behaviourToAdd);
    }
    public bool HasBehaviour<T>() where T : IItemBehaviour
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
    public T GetBehaviour<T>() where T : class, IItemBehaviour
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
    
    //Uhmm idk clonin??
    public ItemState Clone(EntityGeneral givenOwner)
    {
        ItemState clonedItem = new ItemState(itemData,ownerEntity,allSystems,false);
        for (int i = 0; i < behaviours.Count; i++)
        {
            clonedItem.AddBehaviour(behaviours[i]);
        }
        return clonedItem;
    }
}
