using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSystem : MonoBehaviour
{
    public MapManager mapManager;
    public AllSystems allSystems;
    public GameObject expDustParticle;
    public GameObject expFlashParticle;
    public float explStr = 3f;
    Color debugColor;

    public void ExplodeSimple(Vector2 originPos, float strength, float delayTime, List<IStatusEffect> effectsToAdd)
    {
        //StartCoroutine(ExplodeDelay(originPos, strength, delayTime,effectsToAdd));
        ExplodePreciseDelay(36,originPos,strength,strength*100f,strength/4f,strength/1.2f,effectsToAdd,true,delayTime);
    }
    public void ExplodePreciseDelay(int angleDiv, Vector2 originPos, float explodeDist, float pushForce, float damageEntity,float damageBlock,List<IStatusEffect> effectsToAdd,bool doParticles, float delayTime)
    {
        StartCoroutine(ExplodeDelay(angleDiv, originPos, explodeDist, pushForce, damageEntity, damageBlock, effectsToAdd, doParticles, delayTime));
    }

    public void ExplodePrecise(int angleDiv, Vector2 originPos, float explodeDist, float pushForce, float damageEntity,float damageBlock,List<IStatusEffect> effectsToAdd, bool doParticles)
    {
        //These raycasts will hit entities and blocks
        for (int i = 0; i < angleDiv;i++)
        {
            debugColor = Color.red;
            MakeBreakRay(i*360f/angleDiv,originPos,explodeDist,pushForce,damageEntity,damageBlock,effectsToAdd);
        }
        if (doParticles)
        {
            //Do Particles here:
            Instantiate(expFlashParticle,originPos,Quaternion.identity); //Maybe take color from effectsToAdd too
            GameObject particleInst = Instantiate(expDustParticle,originPos,Quaternion.identity); //Maybe take color from effectsToAdd too
            ParticleSystem ps = particleInst.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule ma = ps.main;
            ma.startSpeed = explodeDist*4f;
            ma.startLifetime = explodeDist/20f;
        }


        ScreenShakeTrigger(originPos, explodeDist);
    }


    public void MakeBreakRay(float angle, Vector2 originPos, float explodeDist, float pushForce, float damageEntity,float damageBlock,List<IStatusEffect> effectsToAdd)
    {
        float rad = angle * Mathf.Deg2Rad;
        Vector2 currentRay = new Vector2(Mathf.Cos(rad),Mathf.Sin(rad)).normalized;

        //THIS PART IS FOR ENTITIES ONLY
        RaycastHit2D hit = Physics2D.Raycast(originPos, currentRay, explodeDist);
        Debug.DrawLine(originPos, originPos+(currentRay*explodeDist), debugColor,2f);
        if (hit)
        {
        Vector2 targetPos = hit.transform.position;
        float distanceFactor = explodeDist/Vector2.Distance(targetPos,originPos);
        if (hit.transform.gameObject.GetComponent<EntityGeneral>())
        {
            EntityGeneral hitEntity = hit.transform.gameObject.GetComponent<EntityGeneral>();

            hitEntity.Damage(damageEntity*distanceFactor,"explosion");
            hitEntity.KnockbackAtPos((targetPos-originPos).normalized,hit.point,pushForce);//Better to handle things here
            if (effectsToAdd != null)
            {
            for (int i = effectsToAdd.Count - 1; i >= 0; i--)
            {
                hitEntity.GetComponent<EntityStatusEffects>().AddEffect(effectsToAdd[i]);
            }
            }
            return; //The entity has protected the blocks behind it as well
        }
        }

        //THIS PART IS FOR BLOCKS ONLY
        float distanceTraveled = 0;
        float stepSize = 0.5f;

        while (distanceTraveled < explodeDist)
        {
            Vector2 currentPos = originPos + (currentRay * distanceTraveled);
            Vector3Int gridPos = mapManager.FloatToGridPos(currentPos);

            if (mapManager.HasBlock(gridPos))
            {
                // See if the block is special
                BlockState hitBlock = mapManager.GetBlock(gridPos);
                if (hitBlock.HasBehaviour<ExplosiveBehaviour>())
                {
                    Debug.Log("bomb rayed");
                    float powerOfExp = hitBlock.GetBehaviour<ExplosiveBehaviour>().currentPower;
                    ExplodeSimple(new Vector2(gridPos.x,gridPos.y),powerOfExp,0.15f,null);
                }
                if (hitBlock.HasBehaviour<FlamGasBehaviour>())
                {
                    mapManager.RemoveBlock(gridPos, false);
                    allSystems.fireSystem.SummonFireBurst(gridPos);
                }

                //Try to break the block
                bool broken = hitBlock.BlockBreak(damageBlock, mapManager);
            
                if (broken)
                {
                    //mapManager.RemoveBlock(gridPos, true); //we make sure here
                }
                else
                {
                    //unbroken block stops the ray
                    break; 
                }
            }

            distanceTraveled += stepSize;
        }
    }

    void ScreenShakeTrigger(Vector2 originPos, float explodeDist)
    {
        Collider2D[] results = Physics2D.OverlapCircleAll(originPos,explodeDist*10f);
        if (results.Length > 0)
        {
            foreach (var col in results)
            {    
                if (col.gameObject.GetComponent<PlayerGeneral>())
                {
                    col.gameObject.GetComponent<PlayerGeneral>().
                    playerCam.GetComponent<ScreenShake>().StartShaking(0.45f,0.45f);
                }
            }
        }
    }
    public void BlockPlaceExplosion(Vector3Int originPos, int explodeDist, BlockState blockToPlace)
    {
        StartCoroutine(BlockPlaceExplosionIE(originPos, explodeDist, blockToPlace));
    }
    IEnumerator ExplodeDelay(int angleDiv, Vector2 originPos, float explodeDist, float pushForce, float damageEntity,float damageBlock,List<IStatusEffect> effectsToAdd, bool doParticles, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        ExplodePrecise(angleDiv, originPos, explodeDist, pushForce, damageEntity, damageBlock, effectsToAdd,doParticles);
    }
    IEnumerator BlockPlaceExplosionIE(Vector3Int originPos, int explodeDist, BlockState blockToPlace)
    {
        int pOffs = Mathf.RoundToInt(explodeDist/2);
        for(int xi = originPos.x-pOffs; xi <= originPos.x+pOffs; xi++)
	    {
		    for(int yi = originPos.y-pOffs; yi <= originPos.y+pOffs; yi++)
	        {
                Vector3Int placePos = new Vector3Int(xi,yi,0);
                if (Vector3.Distance(originPos,placePos) <= pOffs)
                {
                    if (mapManager.HasBlock(placePos) == false)
                    {
                        //Debug.Log(placePos);
                        //Debug.Log(blockToPlace);
                        mapManager.PlaceBlockWithState(placePos,blockToPlace.Clone(placePos));
		                yield return new WaitForSeconds(0.02f);
                    }
                }
	        }
	    }
    }
}
