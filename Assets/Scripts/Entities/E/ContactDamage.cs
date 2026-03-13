using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    EntityGeneral entityGen;
    public EntityGeneral owner;
    //float colDotThresh = 0.01f;
    public float impulseThresh = 4f;
    public float damage = 0.19f;
    public float blockDamage = 0.15f;
    int currentCols = 0;
    
    void Start()
    {
        entityGen = this.GetComponent<EntityGeneral>();
    }
    
    void OnCollisionEnter2D(Collision2D col)
    {
        //Debug.Log(col.gameObject.name+"is stoned"+" with velocity:"+entityGen.rigid.velocity.magnitude);
        currentCols++;
        float impulse = col.relativeVelocity.magnitude * entityGen.rigid.mass;
        Vector2 rockVel = entityGen.rigid.velocity;
        Vector2 targetVel = Vector2.zero;
        if (col.gameObject.GetComponent<EntityGeneral>())
        {
            EntityGeneral hitEntity = col.gameObject.GetComponent<EntityGeneral>();
            //we assume it has a rigibody, in future we might want to change it tho

            Debug.Log("impulse is: "+impulse+"\ncollisions: "+currentCols);
            //instead of checking for the collisions, try on ground of some sorts?
            if (impulse >impulseThresh && currentCols == 1) 
            {
                hitEntity.KnockbackAtPos(col.contacts[0].normal, col.contacts[0].point, 300f);
                hitEntity.Damage(damage*impulse,"smashing");
                //Also apply my status effects to what'ive hit
                if (entityGen.entityStatusEffects.HasAnyEffect())
                {
                    entityGen.entityStatusEffects.TransferEffects(hitEntity.entityStatusEffects);
                }
            }
        }
        else //not an entity? let's see if it's a block
        {
            Vector3 checkPos = this.transform.position +(Vector3)col.relativeVelocity.normalized*-entityGen.entityReach;
            Debug.DrawLine(this.transform.position, checkPos, Color.blue,2f);
            MapManager map = entityGen.allSystems.mapManager;
            Vector3Int checkPosGrid = map.FloatToGridPos(checkPos);
            if (map.HasBlock(checkPosGrid))
            {
                BlockState hitBlock = map.GetBlock(checkPosGrid);
                hitBlock.BlockBreak(blockDamage*impulse,map);
            }
        }
    }
    void OnCollisionExit2D(Collision2D col)
    {
        currentCols--;
    }
}
