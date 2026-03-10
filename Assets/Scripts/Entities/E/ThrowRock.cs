using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowRock : MonoBehaviour
{
    EntityGeneral rockEntity;
    public float lifeTime = 25f;
    public EntityGeneral owner;
    float currentLife = 0f;
    //float colDotThresh = 0.01f;
    public float impulseThresh = 4f;
    public float rockDamage = 0.5f;
    public float rockBlockBreak = 0.15f;
    int currentCols = 0;

    public BlockData tempB;
    
    void Start()
    {
        rockEntity = this.GetComponent<EntityGeneral>();
    }
    void Update()
    {
        LifeTimeManag();
    }

    void LifeTimeManag()
    {
        currentLife += Time.deltaTime;
        if (currentLife > lifeTime)
        {   
            //Maybe particle?
            Destroy(this.gameObject);
        }
    }
    
    void OnCollisionEnter2D(Collision2D col)
    {
        //Debug.Log(col.gameObject.name+"is stoned"+" with velocity:"+rockEntity.rigid.velocity.magnitude);
        currentCols++;
        float impulse = col.relativeVelocity.magnitude * rockEntity.rigid.mass;
        Vector2 rockVel = rockEntity.rigid.velocity;
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
                hitEntity.Damage(rockDamage*impulse,"smashing");
                //Also apply my status effects to what'ive hit
                if (rockEntity.entityStatusEffects.HasAnyEffect())
                {
                    rockEntity.entityStatusEffects.TransferEffects(hitEntity.entityStatusEffects);
                }
            }
        }
        else //not an entity? let's see if it's a block
        {
            Vector3 checkPos = this.transform.position +(Vector3)col.relativeVelocity.normalized/-2f;
            Debug.DrawLine(this.transform.position, checkPos, Color.blue,2f);
            MapManager map = rockEntity.allSystems.mapManager;
            Vector3Int checkPosGrid = map.FloatToGridPos(checkPos);
            if (map.HasBlock(checkPosGrid))
            {
                BlockState hitBlock = map.GetBlock(checkPosGrid);
                hitBlock.BlockBreak(rockBlockBreak*impulse,map);
            }
        }
    }
    void OnCollisionExit2D(Collision2D col)
    {
        currentCols--;
    }
}
