using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasDiffusionBehaviour : IBlockBehaviour
{
    Vector2 originPos;

    float totalLifetime = 10f;
    float timer = 0f;
    float moveTimer = 0f;
    float movePeriod = 0.6f;
   
    public GasDiffusionBehaviour(float inLifetime, float period)
    {
        totalLifetime = inLifetime;
        movePeriod = period;
    }
    public void OnPlaced(MapManager map, Vector3Int pos, BlockState state)
    {
        moveTimer = 0f;
        timer = 0f;
    }
    public void OnRemoved(MapManager map, Vector3Int pos, BlockState state)
    {

    }
    public void OnBreak(MapManager map, Vector3Int pos, BlockState state)
    {

    }
    public void Tick(MapManager map, Vector3Int pos, BlockState state, float dt)
    {
        timer += dt;
        moveTimer += dt;

        if (moveTimer > movePeriod) //For the gas to float up in a periodical time
        {
            moveTimer = 0f;
            FloatUpPosCalc(map, pos);
        }
        if (timer > totalLifetime) //For the gas to die off after it's lifetime
        {
            Debug.Log(timer + " timer has ran out. For: "+state.blockData.blockName);
            map.RemoveBlock(pos, false); //MAYBE COME BACK AND MAKE THE "destroy" TRUE
        }
    }

    public void FloatUpPosCalc(MapManager map, Vector3Int pos)
    {
        Vector3Int upPos = pos+new Vector3Int(0,1,0);
        Vector3Int lUpPos = pos+new Vector3Int(-1,1,0);
        Vector3Int rUpPos = pos+new Vector3Int(1,1,0);
        if (map.HasBlock(upPos) == false) //Check the block up
        {
            //Debug.Log("only way is up");
            map.MoveBlock(pos,upPos);
            return;
        }
        else if (map.HasBlock(lUpPos) == false) //Check the block left up
        {
            map.MoveBlock(pos,lUpPos);
            return;
        }
        else if (map.HasBlock(rUpPos) == false) //Check the block right up
        {
            map.MoveBlock(pos,rUpPos);
            return;
        }
        else
        {
            return; //No Move required
        }
    } 
}
