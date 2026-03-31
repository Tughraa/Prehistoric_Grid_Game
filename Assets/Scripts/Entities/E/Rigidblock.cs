using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Rigidblock : MonoBehaviour
{
    public BlockState selfBlock;
    public Rigidbody2D rigid;

    public EntityGeneral entityGeneral;
    public Sprite[] breakLevels = new Sprite[5];
    public SpriteRenderer breakOverlay;

    bool constructed = false;
    [SerializeField] const float breakToHPmult = 3f;
    /*void Start()
    {
        entityGeneral = this.GetComponent<EntityGeneral>();
        AllSystems sy = entityGeneral.allSystems;
        BlockState defBlock = new BlockState(sy.blockLibrary.allBlocks["exampl"],sy.mapManager.FloatToGridPos(this.transform.position),sy.behaviourAdder,true);
        Construct(defBlock);
    }*/
    public void Construct(BlockState inState, Sprite blockSprite)
    {
        //Get The References
        entityGeneral = this.GetComponent<EntityGeneral>();
        rigid = this.GetComponent<Rigidbody2D>();
        selfBlock = inState;
        TileBaseToSprites(); //Get the breakLevel sprites from map manager
        breakOverlay = this.transform.GetChild(0).GetComponent<SpriteRenderer>();

        //Resprite
        //RuleTile tile = selfBlock.blockData.blockTile as RuleTile;
        this.GetComponent<SpriteRenderer>().sprite = blockSprite;//tile.m_DefaultSprite;
            //maybe change mass too
        //Change Health
        entityGeneral.maxHealth = selfBlock.brokenMax*breakToHPmult;
        entityGeneral.health = (selfBlock.brokenMax-selfBlock.brokenLevel)*breakToHPmult;
        //Flammability
        entityGeneral.flammable = selfBlock.blockData.tags.Contains("flammable");
            //Maybe add hasCollision too

        constructed = true;
    }
    void Update()
    {
        if (constructed)
        {
            UpdateBreakLevel();
        }
    }
    void UpdateBreakLevel()
    {
        float brokenPercentage = entityGeneral.health/entityGeneral.maxHealth;
        float divider = 1/breakLevels.Length;
        if (brokenPercentage <= divider)
        {
            breakOverlay.sprite = null;
        }
        else
        {
            int breakSpriteIndex = Mathf.FloorToInt((brokenPercentage-divider)*breakLevels.Length);
            if (breakSpriteIndex > breakLevels.Length-1)
            {
                Debug.Log("something's wrong, this block should've been dead by now");
                return;
            }
            breakOverlay.sprite = breakLevels[breakSpriteIndex];
        }
    }
    void TileBaseToSprites()
    {
        TileBase[] tiles = entityGeneral.allSystems.mapManager.breakLevelTiles;
        breakLevels = new Sprite[tiles.Length];
        int ite = tiles.Length-1;
        foreach(TileBase tileB in tiles)
        {
            RuleTile ruleTile = tileB as RuleTile;
            breakLevels[ite] = ruleTile.m_DefaultSprite;
            ite--;
        }
    }
}
