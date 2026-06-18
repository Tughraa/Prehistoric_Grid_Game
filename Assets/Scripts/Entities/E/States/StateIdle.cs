using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateIdle : IState
{
    float wanderDist;
    public EnemyMovement enemyMovement;
    float wanderPeriod = 5f;
    float timer = 0f;
    public float lookAngle = 90f;

    public string GetName => "idle";
    public bool IsFinished => false;
    public StateIdle(float inWanderDist, EnemyMovement inEnemyMovement)
    {
        wanderDist = inWanderDist;
        enemyMovement = inEnemyMovement;
    }
    public void OnEnter(EntityGeneral entity)
    {

    }
    public void Tick(EntityGeneral entity, float deltaTime)
    {
        timer += deltaTime;
        if (timer > wanderPeriod)
        {
            timer = 0f;
            //Make the random position deterministic
            //Check the random position's availability
            Vector3 randomPOI = RandomAvailablePos(entity.transform.position,entity.allSystems);
            enemyMovement.GoToPoint(randomPOI);
        }
        bool lookLeft = entity.GetComponent<FlyEnemy>().lookLeft; //VERY TEMPORARY, REFACTOR
        lookAngle = lookLeft ? 90f : -90f;
        GameObject ray = RaycastSight(entity.transform.position+new Vector3(0.5f*Mathf.Sign(lookAngle),0f,0f),lookAngle,90f,8,5.4f);
        if (ray != null)
        {
            Debug.Log("playerFound");
            EntityGeneral playerEntity = ray.GetComponent<EntityGeneral>();
            entity.GetComponent<StateManager>().StateTransition(new StateChase(playerEntity,enemyMovement));
        }
    }
    public void OnExit(EntityGeneral entity)
    {

    }
    public Vector2 RandomAvailablePos(Vector2 origin, AllSystems allSystems) //MAKE DETERMINISTIC
    {
        int attempts = 30;
        Vector2 randomPos = RandomPosAroundPoint(origin, Random.Range(wanderDist/2f,wanderDist));
        bool posAvailability = IsPointAvailable(randomPos,allSystems.mapManager);
        while (attempts > 0 && posAvailability == false)
        {   //Repeating isnt the most efficient thing, could be fixed later
            attempts--;
            randomPos = RandomPosAroundPoint(origin, Random.Range(wanderDist/2f,wanderDist));
            posAvailability = IsPointAvailable(randomPos,allSystems.mapManager);
        }
        Debug.Log((30-attempts)+" many attempts");
        return randomPos; 
    }
    Vector2 RandomPosAroundPoint(Vector2 point, float distance)
    {
        float angle = Random.Range(0f,359f);
        Vector2 randomVec = new Vector2(Mathf.Cos(angle),Mathf.Sin(angle));
        return point + randomVec*distance;
    }
    bool IsPointAvailable(Vector2 point, MapManager map)
    {
        /*Debug.DrawLine(point + new Vector2(0.5f, 0.5f), point + new Vector2(-0.5f, -0.5f), Color.red, 2f);
        return !map.HasBlock(map.FloatToGridPos(point));
        */
        RaycastHit2D posCheck = Physics2D.Raycast(point, -Vector2.up);
        Debug.DrawLine(point - new Vector2(0.5f, 0.5f), point + new Vector2(-0.5f, -0.5f), Color.red, 2f);
        if (posCheck)
        {return false;}
        return true;
    }
    public GameObject RaycastSight(Vector3 lineOrigin,float angleOrigin, float angleWidth, int accuracy, float maxDist)
    {
        string allAngles = "";
        for (int i = 0; i <= accuracy; i++)
        {
            float currentAngle = angleOrigin-(angleWidth/2f)+(i*angleWidth/accuracy);
            float currentRadian = currentAngle * Mathf.Deg2Rad;
            Vector2 sightLine = new Vector2(Mathf.Sin(currentRadian),Mathf.Cos(currentRadian));
            allAngles += sightLine+" ";
            RaycastHit2D raycast = Physics2D.Raycast(lineOrigin, sightLine, maxDist);
            Vector2 lineOriginVec2 = lineOrigin;
            Debug.DrawLine(lineOrigin, raycast.point, new Color(0.8f,1f,0.3f,1f), 0.01f);
            if (raycast)
            {
                if (raycast.collider.gameObject.GetComponent<EntityGeneral>())
                {
                    if (raycast.collider.gameObject.GetComponent<EntityGeneral>().entityType == "player")
                    {   
                        return raycast.collider.gameObject;
                    }
                }
            }
        }
        //Debug.Log(allAngles);
        return null;
    }
}
