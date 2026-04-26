using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesSystem : MonoBehaviour
{
    //Do particles...,
    //Model after explosion and entity summon system
    public Dictionary<string, GameObject> allParticles = new Dictionary<string, GameObject>();
    public List<GameObject> allParticleObjs;

    void Awake()
    {
        //allItemsAssign
        foreach (GameObject partcObj in allParticleObjs) {
            allParticles[partcObj.name] = partcObj;
        }
    }

    // THE SUMMONING MODEL IS LIKE SO: <Name> <Pos> <> 
    //Things needed so far: size, color, speed, lifetime
    public GameObject SummonParticle(string particleName, Vector3 summonPos)
    {
        GameObject summonedP = Instantiate(allParticles[particleName],summonPos,Quaternion.identity);
        //AssignSystemsToSummon(summonedP);
        return summonedP;
    }
    public GameObject SummonParticle(string particleName, Vector3 summonPos, float size, short count, Color color, float speed, float lifetime)
    {
        GameObject summonedP = Instantiate(allParticles[particleName],summonPos,Quaternion.identity);
        ParticleSystem ps = summonedP.GetComponent<ParticleSystem>();
        var pMain = ps.main;
        var pEmm = ps.emission;
            //setting speed, lifetime, color
            pMain.startSpeed = speed;
            pMain.startLifetime = lifetime;
            pMain.startColor = color;

            //setting count
            ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[pEmm.burstCount];
            pEmm.GetBursts(bursts);
            bursts[0].minCount = bursts[0].maxCount = count;
            pEmm.SetBursts(bursts);



        return summonedP;
    }
}
