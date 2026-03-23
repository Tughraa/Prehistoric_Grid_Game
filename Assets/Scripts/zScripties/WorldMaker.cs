using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldMaker : MonoBehaviour
{
    public MapManager mapManager;
    public AllSystems allSystems;
    public RandomSystem randomSystem;
    public BlockData defaultBlock;
    public BlockData defaultLightBlock;
    public Vector2Int xLimit;
    public Vector2Int yLimit;
    public float perlinThreshold = 0.4f;
    public float perlinMultX = 0.1f;
    public float perlinMultY = 0.2f;
    public float perlinConst = 100f;

    [SerializeField] BlockData tempFlamGas;

    [Header("Generation Counts")]
    [SerializeField] int vineCount = 125;
    [SerializeField] int explosiveCount = 40;
    [SerializeField] int chestCount = 75;
    [SerializeField] int shooterCount = 0;
    [SerializeField] int stalagmiteCount = 50;
    [SerializeField] int flamGasCount = 12;

    void Start()
    {
        randomSystem = allSystems.randomSystem;
        perlinConst *= (float)randomSystem.terrainGenRNG.NextDouble()*2036f;
        //MapClear();
        mapManager.ReadMapBlocks();
        PutBlocks(xLimit,yLimit);
        PutSurface(xLimit,yLimit.y+2,20);
        StartCoroutine(PlaceContent(0.5f));
        StartCoroutine(ScanTheWorld(0.7f));
    }
    void MapClear()
    {
        mapManager.blockLayer.ClearAllTiles();
    }
    void PutSurface(Vector2Int xRange, int yLower, int thickness)
    {
        for (int ix = xLimit.x; ix <= xLimit.y; ix++)
        {
            float upperPerlin = (OneDimPerlinEval(ix,perlinConst*5f,0.05f)*3f + OneDimPerlinEval(ix,perlinConst*3f,0.01f)*7f)/10f;
            float middlePerlin = (upperPerlin /1.5f)-OneDimPerlinEval(ix,perlinConst*90f,0.08f)/4f;
            float lowerPerlin = (upperPerlin /2f)-OneDimPerlinEval(ix,perlinConst*10f,0.05f)/3f;
            
            for (int iy = 0; iy <= thickness; iy++)
            {
                Vector3Int checkPos = new Vector3Int(ix,iy+yLower,0);
                float yRatio = (float)iy/(float)thickness;
                bool over = (upperPerlin > yRatio) ? true : false;
                bool under = (lowerPerlin < yRatio) ? true : false;
                //Debug.Log("ratio is: "+yRatio+"\nperlin is: "+upperPerlin+over);
                if (over && under && mapManager.HasBlock(checkPos) == false)
                {
                    if (middlePerlin < yRatio)
                    {
                        mapManager.PlaceBlock(checkPos,defaultLightBlock);
                    }
                    else
                    {
                        mapManager.PlaceBlock(checkPos,defaultBlock);
                        //mapManager.blockLayer.SetColor(checkPos,new Color(0.8f,0.5f,0.4f,1f));
                    }
                }
            }

        }
    }
    void PutBlocks(Vector2Int xRange, Vector2Int yRange)
    {
        for (int ix = xLimit.x; ix <= xLimit.y; ix++)
        {
            for (int iy = yLimit.x; iy <= yLimit.y; iy++)
            {
                //Debug.Log(ix+" "+iy+" "+ Mathf.PerlinNoise((float)ix,(float)iy));
                if (PerlinEval(ix,iy,perlinThreshold) && mapManager.HasBlock(new Vector3Int(ix,iy,0)) == false)
                {
                    if (PerlinEval(ix,iy,perlinThreshold*1.4f))
                    {
                        mapManager.PlaceBlock(new Vector3Int(ix,iy,0),defaultLightBlock);
                    }
                    else
                    {
                        mapManager.PlaceBlock(new Vector3Int(ix,iy,0),defaultBlock);
                    }
                }
            }
        }
    }
    bool PerlinEval(float xP, float yP,float threshold)
    {
        float perlinVal = Mathf.PerlinNoise(xP*perlinMultX+perlinConst,yP*perlinMultY+perlinConst);
        //Debug.Log(perlinVal);
        if (perlinVal >= threshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    float OneDimPerlinEval(float xPos, float constant, float strech)
    {
        return Mathf.PerlinNoise(xPos*strech+constant,perlinConst); 
    }
    //Placing other other things
    void PlaceMapThings()
    {
        PlaceVines(vineCount); //normally 75
        PlaceBlocksRandomly("explosive",explosiveCount);
        PlaceEntitiesRandomly("chest",chestCount,1); //it was 50 before
        PlaceEntitiesRandomly("shooter_enemy",shooterCount,1);
        PlaceEntitiesRandomly("stalagmite",stalagmiteCount,-1); //adjust
        PlaceBlockClumpsRandomly(flamGasCount); //normally 7
        allSystems.explosionSystem.ExplodeSimple(new Vector2(-46,-3),6f,2f,null);
    }
    public void PlaceEntitiesRandomly(string entityName, int howMany, int dir)
    {
        for (int i = 0; i < howMany; i++)
        {
            PlaceEntityByName(entityName,RandomAvailablePos(dir)+new Vector3(0f,0f,-0.5f));
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
