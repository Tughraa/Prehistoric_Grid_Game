using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PusherItemBehaviour : IItemBehaviour
{
    float reachDist;
    float pushForce;
    float blockDamage;
    public PusherItemBehaviour(float inReachDist, float inPushForce, float inBlockDamage)
    {
        reachDist = inReachDist;
        pushForce = inPushForce;
        blockDamage = inBlockDamage;
    }

    public void Use(EntityGeneral owner, ItemState state, Vector3 mousePos, float heldFor, Inventory inventory, int slot)
    {
        Vector3 pushDir = (mousePos-owner.transform.position).normalized;
        Vector3 detectOrigin = owner.transform.position+pushDir*owner.entityReach*0.5f;
        float chargeMult = Mathf.Clamp(heldFor,0f,0.9f);
        float currentPushForce = pushForce*chargeMult;
        
        RaycastHit2D hit = Physics2D.Raycast(detectOrigin, pushDir, reachDist);
        Debug.DrawLine(detectOrigin, detectOrigin+pushDir*reachDist, Color.green,2f);

        if (hit.collider == null)
        {
            //uhmm, we didn't hit anything
            Debug.Log("nothing");
            return;
        }
        if (hit.collider.GetComponent<EntityGeneral>()) //entity pushed
        {
            Debug.Log("entity");
            EntityGeneral hitEntity = hit.collider.GetComponent<EntityGeneral>();
            float totalMass = hitEntity.rigid.mass + owner.rigid.mass;
            owner.Knockback(-pushDir,currentPushForce*(hitEntity.rigid.mass/totalMass));
            hitEntity.Knockback(pushDir,currentPushForce*(owner.rigid.mass/totalMass));
        }
        else //non-entity pushed, see if block
        {
            Vector3 checkPos = detectOrigin+pushDir*reachDist*0.65f; //BLOCK BREAKING
            MapManager map = owner.allSystems.mapManager;
            Vector3Int checkPosGrid = map.FloatToGridPos(checkPos);
            if (map.HasBlock(checkPosGrid))
            {
                BlockState hitBlock = map.GetBlock(checkPosGrid);
                hitBlock.BlockBreak(blockDamage*chargeMult,map);
            }
            owner.Knockback(-pushDir,currentPushForce);
            Debug.Log("block");
        }
        state.GetBehaviour<DurabilityBehaviour>().ItemUsed(owner,state,inventory,slot,chargeMult);
    }
    public void RightClick(EntityGeneral owner, ItemState state, Vector3 mousePos, float heldFor, Inventory inventory, int slot)
    {

    }
    public void StartHolding(EntityGeneral owner, ItemState state, Inventory inventory, int slot)
    {
        //Debug.Log("started holding: "+state.itemData.id);
    }
    public void StopHolding(EntityGeneral owner, ItemState state, Inventory inventory, int slot)
    {
        
        //Debug.Log("stopped holding: "+state.itemData.id);
    }
}
