using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamGasBehaviour : IBlockBehaviour
{
    FireSystem fireSystem;
    ParticlesSystem particlesSystem;
    float particleTimer = 0f;
    float particlePeriod = 0.2f;
   
    public FlamGasBehaviour()
    {

    }
    public void OnPlaced(MapManager map, Vector3Int pos, BlockState state)
    {
        fireSystem = map.allSystems.fireSystem;
        particlesSystem = map.allSystems.particlesSystem;
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
        particleTimer += dt;
        if (particleTimer > particlePeriod)
        {
            particlesSystem.SummonParticle("flam_gas",pos);
            particleTimer = 0f;
        }
    }
}
