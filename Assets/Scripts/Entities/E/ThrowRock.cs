using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowRock : MonoBehaviour
{
    EntityGeneral rockEntity;
    public float lifeTime = 25f;
    public EntityGeneral owner;
    float currentLife = 0f;
    
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
}
