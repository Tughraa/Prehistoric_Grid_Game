using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEntity : MonoBehaviour
{
    public AllSystems allSystems;
    ItemPickUp itemPickUp;
    public EntityGeneral entityGeneral;
    public ItemState itemState;
    public string source = "world";
    void Start()
    {
        entityGeneral = this.GetComponent<EntityGeneral>();
        allSystems = entityGeneral.allSystems;
        itemPickUp = this.GetComponent<ItemPickUp>();
        //InitWithName("fire_starter","world");
    }
    public void InitWithName(string itemID,string inSource)
    {
        itemState = new ItemState(allSystems.itemSystem.allItems[itemID],entityGeneral,allSystems,true);
        GeneralInit(inSource);
    }
    public void InitWithState(ItemState inItemState,string inSource)
    {
        itemState = inItemState;
        GeneralInit(inSource);
    }
    void GeneralInit(string inSource)
    {
        entityGeneral = this.GetComponent<EntityGeneral>();
        allSystems = entityGeneral.allSystems;
        itemPickUp = this.GetComponent<ItemPickUp>();
        if (itemState == null)
        {
            Destroy(this.gameObject); //THIS IS THROWING THE PROBLEM UNDER A RUG
            return;
        }
        itemPickUp.storedItem = itemState;
        source = inSource;
        entityGeneral.entityName = itemState.itemData.itemName;
        this.GetComponent<SpriteRenderer>().sprite = itemState.itemData.sprite;
    }
}
