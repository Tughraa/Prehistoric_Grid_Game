using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EntityStatusEffects : MonoBehaviour
{
    List<IStatusEffect> effects = new();

    public GameObject generalEffectParticle;
    float particleTimer = 0f;
    [SerializeField] float particleTimePeriod = 0.5f;
    public GameObject burnEffectParticle;

    public EntityGeneral entityGeneral;
    public Rigidbody2D rigid;

    [SerializeField] float speedBase = 1f;
    public List<float> speedMults = new();
    [SerializeField] float jumpForceBase = 1f;
    public List<float> jumpForceMults = new();
    [SerializeField] float kbSensBase = 1f; //Shorthand for "knockback sensitivity"
    public List<float> kbSensMults = new();
    [SerializeField] float gravityScaleBase = 1f;
    public List<float> gravityScaleMults = new();

    void Start()
    {
        entityGeneral = this.GetComponent<EntityGeneral>();
        rigid = this.GetComponent<Rigidbody2D>();
        this.gravityScaleBase = rigid.gravityScale;
    }

    void Update()
    {
        EffectTickHandler();
        if (effects.Count > 0)
        {   EffectFX(); }

        rigid.gravityScale = GetEntityGravityScale();

        //These ones are just for testing
        if (Input.GetKeyDown(KeyCode.P) && entityGeneral.entityType == "player")
        {
            this.AddEffect(new SpeedEffect(duration: 10f,strength: 3f));
        }
        if (Input.GetKeyDown(KeyCode.L) && entityGeneral.entityType == "player")
        {
            this.AddEffect(new BurnEffect(duration: 5f,strength: 0.5f,period: 0.75f));
        }
        if (Input.GetKeyDown(KeyCode.K) && entityGeneral.entityType == "player")
        {
            this.AddEffect(new JumpBoostEffect(duration: 5f,strength: 2f));
        }
        if (Input.GetKeyDown(KeyCode.O) && entityGeneral.entityType == "player")
        {
            this.AddEffect(new HealingEffect(duration: 5f,strength: 0.5f,period: 0.75f));
        }
        if (Input.GetKeyDown(KeyCode.Y) && entityGeneral.entityType == "player")
        {
            this.AddEffect(new FloatEffect(duration: 5f,strength: 0.125f));
        }
        if (Input.GetKeyDown(KeyCode.M) && entityGeneral.entityType == "player")
        {
            this.AddEffect(new TremorEffect(duration: 10f,strength: 1.5f,period: 0.3f));
        }
    }

    //Add-Remove Methods
    public void AddEffect(IStatusEffect effect)
    {
        if (HasEffect(effect.GetType()))
        {
            foreach (var effecti in effects)
            {
                if (effecti.GetType() == effect.GetType())
                {    
                    effecti.RefreshFrom(effect);
                    return;
                }
            }
        }
        //Add specifier here
        if (entityGeneral.entityType == "player")
        {
            entityGeneral.GetComponent<PlayerGeneral>().Announce(effect.GetMessage,3.5f,effect.GetColor);
        }
        effect.OnApply(entityGeneral);
        effects.Add(effect);
    }
    public void RemoveEffect(Type effectType)
    {
        for (int i = effects.Count - 1; i >= 0; i--)
        {
            if (effects[i].GetType() == effectType)
            {
                effects[i].OnRemove(this.entityGeneral);
                effects.RemoveAt(i);
            }
        }
    }
    public void TransferEffects(EntityStatusEffects to)
    {
        for (int i = effects.Count - 1; i >= 0; i--)
        {
            to.AddEffect(effects[i].Clone());
            //effects.RemoveAt(i); //maybe bool check this
        }
    }

    //Queries
    public bool HasEffect(Type effectType)
    {
        foreach (var effecti in effects)
        {
            if (effecti.GetType() == effectType)
            {    
                return true;
            }
        }
        return false;
    }
    public bool HasAnyEffect()
    {
        if (effects.Count > 0)
        {
            return true;
        }
        return false;
    }

    //Ticking Methods
    void EffectTickHandler()
    {
        float dt = Time.deltaTime;

        for (int i = effects.Count - 1; i >= 0; i--)
        {
            effects[i].Tick(entityGeneral, dt);

            if (effects[i].IsFinished)
            {
                effects[i].OnRemove(entityGeneral);
                effects.RemoveAt(i);
            }
        }
    }
    void EffectFX()
    {
        particleTimer += Time.deltaTime;
        if (particleTimer > particleTimePeriod)
        {
            particleTimer = 0f;
            GameObject particleInstance;
            foreach (var effecti in effects)
            {
                if (HasEffect(effecti.GetType()) && effecti.hasParticles)
                {
                    particleInstance = Instantiate(generalEffectParticle, this.transform.position,Quaternion.identity);
                    var mainPS = particleInstance.GetComponent<ParticleSystem>().main;
                    mainPS.startColor = effecti.GetColor;
                }
                if (entityGeneral.burning && effecti.GetType() == typeof(BurnEffect))
                {
                    Instantiate(burnEffectParticle, this.transform.position,Quaternion.identity);
                }
            }
        }
    }

    //Scaling Entity Stats
    public float GetEntitySpeed()
    {
        float currentSpeed = speedBase;
        foreach(float mult in speedMults)
        {
            currentSpeed *= mult;
        }
        return currentSpeed;
    }
    public float GetEntityJumpBoost()
    {
        float currentJump = jumpForceBase;
        foreach(float mult in jumpForceMults)
        {
            currentJump *= mult;
        }
        return currentJump;
    }
    public float GetEntityKBSensitivty() //knockback sensitivity
    {
        float currentKBSens = kbSensBase;
        foreach(float mult in kbSensMults)
        {
            currentKBSens *= mult;
        }
        return currentKBSens;
    }
    public float GetEntityGravityScale()
    {
        float currentGrav = gravityScaleBase;
        foreach(float mult in gravityScaleMults)
        {
            currentGrav *= mult;
        }
        return currentGrav;
    }
}
