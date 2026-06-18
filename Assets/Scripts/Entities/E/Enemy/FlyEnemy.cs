using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FlyEnemy : MonoBehaviour
{
    StateManager stateManager;
    EntityGeneral entity;
    Vector3 flyStillPoint;
    [SerializeField] EnemyMovement enemyMovement; 
    public bool lookLeft = true;

    //Move this part to a seperate class to reuse
    [Header("Fly Skeleton")]
    [SerializeField] GameObject body;
    [SerializeField] Transform head;
    [SerializeField] Transform tail;
    [SerializeField] Transform wingR;
    [SerializeField] Transform wingL;

    float wingRestR;
    float wingRestL;
    float animTimer = 0f;

    void Start()
    {
        stateManager = this.GetComponent<StateManager>();
        entity = this.GetComponent<EntityGeneral>();
        enemyMovement = this.GetComponent<EnemyMovement>();
        stateManager.InitState(new StateIdle(8f,enemyMovement));
        wingRestR = wingR.eulerAngles.z;
        wingRestL = wingL.eulerAngles.z;
    }
    void Update()
    {
        if (!enemyMovement.moving)
        {
            RunFlyStill();
        }
        else
        {
            RunMoving();
        }
        if (entity.dead == false) //WingAnimations
        {
            animTimer += Time.deltaTime*16f;
            wingR.eulerAngles = new Vector3(0f,wingR.eulerAngles.y,wingRestR+Mathf.Sin(animTimer)*30f);
            wingL.eulerAngles = new Vector3(0f,wingL.eulerAngles.y,wingRestL+Mathf.Sin(animTimer+3f)*30f);
        }
    }
    void GoalGiven(EnemyMovement enemyMovement, Vector2 pos)
    {
        StopFlyStill();
    }
    void GoalReached(EnemyMovement enemyMovement)
    {
        StartFlyStill();
    }
    void StartFlyStill()
    {
        entity.rigid.velocity = Vector2.zero;
        flyStillPoint = this.transform.position;
        entity.entityStatusEffects.gravityScaleMults.Add(0f);
    }
    void RunFlyStill()
    {
        //entity.rigid.AddForce
        if (entity.entityStopDecel <= 0f)
        {
            entity.rigid.velocity = Vector2.zero;
        }
    }
    void StopFlyStill()
    {
        flyStillPoint = this.transform.position;
        entity.entityStatusEffects.gravityScaleMults.Remove(0f);
    }
    void RunMoving()
    {
        if (!enemyMovement.IsPathNull())
        {
            lookLeft = (enemyMovement.GetDirection().x > 0);
            float yRot = lookLeft ? 180f : 0f;
            body.transform.eulerAngles = new Vector3(0f,yRot,body.transform.eulerAngles.z);
        }
    }
    
    void OnEnable()
    {
        enemyMovement.GoalGiven += GoalGiven; //Subscribe and start to listen
        enemyMovement.GoalReached += GoalReached;
    }
    void OnDisable()
    {
        enemyMovement.GoalGiven -= GoalGiven; //Subscribe and start to listen
        enemyMovement.GoalReached -= GoalReached;
    }
}
