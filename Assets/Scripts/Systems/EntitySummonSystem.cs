using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySummonSystem : MonoBehaviour
{
    public AllSystems allSystems;
    
    public Dictionary<string, GameObject> entityFabs = new Dictionary<string, GameObject>();
    public List<GameObject> allEntityFabsAssign;

    void Awake()
    {
        //allItemsAssign
        foreach (GameObject entity in allEntityFabsAssign) {
            entityFabs[entity.GetComponent<EntityGeneral>().entityType] = entity;
        }
    }

    public GameObject SummonEntityFab(GameObject prefab, Vector3 summonPos)
    {
        GameObject summonedEnt = Instantiate(prefab,summonPos,Quaternion.identity);
        AssignSystemsToSummon(summonedEnt);
        return summonedEnt;
    }
    public GameObject SummonEntityFabOnName(string entityType, Vector3 summonPos)
    {
        GameObject summonedEnt = Instantiate(entityFabs[entityType],summonPos,Quaternion.identity);
        AssignSystemsToSummon(summonedEnt);
        return summonedEnt;
    }

    public void AssignSystemsToSummon(GameObject summon)
    {
        summon.GetComponent<EntityGeneral>().allSystems = allSystems;
    }
}
