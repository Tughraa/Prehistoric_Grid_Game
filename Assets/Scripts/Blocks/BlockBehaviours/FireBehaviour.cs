using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBehaviour : IBlockBehaviour
{
    Vector2 originPos;

    float totalLifetime = 10f;
    float timer = 0f;

    float currentStrength = 1f;
   
    public FireBehaviour(float fireStrength, float inLifetime)
    {
        currentStrength = fireStrength;
        totalLifetime = inLifetime;
    }
    public void OnPlaced(MapManager map, Vector3Int pos, BlockState state)
    {
        timer = 0f;
    }
    public void OnRemoved(MapManager map, Vector3Int pos, BlockState state)
    {

    }
    public void OnBreak(MapManager map, Vector3Int pos, BlockState state)
    {
        //Smoke particles or so??
    }
    public void Tick(MapManager map, Vector3Int pos, BlockState state, float dt)
    {
        timer += dt;
        map.allSystems.fireSystem.BurnEntities(pos,0.9f); //For the flame to apply it's effect to entities on contanct

        if (timer > totalLifetime) //For the gas to die off after it's lifetime
        {
            map.RemoveBlock(pos, false); //MAYBE COME BACK AND MAKE THE "destroy" TRUE
        }
    }
}
