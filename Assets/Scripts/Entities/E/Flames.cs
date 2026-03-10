using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flames : MonoBehaviour
{
    public AllSystems allSystems;
    FireSystem fireSystem;
    public float tickPeriod = 1.3f;
    float tickTimer = 0f;
    public Sprite[] sprites;
    int currentSpriteIndex = 0; 
    SpriteRenderer spriteRend;
    float lifeTimeCounter = 0f;
    float lifeTime = 6f;
    void Start()
    {
        fireSystem = allSystems.fireSystem;
        spriteRend = this.GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        Vector3Int thisGridPos = allSystems.mapManager.FloatToGridPos(this.transform.position);
        tickTimer += Time.deltaTime;
        lifeTimeCounter += Time.deltaTime;
        if (tickTimer > tickPeriod)
        {
            Tick();
            StepSprites();
            tickTimer = 0f;
        }
        if (lifeTimeCounter > lifeTime || !allSystems.mapManager.HasBlock(thisGridPos))
        {
            DestroyFlames();
        }
        fireSystem.BurnEntities(thisGridPos,1.1f);
    }
    void StepSprites()
    {
        currentSpriteIndex++;
        if (currentSpriteIndex >= sprites.Length)
        {
            currentSpriteIndex = 0;
        }
        spriteRend.sprite = sprites[currentSpriteIndex];
    } 

    public void Tick()
    {
        FireTouchedBlocks((Vector2)this.transform.position+new Vector2(0,0),0.9f);
        FireTouchedBlocks((Vector2)this.transform.position+new Vector2(1,0),0.4f);
        FireTouchedBlocks((Vector2)this.transform.position+new Vector2(-1,0),0.4f);
        FireTouchedBlocks((Vector2)this.transform.position+new Vector2(0,1),0.4f);
        FireTouchedBlocks((Vector2)this.transform.position+new Vector2(0,-1),0.4f);
    }
    void FireTouchedBlocks(Vector2 originPos, float fireChance)
    {
        //only do so if it's a certain amount of chance
        System.Random blockSpreadRNG = allSystems.randomSystem.blockSpreadRNG;
        float chanceCalc = (float)blockSpreadRNG.NextDouble();
        if (chanceCalc > fireChance)
        {return;}
        Vector3Int gridPos = new Vector3Int(Mathf.RoundToInt(originPos.x),Mathf.RoundToInt(originPos.y),0);
        fireSystem.FireUpBlock(gridPos);
    }
    public void DestroyFlames()
    {
        Vector3Int gridPos = allSystems.mapManager.FloatToGridPos(this.transform.position);
        if (allSystems.mapManager.HasBlock(gridPos))
        {
            System.Random blockSpreadRNG = allSystems.randomSystem.blockSpreadRNG; //chance its not destroyed
            float chanceCalc = (float)blockSpreadRNG.NextDouble();
            if (chanceCalc > 0.35f)
            {allSystems.mapManager.RemoveBlock(gridPos,true);}
        }
        fireSystem.currentFlames.Remove(gridPos);
        Destroy(this.gameObject);
    }
}
