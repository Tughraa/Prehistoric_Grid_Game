using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableItemBehaviour : IItemBehaviour
{
    public GameObject fabToThrow;
    public float throwDist;
    public float throwForce;

    public BlockData blockBombBlock;
    public bool throwOnUse = true;

    public ThrowableItemBehaviour(GameObject throwObject,float inThrowDist, float inThrowForce)
    {
        fabToThrow = throwObject;
        throwDist = inThrowDist;
        throwForce = inThrowForce;
    }

    public void Use(EntityGeneral owner, ItemState state, Vector3 mousePos, float heldFor, Inventory inventory, int slot)
    {
        if (throwOnUse)
        {
            Vector3 originCheckPos = owner.transform.position+(mousePos-owner.transform.position).normalized*throwDist;
            if (PointHasCollision(originCheckPos) && state.HasBehaviour<RockBreakBehaviour>())
            {
                state.GetBehaviour<RockBreakBehaviour>().CantThrow(owner,originCheckPos);
                return;
            }
            ThrowItem(owner, mousePos, heldFor, state);
            if (state.HasBehaviour<ConsumableItemBehaviour>())
            {
                state.GetBehaviour<ConsumableItemBehaviour>().ItemUsed(owner,state,inventory,slot);
            }
        }
        else
        {
            Debug.Log("not throw on use");
        }
    }
    public GameObject ThrowItem(EntityGeneral owner, Vector3 mousePos, float heldFor, ItemState state)
    {
        float currentThrowForce = throwForce/2f + throwForce*(Mathf.Clamp(heldFor,0f,0.9f));
        Vector3 throwOrigin = owner.transform.position+(mousePos-owner.transform.position).normalized*throwDist;    
        GameObject summoned = state.allSystems.entitySummonSystem.SummonEntityFab(fabToThrow,throwOrigin);
        
        Vector2 throwDir = (mousePos-throwOrigin).normalized;
        AssignToThrownObj(owner,summoned,throwDir*currentThrowForce);

        EntityGeneral sumonEnt = summoned.GetComponent<EntityGeneral>();
        sumonEnt.rigid.velocity = owner.rigid.velocity;
        sumonEnt.Knockback(throwDir,currentThrowForce);
        owner.Knockback(-throwDir,currentThrowForce/2f);

        if (sumonEnt.GetComponent<ItemPickUp>())
        {
            sumonEnt.GetComponent<ItemPickUp>().storedItem = state.Clone(owner);
        }
        return summoned;
    }
    public void RightClick(EntityGeneral owner, ItemState state, Vector3 mousePos, float heldFor, Inventory inventory, int slot)
    {
        Vector3Int mouseGrid = (owner.mapManager.FloatToGridPos(mousePos));
        if (owner.mapManager.HasBlock(mouseGrid))
        {
            blockBombBlock = owner.mapManager.GetBlock(mouseGrid).blockData;
        }
    }
    public void StartHolding(EntityGeneral owner, ItemState state, Inventory inventory, int slot)
    {
        //Debug.Log("started holding: "+state.itemData.id);
    }
    public void StopHolding(EntityGeneral owner, ItemState state, Inventory inventory, int slot)
    {
        
        //Debug.Log("stopped holding: "+state.itemData.id);
    }
    void AssignToThrownObj(EntityGeneral owner, GameObject obj, Vector2 throwVec)
    {
        if (obj.GetComponent<ThrowRock>())
        {
            obj.GetComponent<ThrowRock>().owner = owner;
        }
        if (obj.GetComponent<RopeCore>())
        {
            obj.GetComponent<RopeCore>().owner = owner;
            obj.GetComponent<RopeCore>().initThrow = throwVec;
        }
        if (obj.GetComponent<BlockBomb>())
        {
            obj.GetComponent<BlockBomb>().owner = owner;
            if (blockBombBlock != null)
            {obj.GetComponent<BlockBomb>().containedBlock = blockBombBlock;}
            //and also the gas!
        }
    }
    bool PointHasCollision(Vector2 position)
    {
        return Physics2D.OverlapPoint(position) != null;
    }
}
