using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterEnem : MonoBehaviour
{
    public GameObject bulletFab;
    public GameObject tempPlayer;
    public EntityGeneral entityGeneral;
    public float bulletVel = 10f;
    public float tryShootPeriod = 1.2f;
    float shootTimer = 0f;
    void Start()
    {
        entityGeneral = this.GetComponent<EntityGeneral>();
    }
    void Update()
    {
        if (tempPlayer != null)
        {
            ShootClock();
        }
        else //try to find a player to shoot at
        {
            CircleSearchPlayer((Vector2)this.transform.position,20f);
        }
    }
    void ShootClock()
    {
        shootTimer += Time.deltaTime;
        if (shootTimer >= tryShootPeriod && !entityGeneral.dead)
        {
            ShootAt(tempPlayer.transform.position);
            shootTimer = 0f;
        }
    }
    public void ShootAt(Vector2 targetPos) //We have to adjust this so that we use the entity summoning system
    {   
        Vector2 throwVector = CalcThrowVector(this.transform.position,targetPos,bulletVel,bulletFab.GetComponent<Rigidbody2D>().gravityScale*Physics2D.gravity.y);
        if (throwVector != Vector2.zero)
        {
            GameObject instBullet = Instantiate(bulletFab,this.transform.position+new Vector3(0f,0.65f,0f),Quaternion.identity);
            Rigidbody2D bulRigid = instBullet.GetComponent<Rigidbody2D>();
            instBullet.GetComponent<Projectile>().owner = entityGeneral;
            instBullet.GetComponent<EntityGeneral>().allSystems = entityGeneral.allSystems;
            bulRigid.velocity = throwVector;
        }
        else
        {
            //cant do the shot
        }
    }

    public Vector2 CalcThrowVector(Vector2 originPos, Vector2 targetPos, float initVelocity, float gravity)
    {
        float xDist = targetPos.x - originPos.x;
        float yDist = targetPos.y - originPos.y;

        float v = initVelocity;
        float v2 = v * v;
        float g = Mathf.Abs(gravity);

        float discriminant = v2 * v2 - g * (g * xDist * xDist + 2f * yDist * v2);

        if (discriminant < 0f)
        {
            Debug.Log("can't do the shot");
            return Vector2.zero;
        }

        float sqrtD = Mathf.Sqrt(discriminant);

        float tanTheta = (v2 - sqrtD) / (g * Mathf.Abs(xDist));

        float theta = Mathf.Atan(tanTheta);

        return new Vector2(Mathf.Cos(theta)*Mathf.Sign(xDist), Mathf.Sin(theta)) * initVelocity;
    }
    void CircleSearchPlayer(Vector2 where, float searchRadius)
    {
        Collider2D[] results = Physics2D.OverlapCircleAll(where,searchRadius);
        
        Debug.DrawLine(where- new Vector2(0f,searchRadius), where+ new Vector2(0f,searchRadius), Color.green,2f);
        Debug.DrawLine(where- new Vector2(searchRadius,0f), where+ new Vector2(searchRadius,0f), Color.green,2f);

        if (results.Length > 0)
        {
            foreach (Collider2D col in results)
            {
                if (col.gameObject.GetComponent<EntityGeneral>())
                {
                    if (col.gameObject.GetComponent<EntityGeneral>().entityType == "player") //we only look for players for now
                    {
                        tempPlayer = col.gameObject;
                    }
                }
            }
        }
    }
}
