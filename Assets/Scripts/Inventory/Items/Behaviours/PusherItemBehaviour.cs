using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PusherItemBehaviour : IItemBehaviour
{
    float reachDist;
    float pushForce;
    public PusherItemBehaviour(float inReachDist, float inPushForce)
    {
        reachDist = inReachDist;
        pushForce = inPushForce;
    }

    public void Use(EntityGeneral owner, ItemState state, Vector3 mousePos, float heldFor, Inventory inventory, int slot)
    {
        Vector3 pushDir = (mousePos-owner.transform.position).normalized;
        Vector3 detectOrigin = owner.transform.position+pushDir*owner.entityReach*0.5f;
        
        RaycastHit2D hit = Physics2D.Raycast(detectOrigin, pushDir, reachDist);
        Debug.DrawLine(detectOrigin, detectOrigin+pushDir*reachDist, Color.green,2f);

        if (hit.collider == null)
        {
            //uhmm, we didn't hit anything
            Debug.Log("nothing");
            return;
        }
        if (hit.collider.GetComponent<EntityGeneral>())
        {
            Debug.Log("entity");
            EntityGeneral hitEntity = hit.collider.GetComponent<EntityGeneral>();
            float totalMass = hitEntity.rigid.mass + owner.rigid.mass;
            owner.Knockback(-pushDir,pushForce*(hitEntity.rigid.mass/totalMass));
            hitEntity.Knockback(pushDir,pushForce*(owner.rigid.mass/totalMass));
        }
        else
        {
            owner.Knockback(-pushDir,pushForce);
            Debug.Log("block");
        }
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
