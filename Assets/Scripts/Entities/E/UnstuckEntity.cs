using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnstuckEntity : MonoBehaviour
{
    public EntityGeneral entityGeneral;
    MapManager mapManager; 
    void Start()
    {
        entityGeneral = this.GetComponent<EntityGeneral>();
        mapManager = entityGeneral.allSystems.mapManager;
    }

    void Update()
    {
        Vector3Int currentGridPos = mapManager.FloatToGridPos(this.transform.position);
        if (HasCollision(currentGridPos))
        {
            this.transform.position = FindAvailableSpace(currentGridPos);
        }
    }
    Vector3 FindAvailableSpace(Vector3Int startPos)
    {
        if (HasCollision(startPos) == false)
        {
            return startPos;
        }
        Vector3 closest = this.transform.position+(this.transform.position-startPos).normalized*0.5f;
        Vector3Int closestInt = mapManager.FloatToGridPos(closest);
        if (HasCollision(closestInt) == false)
        {
            Debug.Log(closestInt+" was empty");
            return closestInt;
        }
        else
        {
            Debug.Log(closestInt+" wasnt empty");
        }
        return startPos+new Vector3(0f,0.5f,0f);
    }   

    public bool HasCollision(Vector3Int pos)
    {
        if (mapManager.HasBlock(pos))
        {
            if (mapManager.GetBlock(pos).blockData.hasCollision)
            {
                return true;
            }
        }
        return false;
    } 
}
