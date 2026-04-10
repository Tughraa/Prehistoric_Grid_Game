using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowBehaviour : IBlockBehaviour
{
    Vector2 originPos;

    float moveTimer = 0f;
    float movePeriod = 0.6f;
    int checkWide = 3;
   
    public FlowBehaviour(float period, int inCheckWide)
    {
        movePeriod = period;
        checkWide = inCheckWide;
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

        if (moveTimer > movePeriod) //For the gas to float up in a periodical time
        {
            moveTimer = 0f;
            FlowDownPosCalc(map, pos, checkWide);
        }
    }

    public void FlowDownPosCalc(MapManager map, Vector3Int pos, int wide)
    {
        Vector3Int downPos = pos+new Vector3Int(0,-1,0);
        if (BlockIsAvailable(map,downPos)) //if right underneath is empty, go there
        {
            map.MoveBlock(pos,downPos);
            return;
        }
        Debug.Log("i knew it wasnt empty");
        //Check Right And Left
        int rightCount = 1;
        int leftCount = 1;
        bool rightTurn = true; //Might leave to RNG in the future

        //while neither side is blocked AND neither side is fully checked
        while ((rightCount != 0 || leftCount != 0) && (rightCount <= wide || leftCount <= wide))
        {
            if (rightTurn) //currently looking at the right side
            {
                Vector3Int currentCheck = pos+new Vector3Int(rightCount,0,0);
                if (CheckAvailable(map, currentCheck, ref rightTurn, ref rightCount))
                {
                    Vector3Int oneRight = pos+new Vector3Int(1,0,0);
                    map.MoveBlock(pos, oneRight); //we go right
                    Debug.Log("going right! right???");
                    return;
                }
            }
            else
            {
                Vector3Int currentCheck = pos+new Vector3Int(-leftCount,0,0);
                if (CheckAvailable(map, currentCheck, ref rightTurn, ref leftCount))
                {
                    Vector3Int oneLeft = pos+new Vector3Int(-1,0,0);
                    map.MoveBlock(pos, oneLeft); //we go left
                    return;
                }
            }
            Debug.Log("blockage: "+(rightCount != 0 || leftCount != 0)+"\ncheck end: "+(rightCount <= wide || leftCount <= wide));
        }
    }
    public bool CheckAvailable(MapManager map, Vector3Int pos, ref bool rightTurn, ref int thisDir)
    {
        if (BlockIsAvailable(map,pos)) //the way we go is not blocked
        {
            if (BlockIsAvailable(map,pos+new Vector3Int(0,-1,0))) //There is salvation in this way, we shall go there
            {
                Debug.Log("case 0");
                return true;
            }
            else //It's not blocked but we cannot flow down, let's check the other direction
            {
                thisDir++;
                rightTurn = !rightTurn;
                Debug.Log("case 1");
                return false;
            }
        }
        else //the way is blocked, cancel this direction by making the value a 0
        {
            thisDir = 0;
            rightTurn = !rightTurn;
            Debug.Log("case 2");
            return false;
        }
    }
    bool BlockIsAvailable(MapManager map, Vector3Int pos)
    {
        return !map.HasBlock(pos);
    }
}

