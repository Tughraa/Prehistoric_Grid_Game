using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableItemBehaviour : IItemBehaviour
{
    public GameObject fabToThrow;
    public float throwDist;
    public float throwForce;

    public BlockData blockBombBlock;

    public ThrowableItemBehaviour(GameObject throwObject,float inThrowDist, float inThrowForce)
    {
        fabToThrow = throwObject;
        throwDist = inThrowDist;
        throwForce = inThrowForce;
    }

    public void Use(EntityGeneral owner, ItemState state, Vector3 mousePos, float heldFor, Inventory inventory, int slot)
    {
        float currentThrowForce = throwForce/2f + throwForce*(Mathf.Clamp(heldFor,0f,0.9f)); //make that 0.9 changable plz
        
        Vector3 throwOrigin = owner.transform.position+(mousePos-owner.transform.position).normalized*throwDist;
        GameObject summoned = state.allSystems.entitySummonSystem.SummonEntityFab(fabToThrow,throwOrigin);
        AssignToThrownObj(owner,summoned);

        Vector2 throwDir = (mousePos-throwOrigin).normalized;
        EntityGeneral sumonEnt = summoned.GetComponent<EntityGeneral>();
        sumonEnt.rigid.velocity = owner.rigid.velocity;
        sumonEnt.Knockback(throwDir,currentThrowForce);

        if (sumonEnt.GetComponent<ItemPickUp>())
        {
            sumonEnt.GetComponent<ItemPickUp>().storedItem = state.Clone(owner);
        }
        state.GetBehaviour<ConsumableItemBehaviour>().ItemUsed(inventory,slot);
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
    void AssignToThrownObj(EntityGeneral owner, GameObject obj)
    {
        if (obj.GetComponent<ThrowRock>())
        {
            obj.GetComponent<ThrowRock>().owner = owner;
        }
        if (obj.GetComponent<BlockBomb>())
        {
            obj.GetComponent<BlockBomb>().owner = owner;
            if (blockBombBlock != null)
            {obj.GetComponent<BlockBomb>().containedBlock = blockBombBlock;}
            //and also the gas!
        }
    }
}
