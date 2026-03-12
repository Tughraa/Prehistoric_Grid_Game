using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldMaker : MonoBehaviour
{
    public int AmountOfShooters = 0;
    public MapManager mapManager;
    public AllSystems allSystems;
    public RandomSystem randomSystem;
    public BlockData defaultBlock;
    public Vector2Int xLimit;
    public Vector2Int yLimit;
    public float perlinThreshold = 0.4f;
    public float perlinMultX = 0.1f;
    public float perlinMultY = 0.2f;
    public float perlinConst = 100f;

    [SerializeField] BlockData tempFlamGas;
    void Start()
    {
        randomSystem = allSystems.randomSystem;
        perlinConst *= (float)randomSystem.terrainGenRNG.NextDouble()*2036f;
        //MapClear();
        mapManager.ReadMapBlocks();
        PutBlocks(xLimit,yLimit);
        StartCoroutine(PlaceContent(0.5f));
        StartCoroutine(ScanTheWorld(0.7f));
    }
    void MapClear()
    {
        mapManager.blockLayer.ClearAllTiles();
    }
    void PutBlocks(Vector2Int xRange, Vector2Int yRange)
    {
        for (int ix = xLimit.x; ix <= xLimit.y; ix++)
        {
            for (int iy = yLimit.x; iy <= yLimit.y; iy++)
            {
                //Debug.Log(ix+" "+iy+" "+ Mathf.PerlinNoise((float)ix,(float)iy));
                if (PerlinEval(ix,iy) && mapManager.HasBlock(new Vector3Int(ix,iy,0)) == false)
                {
                    mapManager.PlaceBlock(new Vector3Int(ix,iy,0),defaultBlock);
                }
            }
        }
    }
    bool PerlinEval(float xP, float yP)
    {
        float perlinVal = Mathf.PerlinNoise(xP*perlinMultX+perlinConst,yP*perlinMultY+perlinConst);
        //Debug.Log(perlinVal);
        if (perlinVal >= perlinThreshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //Placing other other things
    void PlaceMapThings()
    {
        PlaceVines(75);
        PlaceBlocksRandomly("explosive",40);
        PlaceEntitiesRandomly("chest",50); //it was 50 before
        PlaceEntitiesRandomly("shooter_enemy",AmountOfShooters);
        PlaceBlockClumpsRandomly(7); //normally 7
        allSystems.explosionSystem.ExplodeSimple(new Vector2(-46,-3),6f,2f,null);
    }
    public void PlaceEntitiesRandomly(string entityName, int howMany)
    {
        for (int i = 0; i < howMany; i++)
        {
            PlaceEntityByName(entityName,RandomAvailablePos(1)+new Vector3(0f,0f,-0.5f));
        }
    }
    public void PlaceBlocksRandomly(string blockName, int howMany)
    {
        for (int i = 0; i < howMany; i++)
        {
            PlaceBlockByName(blockName,RandomAvailablePos(1));
        }
    }
    public void PlaceVines(int howMany)
    {
        for (int i = 0; i < howMany; i++)
        {
            int randomVal = randomSystem.worldPlacementRNG.Next(3, 7);
            VineHangFrom(RandomAvailablePos(-1),randomVal);
        }
    }
    public void PlaceBlockClumpsRandomly(int howMany) //Upgrade to include blockType in the future
    {
        for (int i = 0; i < howMany; i++)
        {
            Vector3Int pos = RandomAvailablePos(1);
            BlockState flamGas = new BlockState(tempFlamGas,pos,allSystems.behaviourAdder,false);
            flamGas.AddBehaviour(new FlamGasBehaviour());
            allSystems.explosionSystem.BlockPlaceExplosion(pos,6,flamGas);
        }
    }
    public void PlaceEntityByName(string entityName, Vector3 position)
    {
        GameObject placeFab = allSystems.entitySummonSystem.SummonEntityFabOnName(entityName,position);
    }
    public void PlaceBlockByName(string blockName, Vector3Int position)
    {
        BlockData blockToPlace = allSystems.blockLibrary.allBlocks[blockName];
        mapManager.PlaceBlock(position,blockToPlace);
    }
    public Vector3Int RandomAvailablePos(int yInc)
    {
        Vector3Int randomPos = RandomBlockPosition();
        int attempts = 0;
        while (mapManager.HasBlock(randomPos) && attempts < 15)
        {   //until we find a block that isn't taken and and supported
            attempts++;
            randomPos += new Vector3Int(0,yInc,0);
            if (randomPos.y > yLimit.y || randomPos.y < yLimit.x)
            {
                randomPos = new Vector3Int(randomPos.x,0,randomPos.z);
            }
        }
        //PlaceBlockByName("ladder",randomPos);
        return randomPos;
    }
    public void VineHangFrom(Vector3Int startPos,int length)//check positions
    {
        Vector3Int pos = startPos;
        int attempts = 0;
        while (attempts < length)
        {
            PlaceBlockByName("vine",pos);
            pos += new Vector3Int(0,-1,0);
            if (pos.y > yLimit.y || pos.y < yLimit.x)
            {
                break;
            }
            attempts++;
        }
    }
    public Vector3Int RandomPosition()
    {
        int xPos = randomSystem.worldPlacementRNG.Next(xLimit.x,xLimit.y+1);
        int yPos = randomSystem.worldPlacementRNG.Next(yLimit.x,yLimit.y+1);
        return (new Vector3Int(xPos,yPos,-1));
    }
    public Vector3Int RandomBlockPosition()
    {
        int randomIndex = randomSystem.worldPlacementRNG.Next(0,mapManager.allBlocks.Count);
        while (mapManager.allBlocks.ElementAt(randomIndex).Key.x < xLimit.x)
        {
            randomIndex = randomSystem.worldPlacementRNG.Next(0,mapManager.allBlocks.Count);
        }
        return mapManager.allBlocks.ElementAt(randomIndex).Key;
    }
    IEnumerator PlaceContent(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        PlaceMapThings();
    } 
    IEnumerator ScanTheWorld(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        AstarPath.active.Scan();
    } 
}
