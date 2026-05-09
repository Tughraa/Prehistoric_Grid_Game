using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeItemBehaviour : IItemBehaviour
{
    //float ropeDist = 
    GameObject currentTie = null;
    //bool currentlyThrown = false;
    public RopeItemBehaviour(ItemState state) //Constructor
    {
        state.GetBehaviour<ThrowableItemBehaviour>().throwOnUse = false;
    }
    public void Use(EntityGeneral owner, ItemState state, Vector3 mousePos, float heldFor, Inventory inventory, int slot)
    {
        //state.GetBehaviour<ThrowableItemBehaviour>().throwOnUse = false;
        if (currentTie == null)
        {
            currentTie = state.GetBehaviour<ThrowableItemBehaviour>().ThrowItem(owner,mousePos,500f, state);
        }
        else
        {
            owner.Knockback((currentTie.transform.position-owner.transform.position),400f);//change 400
            GameObject.Destroy(currentTie); //This can't happen if it's other entities, check for RopeCores
        }
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
