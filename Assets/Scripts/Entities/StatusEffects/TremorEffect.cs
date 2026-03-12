using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TremorEffect : IStatusEffect
{
    public float effectDuration;
    public float effectStrength;

    public Color GetColor => new Color(0.75f,0.45f,0.28f,0.85f);
    public string GetMessage => ("You are violently shaking!");
    public string GetName => ("tremor");
    public bool hasParticles => true;
    public bool IsFinished => (effectDuration <= 0f);
    public float remainingDuraton => effectDuration;

    public float applyBreakPeriod = 0.75f;
    float breakTimer = 0f;
    
    public TremorEffect(float duration, float strength, float period)
    {
        effectDuration = duration;
        effectStrength = strength;
        applyBreakPeriod = period;
    }
    
    public IStatusEffect Clone()
    {
        return new TremorEffect(effectDuration, effectStrength, applyBreakPeriod);
    }

    public void RefreshFrom(IStatusEffect other)
    {
        if (other is not TremorEffect incoming)
        {    
            return;
        }
        effectDuration = Mathf.Max(effectDuration, incoming.effectDuration);
        effectStrength = Mathf.Max(effectStrength, incoming.effectStrength);
        applyBreakPeriod = Mathf.Min(applyBreakPeriod, incoming.applyBreakPeriod);
    }

    public void OnApply(EntityGeneral entity)
    {
        entity.burning = true;
    }
    public void OnRemove(EntityGeneral entity)
    {
        entity.burning = false;
    }
    public void Tick(EntityGeneral entity, float deltaTime)
    {
        breakTimer += deltaTime;
        //add screen shake if player
        if (entity.entityType == "player")
        {
            entity.GetComponent<PlayerGeneral>().playerCam.GetComponent<ScreenShake>().StartShaking(0.05f,0.2f);
        }
        if (breakTimer >= applyBreakPeriod)
        {
            breakTimer = 0f;
            //entity.Damage(effectStrength,"fire"); //maybe add damage
            BreakTouchedBlocks(entity.mapManager, entity.transform.position+new Vector3(0.6f,0f,0f));
            BreakTouchedBlocks(entity.mapManager, entity.transform.position+new Vector3(-0.6f,0f,0f));
            BreakTouchedBlocks(entity.mapManager, entity.transform.position+new Vector3(0f,0.6f,0f));
            BreakTouchedBlocks(entity.mapManager, entity.transform.position+new Vector3(0f,-0.6f,0f));

            //throw player around
            //entity.Knockback(Random.insideUnitSphere,effectStrength*10f); //change the randomization to be deterministic
        }
        effectDuration -= deltaTime;
    }
    void BreakTouchedBlocks(MapManager mapManager, Vector3 position) //Add chance to this, make it work with explosion system breakRay
    {
        Vector3Int intPos = mapManager.FloatToGridPos(position);
        if (mapManager.HasBlock(intPos))
        {
            BlockState blockAtHand = mapManager.GetBlock(intPos);
            blockAtHand.BlockBreak(effectStrength,mapManager);
        }
    }
}
