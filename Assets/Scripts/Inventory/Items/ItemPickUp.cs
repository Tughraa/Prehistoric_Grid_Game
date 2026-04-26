using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public ItemState storedItem;
    public bool itemEntity = true;
    public string assignItemName;

    //float timer = 0f;
    void Start()
    {
        //timer += Time.deltaTime;
        if (assignItemName != "")
        {
            AllSystems allSystems = this.GetComponent<EntityGeneral>().allSystems;
            ItemData toldItemData = allSystems.itemSystem.allItems[assignItemName];
            storedItem = new ItemState(toldItemData,this.GetComponent<EntityGeneral>(),allSystems,true);
        }
    }
    public void GetPickedUp()
    {
        Destroy(this.gameObject);
    }
}
