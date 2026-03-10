using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBehaviour : IBlockBehaviour
{
    Vector2 originPos;
    public float currentPower = 1f;
    ExplosionSystem explosionSystem;
    List<IStatusEffect> effectsToAdd;
   
    public ExplosiveBehaviour(float explodePower, List<IStatusEffect> inEffectsToAdd)
    {
        currentPower = explodePower;
        effectsToAdd = inEffectsToAdd;
    }
    public void OnPlaced(MapManager map, Vector3Int pos, BlockState state)
    {
        
    }
    public void OnRemoved(MapManager map, Vector3Int pos, BlockState state)
    {

    }
    
    public void OnBreak(MapManager map, Vector3Int pos, BlockState state)
    {
        explosionSystem = map.allSystems.explosionSystem;
        //Over here is where we go explodin!
        if (state.brokenLevel /state.brokenMax >= 0.5f) //if it's below half the brokenness
        {
            Explode(map, pos);
        }
    }
    public void Explode(MapManager map, Vector3Int pos)
    {
        explosionSystem = map.allSystems.explosionSystem;
        map.RemoveBlock(pos,false);
        Vector2 vec2pos = new Vector2(pos.x,pos.y);
        explosionSystem.ExplodeSimple(vec2pos,currentPower,0.05f,effectsToAdd);
    }
    public void Tick(MapManager map, Vector3Int pos, BlockState state, float dt)
    {
        
    }
}
