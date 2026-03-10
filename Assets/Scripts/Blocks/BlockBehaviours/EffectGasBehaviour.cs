using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectGasBehaviour : IBlockBehaviour
{
    IStatusEffect currentEffect;
    Color currentColor;
    Vector2 originPos;
   
    public EffectGasBehaviour(IStatusEffect effect)
    {
        currentEffect = effect;
        currentColor = effect.GetColor;
    }
    public void OnPlaced(MapManager map, Vector3Int pos, BlockState state)
    {
        map.blockLayer.SetColor(pos,currentColor);
    }
    public void OnRemoved(MapManager map, Vector3Int pos, BlockState state)
    {

    }
    public void OnBreak(MapManager map, Vector3Int pos, BlockState state)
    {

    }
    public void Tick(MapManager map, Vector3Int pos, BlockState state, float dt)
    {
        ApplyGasEffect(pos); //For the gas to apply it's effect to entities on contanct
    }
    public void ApplyGasEffect(Vector3Int pos)
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
}
