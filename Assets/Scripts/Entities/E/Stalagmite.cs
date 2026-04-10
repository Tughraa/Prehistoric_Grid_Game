using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stalagmite : MonoBehaviour
{
    public EntityGeneral entity;
    MapManager mapManager;
    public bool still = true;
    public Vector3Int tiedBlock;
    void Start()
    {
        entity = this.GetComponent<EntityGeneral>();
        mapManager = entity.allSystems.mapManager;
        tiedBlock = mapManager.FloatToGridPos(this.transform.position+new Vector3(0f,1f,0f));
        entity.EntityDamaged += GetHurt; //Subscribed on the start
        mapManager.BlockRemoved += CheckBlock;
    }
    void OnEnable()
    {
        //entity = this.GetComponent<EntityGeneral>();
        //mapManager = entity.allSystems.mapManager;
        //entity.EntityDamaged += GetHurt; //Subscribe and start to listen
        //mapManager.BlockRemoved += CheckBlock;
    }

    void OnDisable()
    {
        entity.EntityDamaged -= GetHurt; //stop listening
        mapManager.BlockRemoved -= CheckBlock;
    }
    public void CheckBlock(Vector3Int pos, BlockData data)
    {
        if (pos == tiedBlock)
        {
            MakeUnstil();
        }
    }
    public void GetHurt(EntityGeneral ent, float amount, string type)
    {
        MakeUnstil();//Not interested in type or amount
    }
    public void MakeUnstil()
    {
        still = false;
        entity.rigid.constraints = RigidbodyConstraints2D.None;
        entity.rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
