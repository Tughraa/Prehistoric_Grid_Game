using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBomb : MonoBehaviour
{
    EntityGeneral bBombEntity;
    public float lifeTime = 25f;
    public EntityGeneral owner;
    float currentLife = 0f;
    float colVelThresh = 3f;

    public BlockData containedBlock;
    
    void Start()
    {
        bBombEntity = this.GetComponent<EntityGeneral>();
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
            TriggerBomb();
        }
    }
    public void TriggerBomb()
    {
        Vector3Int thrownPos = bBombEntity.GetGridPos();
        BlockState blockToPlace = new BlockState(containedBlock,thrownPos,bBombEntity.allSystems.behaviourAdder,true);
        bBombEntity.allSystems.explosionSystem.BlockPlaceExplosion(thrownPos, 6, blockToPlace);
        Destroy(this.gameObject);
    }
    
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.relativeVelocity.magnitude >= colVelThresh)
        {
            TriggerBomb();
        }
    }
}
