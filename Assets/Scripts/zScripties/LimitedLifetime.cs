using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitedLifetime : MonoBehaviour
{
    public float totalLifeTime = 1f;
    float lifeCounter = 0f;

    void Update()
    {
        lifeCounter += Time.deltaTime;
        if (lifeCounter >= totalLifeTime)
        {
            Destroy(this.gameObject);
        }
    }
}
