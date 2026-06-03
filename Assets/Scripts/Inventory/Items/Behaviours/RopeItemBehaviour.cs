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
        //Throw it if it hasn't been thrown
        if (currentTie == null)
        {
            currentTie = state.GetBehaviour<ThrowableItemBehaviour>().ThrowItem(owner,mousePos,500f, state);
        }
        //Take it back if it has already been thrown
        else
        {
            BreakTie(owner,mousePos);
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
        BreakTie(owner, Vector3.zero);
    }
    public void BreakTie(EntityGeneral owner, Vector3 mousePos)
    {
        if (currentTie == null)
        {
            return;
        }
        Vector2 throwDir = (mousePos-owner.transform.position).normalized;
        //Hm
        RopeCore tiedRopeCore = currentTie.GetComponent<RopeCore>();
        if (tiedRopeCore.activeEntityJoint != null) //Destroy ties with entity first
            {GameObject.Destroy(tiedRopeCore.activeEntityJoint);}
        if (tiedRopeCore.connected)
        {
            //tiedRopeCore.transform.position
            owner.Knockback((currentTie.transform.position-owner.transform.position),400f);//change 400
        }
        GameObject.Destroy(currentTie); //This can't happen if it's other entities, check for RopeCores
    }
}
