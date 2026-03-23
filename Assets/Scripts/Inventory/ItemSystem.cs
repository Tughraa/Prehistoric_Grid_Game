using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSystem : MonoBehaviour
{
    public AllSystems allSystems;
    public Dictionary<string, ItemData> allItems = new Dictionary<string, ItemData>();
    public List<ItemData> allItemsAssign;

    void Awake()
    {
        foreach (ItemData idata in allItemsAssign) {
            allItems[idata.id] = idata;
        }
    }
    public GameObject SummonItemWithState(Vector3 pos,ItemState state, string itemSource)
    {
        GameObject summonedObj = allSystems.entitySummonSystem.SummonEntityFabOnName("item_entity",pos);
        ItemEntity summonedItem = summonedObj.GetComponent<ItemEntity>();
        summonedItem.InitWithState(state,itemSource);
        return summonedObj;
    }
    public GameObject SummonItemWithName(Vector3 pos,string itemID, string itemSource)
    {
        GameObject summonedObj = allSystems.entitySummonSystem.SummonEntityFabOnName("item_entity",pos);
        ItemEntity summonedItem = summonedObj.GetComponent<ItemEntity>();
        summonedItem.InitWithName(itemID,itemSource);
        return summonedObj;
    }

    public void UseItem(EntityGeneral user, ItemState itemState, Vector3 mousePos, float mouseHeldTime, Inventory inventory, int slot)
    {
        itemState.ItemUse(user, mousePos, mouseHeldTime, inventory, slot);
    }
    public void RightClickItem(EntityGeneral user, ItemState itemState, Vector3 mousePos, float mouseHeldTime, Inventory inventory, int slot)
    {
        itemState.ItemRightClick(user, mousePos, mouseHeldTime, inventory, slot);
    }
}
