using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamGasBehaviour : IBlockBehaviour
{
    FireSystem fireSystem;
   
    public FlamGasBehaviour()
    {

    }
    public void OnPlaced(MapManager map, Vector3Int pos, BlockState state)
    {
        fireSystem = map.allSystems.fireSystem;
    }
    public void OnRemoved(MapManager map, Vector3Int pos, BlockState state)
    {

    }
    
    public void OnBreak(MapManager map, Vector3Int pos, BlockState state)
    {
        //Over here is where we go explodin!
        Ignite(map, pos);
    }
    public void Ignite(MapManager map, Vector3Int pos)
    {
        map.RemoveBlock(pos,false);
        fireSystem.SummonFireBurst(pos);
    }
    public void Tick(MapManager map, Vector3Int pos, BlockState state, float dt)
    {
        
    }
}
