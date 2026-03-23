using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PusherItemBehaviour : IItemBehaviour
{
    float reachDist;
    float pushForce;
    float blockDamage;
    float entityDamage;
    public PusherItemBehaviour(float inReachDist, float inPushForce, float inBlockDamage, float inEntityDamage)
    {
        reachDist = inReachDist;
        pushForce = inPushForce;
        blockDamage = inBlockDamage;
        entityDamage = inEntityDamage;
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
            return;
        }
        if (hit.collider.GetComponent<EntityGeneral>()) //entity pushed
        {
            EntityGeneral hitEntity = hit.collider.GetComponent<EntityGeneral>();
            float totalMass = hitEntity.rigid.mass + owner.rigid.mass;
            owner.Knockback(-pushDir,currentPushForce*(hitEntity.rigid.mass/totalMass)*1.6f);
            hitEntity.Knockback(pushDir,currentPushForce*(owner.rigid.mass/totalMass)*1.6f);
            hitEntity.Damage(entityDamage*chargeMult,"poking");
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
