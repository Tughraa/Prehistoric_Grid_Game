using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidBehaviour : IBlockBehaviour
{
    IStatusEffect currentEffect;
    Color currentColor;
    Vector2 originPos;
   
    public LiquidBehaviour(IStatusEffect effect)
    {
        currentEffect = effect;
        if (effect != null)
        {currentColor = effect.GetColor;}
        else
        {currentColor = new Color(0.1f,0.7f,0.95f,0.3f);}
    }
    public void OnPlaced(MapManager map, Vector3Int pos, BlockState state)
    {
        Color transColor = new Color(currentColor.r,currentColor.g,currentColor.b,0.3f);
        map.blockLayer.SetColor(pos,currentColor);
    }
    public void OnRemoved(MapManager map, Vector3Int pos, BlockState state)
    {
        //Evaporate(map, pos, false); //OnRemoved is having a problem rn
        /*if (map.HasBlock(pos))
        {
            if (map.GetBlock(pos) == state)
            {
                Evaporate(map, pos, false);
            }
        }*/
        Debug.Log("water removed at:"+pos);
    }
    public void OnBreak(MapManager map, Vector3Int pos, BlockState state)
    {
        Evaporate(map, pos, true);
    }
    public void Tick(MapManager map, Vector3Int pos, BlockState state, float dt)
    {
        if (currentEffect != null)
        {
            ApplyLiquidEffect(pos); //For the gas to apply it's effect to entities on contanct
        }
    }
    public void ApplyLiquidEffect(Vector3Int pos)
    {
        originPos = new Vector2(pos.x,pos.y);
        Collider2D[] results = Physics2D.OverlapBoxAll(originPos,new Vector2(0.9f, 0.9f),0f);
        
        
        Debug.DrawLine(originPos+new Vector2(-0.5f,-0.5f), originPos+new Vector2(0.5f,0.5f), Color.green);
        if (results.Length > 0)
        {
            foreach (var col in results)
            {    
                TryApplyEffect(col.gameObject);
            }
        }
    }
    public void TryApplyEffect(GameObject objToTry)
    {
        if (objToTry.GetComponent<EntityStatusEffects>())
        {
            IStatusEffect effectCopy = currentEffect.Clone();
            objToTry.GetComponent<EntityStatusEffects>().AddEffect(effectCopy);
        }
    }

    public void Evaporate(MapManager map, Vector3Int pos, bool removeBlock)
    {
        BlockState evaporGas = new BlockState(map.allSystems.blockLibrary.allBlocks["effect_gas"],pos,map.allSystems.behaviourAdder,false);
        if (currentEffect != null)
        {
            IStatusEffect effectCopy = currentEffect.Clone();
            evaporGas.AddBehaviour(new GasDiffusionBehaviour(22f,0.6f));
            evaporGas.AddBehaviour(new EffectGasBehaviour(effectCopy));//constructing this gas ourselves may not be the best idea
        }
        if (removeBlock && map.HasBlock(pos))
        {
            map.RemoveBlock(pos,true);
        }
        map.PlaceBlockWithState(pos,evaporGas);
    }
}
