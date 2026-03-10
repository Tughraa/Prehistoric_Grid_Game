using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    
    
    public EntityGeneral owner;
    public float lifeTime = 25f;
    float currentLife = 0f;
    public float damage = 1f;

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
            StartCoroutine(SendToDestroy(2f));
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<EntityGeneral>())
        {
            EntityGeneral hitEntity = col.gameObject.GetComponent<EntityGeneral>();
            //we assume it has a rigibody, in future we might want to change it tho
            if (hitEntity == owner)
            {
                return; //Don't hit the owner ofc
            }
            hitEntity.KnockbackAtPos(col.contacts[0].normal, col.contacts[0].point, 300f);
            hitEntity.Damage(damage,"smashing");
            StartCoroutine(SendToDestroy(0.2f));
            //We may want to add effect transfering further on:
            /*if (rockEntity.entityStatusEffects.HasAnyEffect())
            {
                rockEntity.entityStatusEffects.TransferEffects(hitEntity.entityStatusEffects);
            }*/
        }
        else
        {
            //check if block to destroy
        }
        StartCoroutine(SendToDestroy(2f));
    }
    IEnumerator SendToDestroy(float inSeconds)
    {
        SpriteRenderer sprite = this.GetComponent<SpriteRenderer>();
        float currentTime = inSeconds;
        while (currentTime > 0f)
        {
            sprite.color = new Color(sprite.color.r,sprite.color.g,sprite.color.b,currentTime/inSeconds);
            yield return null;
            currentTime -= Time.deltaTime;
        }
        Destroy(this.gameObject);
    }
}
