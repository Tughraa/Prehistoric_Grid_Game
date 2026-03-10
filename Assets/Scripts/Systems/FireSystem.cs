using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSystem : MonoBehaviour
{
    [SerializeField] MapManager mapManager;
    [SerializeField] GameObject smallExplosionParticle;
    [SerializeField] GameObject midExplosionParticle;
    [SerializeField] GameObject flamesFab;
    public AllSystems allSystems;
    public Dictionary<Vector3Int,Flames> currentFlames = new Dictionary<Vector3Int,Flames>();

    void OnEnable()
    {
        mapManager.BlockPlaced += OnBlockPlaced; //Subscribe and start to listen
    }

    void OnDisable()
    {
        mapManager.BlockPlaced -= OnBlockPlaced; //stop listening
    }
    void OnBlockPlaced(Vector3Int pos, BlockData data)
    {
        if (data.tags.Contains("burning")) //when a burning block is placed
        {
            List<Vector3Int> neighbours = FindNeighbouringBlocks(pos);
            foreach (Vector3Int nPos in neighbours)
            {
                FireUpBlock(nPos);
            }
        }
        if (BlockFireInteractable(data))
        {
            List<Vector3Int> neighbours = FindNeighbouringBlocks(pos);
            foreach (Vector3Int nPos in neighbours)
            {
                if (CheckForFire(nPos))
                {
                    FireUpBlock(pos);
                }
            }
        }
    }
    public void SummonFireBurst(Vector3Int pos)
    {
        //Instantiate(smallExplosionParticle,pos,Quaternion.identity);
        Instantiate(midExplosionParticle,pos,Quaternion.identity);
        
        Vector2 vec2pos = new Vector2(pos.x,pos.y);
        List<IStatusEffect> listWBurn = new List<IStatusEffect>();
        listWBurn.Add(new BurnEffect(3f,0.5f,0.75f));
        allSystems.explosionSystem.ExplodePreciseDelay(36, vec2pos, 2f, 210f, 2.5f,1f,listWBurn,false, 0.225f);
        //Maybe create a precision explosion from allSystems.explosionSystem.ExplodePrecise
    }

    public List<Vector3Int> FindNeighbouringBlocks(Vector3Int pos) //fix this when your tummy feels better
    {
        List<Vector3Int> returnVec = new List<Vector3Int>();
        returnVec.Add(pos+new Vector3Int(0,1,0));//up
        returnVec.Add(pos+new Vector3Int(0,-1,0));//down
        returnVec.Add(pos+new Vector3Int(-1,0,0));//left
        returnVec.Add(pos+new Vector3Int(1,0,0));//right
        return returnVec;
    }
    public bool BlockFireInteractable(BlockData saidBlock)
    {
        //BlockData saidBlock = mapManager.GetBlock(pos).blockData;
        if (saidBlock.tags.Contains("flammable"))
        {
            return true;
        }
        if (saidBlock.tags.Contains("fire_interact"))
        {
            return true;
        }
        return false;
    }
    public void FireUpBlock(Vector3Int pos)
    {
        if (mapManager.HasBlock(pos) == false)
        {
            return;
        }
        BlockData blockData = mapManager.GetBlock(pos).blockData;
        if (blockData.id == "flammable_gas")
        {
            mapManager.GetBlock(pos).GetBehaviour<FlamGasBehaviour>().Ignite(mapManager,pos);
            return;
        }
        if (blockData.id == "explosive")
        {
            mapManager.GetBlock(pos).GetBehaviour<ExplosiveBehaviour>().Explode(mapManager,pos);
            return;
        }
        if (blockData.tags.Contains("flammable"))
        {
            SummonFlames(pos);
            return;
        }
    }
    public void SummonFlames(Vector3Int pos)
    {
        if (currentFlames.ContainsKey(pos))
        {
            return;
        }
        GameObject flamesSummoned = Instantiate(flamesFab,pos+new Vector3(0,0,-1),Quaternion.identity);
        flamesSummoned.GetComponent<Flames>().allSystems = allSystems;
        currentFlames.Add(pos,flamesSummoned.GetComponent<Flames>());
    }
    public bool CheckForFire(Vector3Int pos)
    {
        if (mapManager.HasBlock(pos) == false)
        {
            return false;
        }
        if (mapManager.GetBlock(pos).blockData.tags.Contains("burning"))
        {
            return true;
        }
        return false;
    }
    public void BurnEntities(Vector3Int pos, float range)
    {
        Vector2 originPos = new Vector2(pos.x,pos.y);
        Collider2D[] results = Physics2D.OverlapBoxAll(originPos,new Vector2(range, range),0f);
        //Debug.Log(originPos);
        Debug.DrawLine(originPos+new Vector2(-range/2f,-range/2f), originPos+new Vector2(range/2f,range/2f), Color.red);
        if (results.Length > 0)
        {
            foreach (var col in results)
            {    
                if (col.gameObject.GetComponent<EntityStatusEffects>())
                {
                    if (col.gameObject.GetComponent<EntityGeneral>().flammable == false)
                    {
                        return;
                    }
                    col.gameObject.GetComponent<EntityStatusEffects>().AddEffect(new BurnEffect(duration: 6f,strength: 0.5f,0.75f)); //Make the effect modular
                }
            }
        }
    }
}
