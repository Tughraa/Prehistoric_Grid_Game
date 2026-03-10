using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnEffect : IStatusEffect
{
    public float effectDuration;
    public float effectStrength;

    public Color GetColor => Color.red;
    public string GetMessage => ("You are burning!");
    public bool hasParticles => false;
    public bool IsFinished => (effectDuration <= 0f);

    public float applyDamagePeriod = 0.75f;
    float damageTimer = 0f;
    
    public BurnEffect(float duration, float strength, float period)
    {
        effectDuration = duration;
        effectStrength = strength;
        applyDamagePeriod = period;
    }
    
    public IStatusEffect Clone()
    {
        return new BurnEffect(effectDuration, effectStrength, applyDamagePeriod);
    }

    public void RefreshFrom(IStatusEffect other)
    {
        if (other is not BurnEffect incoming)
        {    
            return;
        }
        effectDuration = Mathf.Max(effectDuration, incoming.effectDuration);
        effectStrength = Mathf.Max(effectStrength, incoming.effectStrength);
        applyDamagePeriod = Mathf.Min(applyDamagePeriod, incoming.applyDamagePeriod);
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
        damageTimer += deltaTime;
        if (damageTimer >= applyDamagePeriod)
        {
            damageTimer = 0f;
            entity.Damage(effectStrength,"fire");
        }
        effectDuration -= deltaTime;

        SpreadToEntities(entity.transform.position,1.2f,entity.gameObject); //Adjust the size based on entitySize
        FireTouchedBlocks(entity.mapManager.allSystems.fireSystem, entity.transform.position);
        //If we want to fire up touched blocks:
        {
            FireTouchedBlocks(entity.mapManager.allSystems.fireSystem, entity.transform.position+new Vector3(0.5f,0f,0f));
            FireTouchedBlocks(entity.mapManager.allSystems.fireSystem, entity.transform.position+new Vector3(-0.5f,0f,0f));
            FireTouchedBlocks(entity.mapManager.allSystems.fireSystem, entity.transform.position+new Vector3(0f,0.5f,0f));
            FireTouchedBlocks(entity.mapManager.allSystems.fireSystem, entity.transform.position+new Vector3(0f,-0.5f,0f));
        }
    }
    void SpreadToEntities(Vector2 originPos,float spreadSize,GameObject selfEntity)
    {
        Collider2D[] results = Physics2D.OverlapCircleAll(originPos,spreadSize);
        
        Debug.DrawLine(originPos- new Vector2(0f,spreadSize/2f), originPos+ new Vector2(0f,spreadSize/2f), Color.magenta,2f);
        Debug.DrawLine(originPos- new Vector2(spreadSize/2f,0f), originPos+ new Vector2(spreadSize/2f,0f), Color.magenta,2f);

        if (results.Length > 0)
        {
            foreach (var col in results)
            {    
                if (col.gameObject.GetComponent<EntityStatusEffects>())
                {
                    EntityStatusEffects entitySE = col.gameObject.GetComponent<EntityStatusEffects>();
                    if (col.gameObject.GetComponent<EntityGeneral>().flammable == false
                        || col.gameObject == selfEntity
                        || entitySE.HasEffect(this.GetType()))
                    {
                        continue;
                    }
                    entitySE.AddEffect(new BurnEffect(3f,effectStrength,applyDamagePeriod));
                }
            }
        }
    }
    void FireTouchedBlocks(FireSystem fireSystem, Vector2 originPos)
    {
        Vector3Int gridPos = new Vector3Int(Mathf.RoundToInt(originPos.x),Mathf.RoundToInt(originPos.y),0);
        fireSystem.FireUpBlock(gridPos);
    }
}
