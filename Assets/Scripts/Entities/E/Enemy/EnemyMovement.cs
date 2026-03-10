using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 1f;
    public float jumpForce = 400f;
    public EnemyGeneral enemyGeneral;
    public AllSystems allSystems;

    public bool flying = false;

    public GameObject targetObj;
    public Vector2 targetPos;
    public float minPathLength = 1.5f;

    public Seeker seeker;
    float timer = 0f;
    public float pathPeriod = 0.5f;
    Path path;
    int currentWaypoint = 0;

    void Start()
    {
        seeker = this.GetComponent<Seeker>();
        enemyGeneral = this.GetComponent<EnemyGeneral>();
        allSystems = enemyGeneral.entityGeneral.allSystems;
    }
    void OnPathComplete(Path p)
    {
        if (p.error) return;

        path = p;
        currentWaypoint = 0;
    }
    public void CalculatePath()
    {
        seeker.StartPath(this.transform.position, targetPos, OnPathComplete);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= pathPeriod)
        {
            targetPos = targetObj.transform.position;
            CalculatePath();
            timer = 0f;
        }
        FollowPath();
    }
    void FollowPath()
    {
        if (path == null) //no path to follow
        {return;}
        if (path.GetTotalLength() < minPathLength) //path is too short to bother
        {return;}
        if (currentWaypoint >= path.vectorPath.Count) //already at destination
        {return;}
        if (enemyGeneral.entityGeneral.entityStopDecel > 0f) //stunned
        {return;}
        if (enemyGeneral.entityGeneral.dead) //dead
        {return;}

        Vector2 dir = ((Vector2)path.vectorPath[currentWaypoint] - (Vector2)transform.position).normalized;
        GetComponent<Rigidbody2D>().velocity = dir * speed*enemyGeneral.entityGeneral.entityStatusEffects.GetEntitySpeed();

        float dist = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);
        if (dist < 0.1f)
        {currentWaypoint++;}
    }
}
