using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyEffect : IStatusEffect
{
    public float effectDuration;
    public float effectStrength;

    public Color GetColor => new Color(0.2f,0.79f,0.89f,0.65f);
    public string GetMessage => ("You feel liberated from the ground");
    public string GetName => ("flying");
    public bool hasParticles => true;
    public bool IsFinished => (effectDuration <= 0f);
    public float remainingDuraton => effectDuration;
    
    public FlyEffect(float duration, float strength)
    {
        effectDuration = duration;
        effectStrength = strength;
    }
    public IStatusEffect Clone()
    {
        return new FlyEffect(effectDuration, effectStrength);
    }

    public void RefreshFrom(IStatusEffect other)
    {
        if (other is not FlyEffect incoming)
        {    
            return;
        }
        effectDuration = Mathf.Max(effectDuration, incoming.effectDuration);
        effectStrength = Mathf.Max(effectStrength, incoming.effectStrength);
    }

    public void OnApply(EntityGeneral entity) //works only for players, for now
    {
        entity.entityStatusEffects.gravityScaleMults.Add(0f);

        if (entity.entityType == "player")
        {
            entity.GetComponent<PlayerMovement>().flying = true;
        }
    }
    public void OnRemove(EntityGeneral entity)
    {
        entity.entityStatusEffects.gravityScaleMults.Remove(0f);

        if (entity.entityType == "player")
        {
            entity.GetComponent<PlayerMovement>().flying = false;
        }
    }
    public void Tick(EntityGeneral entity, float deltaTime)
    {
        effectDuration -= deltaTime;
    }
}
