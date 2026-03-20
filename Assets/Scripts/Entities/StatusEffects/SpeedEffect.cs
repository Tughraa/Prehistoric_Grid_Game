using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedEffect : IStatusEffect
{
    public float effectDuration;
    public float effectStrength;

    public string GetMessage => ("You feel the urge to run");
    public string GetName => ("speed");
    public Color GetColor => new Color(0.02f,0.6f,0.8f,0.85f);
    public bool hasParticles => true;
    public bool IsFinished => (effectDuration <= 0f);
    public float remainingDuraton => effectDuration;
    
    public SpeedEffect(float duration, float strength)
    {
        effectDuration = duration;
        effectStrength = strength;
    }
    public IStatusEffect Clone()
    {
        return new SpeedEffect(effectDuration, effectStrength);
    }

    public void RefreshFrom(IStatusEffect other)
    {
        if (other is not SpeedEffect incoming)
        {    
            return;
        }
        effectDuration = Mathf.Max(effectDuration, incoming.effectDuration);
        effectStrength = Mathf.Max(effectStrength, incoming.effectStrength);
    }

    public void OnApply(EntityGeneral entity)
    {
        entity.rigid.velocity *= 1.5f; //get a little faster when you get the effect

        entity.entityStatusEffects.speedMults.Add(effectStrength);
    }
    public void OnRemove(EntityGeneral entity)
    {
        entity.rigid.velocity = new Vector2(entity.rigid.velocity.x/2f,entity.rigid.velocity.y);

        entity.entityStatusEffects.speedMults.Remove(effectStrength);
        effectDuration = -2f;
    }
    public void Tick(EntityGeneral entity, float deltaTime)
    {
        effectDuration -= deltaTime;
    }
}
