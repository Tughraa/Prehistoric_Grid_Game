using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyGeneral : MonoBehaviour
{
    public EntityGeneral entityGeneral;
    public bool meleeHitter = false;
    public float meleeRange = 2.5f;
    public float meleeDamage = 0.1f;
    [SerializeField] Image enemyHealthBar;
    
    void Start()
    {
        entityGeneral = this.GetComponent<EntityGeneral>();
    }
    void Update()
    {
        if (meleeHitter && !entityGeneral.dead)
        {
            MeleeAttackDetection(); //this is all very simple, next up lets use items to keep it systemic
        }        
    }

    //Health
    void OnEnable()
    {
        entityGeneral.EntityDamaged += GetHurt; //Subscribe and start to listen
        entityGeneral.EntityHealed += GetHeal;
    }
    void OnDisable()
    {
        entityGeneral.EntityDamaged -= GetHurt; //stop listening
        entityGeneral.EntityHealed -= GetHeal;
    }
    public void GetHurt(EntityGeneral entity, float amount, string damageType)
    {UpdateHealthBar();}
    public void GetHeal(EntityGeneral entity, float amount)
    {UpdateHealthBar();}
    public void UpdateHealthBar()
    {
        enemyHealthBar.fillAmount = entityGeneral.health/entityGeneral.maxHealth;
    }

    //Melee attacks
    private void MeleeAttackDetection()
    {
        Vector2 origin = transform.position;

        // Overlap circle: detect anything inside melee range
        Collider2D[] results = Physics2D.OverlapCircleAll(origin, meleeRange);

        // Debug visualization
        Debug.DrawLine(origin - new Vector2(0f, meleeRange), origin + new Vector2(0f, meleeRange), Color.red, 2f);
        Debug.DrawLine(origin - new Vector2(meleeRange, 0f), origin + new Vector2(meleeRange, 0f), Color.red, 2f);

        foreach (var col in results)
        {
            PlayerGeneral player = col.GetComponent<PlayerGeneral>();
            if (player == null)
            {continue;}

            // Now we check LOS using a raycast
            Vector2 targetPos = col.transform.position;
            Vector2 direction = targetPos - origin;

            RaycastHit2D hit = Physics2D.Raycast(origin+direction.normalized*0.5f, direction.normalized, direction.magnitude);

            if (hit.collider == null)
            {continue;}

            // Is the first hit actually the player?
            if (hit.collider.gameObject == col.gameObject)
            {
                player.GetComponent<EntityGeneral>().Damage(meleeDamage, "enemy");
            }
        }
    }
}
