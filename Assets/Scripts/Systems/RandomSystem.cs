using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomSystem : MonoBehaviour
{
    public string seed;
    public bool randomizeSeedOnStart;
    public AllSystems allSystems;
    int worldSeed = 123456;
    public System.Random terrainGenRNG;
    public System.Random lootRNG;
    public System.Random blockSpreadRNG;
    public System.Random worldPlacementRNG;
    public System.Random effectRNG;

    public PlayerGeneral tempPlayer;
    void Awake()
    {
        if (randomizeSeedOnStart)
        {
            seed = RandomString(7);
        }
        tempPlayer.Announce("Current Seed: "+ seed,8f,new Color(0.6f,0.76f,0.84f,0.9f));
        worldSeed = SeedFromString(seed);
        terrainGenRNG = new System.Random(worldSeed);
        lootRNG = new System.Random(worldSeed);// * 23384); //make some multiplications
        blockSpreadRNG = new System.Random(worldSeed);
        worldPlacementRNG = new System.Random(worldSeed);
        effectRNG = new System.Random(worldSeed);
    }
    int SeedFromString(string s)
    {
        int hash = 23;
        foreach (char c in s)
        {
            hash = hash * 31 + c;
        }
        return hash;
    }
    string RandomString(int size)
    {
        string glyphs= "ABCDEFGHIJKLMNOPQRSTUVWXYZ";//0123456789";
        string randomString = "";
        for(int i=0; i<size; i++)
        {
            randomString += glyphs[Random.Range(0, glyphs.Length)];
        }
        return randomString;
    }
    
}
