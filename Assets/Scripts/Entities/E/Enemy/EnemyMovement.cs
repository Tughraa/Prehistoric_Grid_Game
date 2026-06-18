using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 1f;
    public float jumpForce = 400f;
    public EnemyGeneral enemyGeneral;
    public AllSystems allSystems;

    public bool flying = false;
    public bool moving = false;

    public float minPathLength = 1.5f;

    public Seeker seeker;
    float timer = 0f;
    public float pathPeriod = 0.5f;
    Path path;
    int currentWaypoint = 0;
    public event Action<EnemyMovement,Vector2> GoalGiven;
    public event Action<EnemyMovement> GoalReached;
    
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
    public void GoToPoint(Vector2 point)
    {
        seeker.StartPath(this.transform.position, point, OnPathComplete);
        GoalGiven?.Invoke(this,point);
        moving = true;
    }

    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetKeyDown(KeyCode.X))
        {
            GoToPoint(mousePos);
        }
        if (moving || true)
        {
            FollowPath();
        }
    }
    public void PathsFinished()
    {
        if (!moving)
        {return;}
        
        moving = false;
        GoalReached?.Invoke(this);
    }
    public bool IsPathNull()
    {
        return (path == null) || (currentWaypoint >= path.vectorPath.Count) || (path.GetTotalLength() < minPathLength);
    }
    public Vector2 GetDirection()
    {
        return ((Vector2)path.vectorPath[currentWaypoint] - (Vector2)transform.position).normalized;
    }
    void FollowPath()
    {
        if (path == null) //no path to follow
        {PathsFinished();return;}
        if (path.GetTotalLength() < minPathLength) //path is too short to bother
        {PathsFinished();return;}
        if (currentWaypoint >= path.vectorPath.Count) //already at destination
        {PathsFinished();return;}
        if (enemyGeneral.entityGeneral.entityStopDecel > 0f) //stunned
        {return;}
        if (enemyGeneral.entityGeneral.dead) //dead
        {return;}

        Vector2 dir = GetDirection();
        GetComponent<Rigidbody2D>().velocity = dir * speed*enemyGeneral.entityGeneral.entityStatusEffects.GetEntitySpeed();

        float dist = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);
        if (dist < 0.1f)
        {currentWaypoint++;}
    }
}
