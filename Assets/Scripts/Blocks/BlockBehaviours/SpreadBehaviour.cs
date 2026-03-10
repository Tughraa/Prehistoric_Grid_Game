using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadBehaviour : IBlockBehaviour
{
    Vector2 originPos;

    float moveTimer = 0f;
    float movePeriod = 2f;

    bool doesSpread = true;

    float spreadChance = 0.4f;

    int spreadRange = 1;
   
    public SpreadBehaviour(bool spreading, float inSpreadChance, float inMovePeriod, int inSpreadRange)
    {
        doesSpread = spreading;
        spreadChance = inSpreadChance;
        movePeriod = inMovePeriod;
        spreadRange = inSpreadRange;
    }
    public void OnPlaced(MapManager map, Vector3Int pos, BlockState state)
    {
        moveTimer = 0f;
    }
    public void OnRemoved(MapManager map, Vector3Int pos, BlockState state)
    {

    }
    public void OnBreak(MapManager map, Vector3Int pos, BlockState state)
    {
        
    }
    public void Tick(MapManager map, Vector3Int pos, BlockState state, float dt)
    {
        moveTimer += dt;
        
        if (moveTimer > movePeriod) //For the flames to spread
        {
            moveTimer = 0f;
            SpreadBlock(map, pos, state);
        }
    }
    
    public void SpreadBlock(MapManager map, Vector3Int pos, BlockState state)
    {
        for (int ix = pos.x-spreadRange; ix <= pos.x+spreadRange;ix++)
        {
            for (int iy = pos.y-spreadRange-1; iy <= pos.y+spreadRange-1;iy++)
            {
                Vector3Int placeToCheck = new Vector3Int(ix,iy,pos.z);
                if (SuitableForFlame(map,placeToCheck))
                {
                    //Leave this to chance:
                    System.Random blockSpreadRNG = map.allSystems.randomSystem.blockSpreadRNG;
                    float chanceCalc = (float)blockSpreadRNG.NextDouble();
                    Debug.Log(chanceCalc+" is the chance for it to spread");
                    if (chanceCalc > spreadChance)
                    {
                        return;
                    }
                    map.PlaceBlock(placeToCheck+new Vector3Int(0,1,0),state.blockData);
                }
            }
        }
    }
    public bool SuitableForFlame(MapManager map, Vector3Int pos)
    {
        if (map.HasBlock(pos) == false)
        {
            return false;
        }
        BlockState blockToCheck = map.GetBlock(pos);
        if (blockToCheck.blockData.tags.Contains("flammable")) //is this block flammable at all
        {
            if (map.HasBlock(pos+new Vector3Int(0,1,0)) == false)
            {
                return true;
            }
            else
            {
                return false; //This may be a good oppurtunity to check for breaklevel and just replace w/ fire, or idk
            }
        }
        else 
        {
            return false;
        }
    }
}
