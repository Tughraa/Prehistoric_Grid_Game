using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EntityGeneral : MonoBehaviour
{
    [Header("References")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Vector2 detectBoxSize = new Vector2(0.8f,0.05f);
    [SerializeField] float detectBoxDist = 0.5f;
    [SerializeField] Material defaultMaterial;
    [SerializeField] Material hurtMaterial;
    [SerializeField] Material interactMaterial;
    RaycastHit2D[] results;
    public Rigidbody2D rigid;
    public EntityStatusEffects entityStatusEffects;
    public MapManager mapManager;
    public AllSystems allSystems;
    public PlayerGeneral playerGeneral;

    [Header("EntityData")]
    public string entityName = "undef";
    public string entityType = "undef";

    public bool onGround;
    public float airTime = 0f;
    public float lastFallVel = 0f;
    public bool flammable = true;
    public bool burning = false;
    
    
    public bool immortal = false;
    public float maxHealth = 10f;
    public float health = 10f;
    public bool dead = false;
    public float iFrameTime = 0.05f;

    public float entityReach = 1f;
    
    /*
    public float entitySpeed = 1f;
    public float entityJumpBoost = 1f;
    public float entityKBsensitivity = 1f;
    public float entityStopDecel = 0f;
    */
    
    public float entityStopDecel = 0f;

    public List<string> tags;
    public List<string> damageImmunities;

    public float currentIFrames = 0f;

    public bool destroyOnDead = false;
    public bool interactable = false;
    public bool currentInteract = false; //implement it's usage

    public event Action<EntityGeneral,float,string> EntityDamaged;
    public event Action<EntityGeneral,float> EntityHealed;
    
    //[Header("New")]
    
    bool currentlyFalling = false;
    void Start()
    {
        rigid = this.GetComponent<Rigidbody2D>();
        entityStatusEffects = this.GetComponent<EntityStatusEffects>();
        mapManager = allSystems.mapManager;
    }

    void Update()
    {
        onGround = OnGround();
        AirTimeCalc();
        FallDamage(1.5f,2f);
        //Vector3 vel3 = rigid.velocity; //this is to see the velocity of each entity visually
        //Debug.DrawLine(this.transform.position, this.transform.position+vel3, Color.green, 0.5f);
        if (entityStopDecel > 0f)
        {
            entityStopDecel -= Time.deltaTime;
            if (rigid.velocity.magnitude < 0.1f)
            {
                entityStopDecel = 0f;
            }
        }
        if (currentIFrames > 0f)
        {
            currentIFrames -= Time.deltaTime;
            if (currentIFrames <= 0f)
            {
                ChangeMaterial(false); //is a bit short, maybe elongate it with a different timer
            }
        }
    }

    //Health
    public void Damage(float amount, string damageType)
    {
        if (damageImmunities.Contains(damageType) || currentIFrames > 0f || immortal)
        {
            //ShruggedOff(amount,damageType) for animatouns
            return;
        }
        ChangeMaterial(true);
        currentIFrames = iFrameTime;
        health -= amount;
        EntityDamaged?.Invoke(this, amount, damageType);
        if (health <= 0f)
        {
            Death();
            Debug.Log(entityName+" dies now");
        }
    }
    public void Death()
    {
        dead = true;
        health = 0f;
        this.GetComponent<SpriteRenderer>().color = Color.red; //This is temporary, will be removed
        if (destroyOnDead)
        {
            //Instantiate particles from entitysummoner
            Destroy(this.gameObject);
        }
    }
    public void Heal(float amount)
    {
        health = Mathf.Min(health+amount,maxHealth);
        EntityHealed?.Invoke(this, amount);
    }

    //Knockbacks are handled here
    public void Knockback(Vector2 knockVec,float knockAmount)
    {
        entityStopDecel = knockAmount/900;
        rigid.velocity = new Vector2(rigid.velocity.x,0f); //Cut the player y velocity
        rigid.AddForce(knockVec.normalized*(knockAmount*entityStatusEffects.GetEntityKBSensitivty()));
    }
    public void KnockbackAtPos(Vector2 knockVec, Vector2 knockPos,float knockAmount)
    {
        entityStopDecel = knockAmount/900;
        //Debug.Log(knockAmount);

        rigid.velocity = new Vector2(rigid.velocity.x,0f); //Cut the player y velocity
        rigid.AddForceAtPosition(knockVec.normalized*(knockAmount*entityStatusEffects.GetEntityKBSensitivty()),knockPos);
    }

    //OnGround
    public bool OnGround()
    {
        Vector2 originPos = this.transform.position;
        results = Physics2D.BoxCastAll(originPos,detectBoxSize,0f,new Vector3(0f,-entityReach,0f),detectBoxDist,groundLayer);
        if (results.Length > 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void AirTimeCalc()
    {
        //if (entityType == "player"){Debug.Log("airTime: "+airTime);}
        if (!onGround)
        {
            airTime += Time.deltaTime;
        }
        else
        {
            airTime = 0f;
        }
    }
    void FallDamage(float damageAmount, float damageCutoff)
    {
        //if (entityType == "player"){Debug.Log("fallTime: "+lastFallVel);}
        if (rigid.velocity.y < 0f)
        {
            lastFallVel = -rigid.velocity.y/10f;
            currentlyFalling = true;
        }
        else if (currentlyFalling)
        {
            float damageCalc = (lastFallVel*damageAmount);
            if (damageCalc > damageCutoff)
            {
                Damage(damageCalc,"fall");
            }
            currentlyFalling = false;
            lastFallVel = 0f;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(this.transform.position -transform.up*0.5f*entityReach, detectBoxSize); //GroundColDetect
    }
    public Vector3Int GetGridPos()
    {
        return new Vector3Int(Mathf.RoundToInt(this.transform.position.x),Mathf.RoundToInt(this.transform.position.y),0);
    }
    public void ChangeMaterial(bool hurt)
    {
        SpriteRenderer spir = this.GetComponent<SpriteRenderer>();
        if (hurt)
        {
            spir.material = hurtMaterial;
        }
        else if (currentInteract)
        {
            spir.material = interactMaterial;
        }
        else
        {
            spir.material = defaultMaterial;
        }
    }
}
