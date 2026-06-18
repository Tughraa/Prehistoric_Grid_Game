using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateChase : IState
{
    public EntityGeneral toChase;
    public EnemyMovement enemyMovement;
    float pathUpdateTimer = 0f;
    float pathUpdatePeriod = 0.5f;
    float speedMult = 1.81f;
    float distanceThreshold = 14f;
    
    public string GetName => "chase";
    public bool IsFinished => false;
    public StateChase(EntityGeneral inToChase, EnemyMovement inEnemyMovement)
    {
        toChase = inToChase;
        enemyMovement = inEnemyMovement;
    }
    public void OnEnter(EntityGeneral entity)
    {
        entity.entityStatusEffects.speedMults.Add(speedMult);
        entity.GetComponent<EnemyGeneral>().meleeHitter = true;
    }
    public void Tick(EntityGeneral entity, float deltaTime)
    {
        pathUpdateTimer += deltaTime;
        if (pathUpdateTimer >= pathUpdatePeriod)
        {
            pathUpdateTimer = 0f;
            enemyMovement.GoToPoint(toChase.transform.position);
        }
        float distance = Vector2.Distance(toChase.transform.position,entity.transform.position);
        if (distance > distanceThreshold) //and last path empty
        {
            StateManager stateManag = entity.GetComponent<StateManager>();
            stateManag.StateTransition(new StateIdle(8f,enemyMovement));
        }
    }
    public void OnExit(EntityGeneral entity)
    {
        entity.entityStatusEffects.speedMults.Remove(speedMult);
        entity.GetComponent<EnemyGeneral>().meleeHitter = false;
    }
}
