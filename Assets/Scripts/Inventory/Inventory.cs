using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public ItemState[] items = new ItemState[5];
    public int heldItemIndex = 0;

    public EntityGeneral invEntity;

    public bool canUseItem = true;

    public AllSystems allSystems;
    public ItemSystem itemSystem;
    
    public bool dropOnDeath = true;
    public bool giveDevKit = false;
    
    void Start()
    {
        invEntity = this.GetComponent<EntityGeneral>();
        allSystems = invEntity.allSystems;
        itemSystem = allSystems.itemSystem;
        if (giveDevKit)
        {
            ReplaceItem(4,new ItemState(itemSystem.allItems["gods_hands"],invEntity,allSystems,true));
        }
    }
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.X))
        {
            GiveItem(new ItemState(itemSystem.allItems["throw_rock"],invEntity,allSystems,true));
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            GiveItem(new ItemState(itemSystem.allItems["plant_seed"],invEntity,allSystems,true));
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            GiveItem(new ItemState(itemSystem.allItems["block_bomb"],invEntity,allSystems,true));
        }*/
    }

    //Giving and removing items
    public void ReplaceItem(int slot, ItemState item)
    {
        item.ownerEntity = invEntity;
        items[slot] = item;
        if (slot == heldItemIndex)
        {
            if (HeldItem() != null)
            {HeldItem().ItemStartHolding(invEntity,this,heldItemIndex);}
        }
    }
    public bool GiveItem(ItemState item)
    {
        if (IsFull())
        {
            //Summon an item entity instead
            return false; //The inventory's already full
        }
        for (int i = 0; i < items.Length; i++)
        {
            if (!SlotHasItem(i))
            {
                ReplaceItem(i,item);
                return true; 
            }
        }
        return false;
    }
    public void ClearItemSlot(int slot)
    {
        if (slot == heldItemIndex)
        {
            if (HeldItem() != null)
            {HeldItem().ItemStopHolding(invEntity,this,heldItemIndex);}
        }
        items[slot] = null;
    }

    //Checking and getting items
    public ItemState HeldItem() 
    {
        return items[heldItemIndex];
    }
    public ItemState GetSlotItem(int slot)
    {
        return items[slot];
    }
    public bool HasItems() //this inventory has at least 1 item
    {
        foreach(ItemState item in items)
        {
            if (item != null)
            {
                return true;
            }
        }
        return false; 
    }
    
    public bool SlotHasItem(int slotNum)
    {
        return !(items[slotNum] == null);
    }
    public bool IsFull() //inventory has no slots free
    {
        foreach(ItemState item in items)
        {
            if (item == null)
            {
                return false;
            }
        }
        return true; 
    }
    public int GetFirstFullSlot() 
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (SlotHasItem(i))
            {
                return i; 
            }
        }
        return 0;
    }

    //Item Dropping
    public void ItemDrop(int slot, Vector3 pos) //Add knockback to the item
    {
        if (SlotHasItem(slot) == false)
        {
            return;
        }
        GameObject dropped = itemSystem.SummonItemWithState(pos,GetSlotItem(slot),invEntity.entityName);
        //dropped.GetComponent<EntityGeneral>().Knockback...
        ClearItemSlot(slot);
    }
    public void DropAllItems()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (SlotHasItem(i))
            {
                Vector3 dropPos = this.transform.position + new Vector3(Mathf.Cos(i*12f),Mathf.Sin(i*12f),0f)*invEntity.entityReach;
                ItemDrop(i,dropPos);
            }
        }
    }
    void OnEnable() {invEntity.EntityDamaged += GetHurt;} //Death check, maybe if it's high enough you drop an item just maybe
    void OnDisable() {invEntity.EntityDamaged -= GetHurt;}
    public void GetHurt(EntityGeneral entity, float amount, string damageType)
    {
        if (entity.health <= 0f && dropOnDeath) //Dead
        {
            DropAllItems();
        }
    }

    //Item Interaction
    public void HeldItemSwitch(int changeDir)
    {
        int currentIndex = heldItemIndex;
        currentIndex += changeDir;
        if (currentIndex >= items.Length)
        {
            currentIndex = 0;
        }
        else if (currentIndex<0)
        {
            currentIndex = items.Length-1;
        }
        HeldItemJump(currentIndex);
    }
    public void HeldItemJump(int toJump)
    {
        if (HeldItem() != null)
        {HeldItem().ItemStopHolding(invEntity,this,heldItemIndex);}
        heldItemIndex = toJump;
        if (HeldItem() != null)
        {HeldItem().ItemStartHolding(invEntity,this,heldItemIndex);}
    }
    public void ItemUsing(Vector2 applyPos, float heldDownTime)
    {
        if (!canUseItem)
        {
            return;
        }
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        itemSystem.UseItem(invEntity,HeldItem(),applyPos,heldDownTime,this,heldItemIndex);
    }
    public void ItemRightClick(Vector2 applyPos, float heldDownTime)
    {
        if (!canUseItem)
        {
            return;
        }
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        itemSystem.RightClickItem(invEntity,HeldItem(),applyPos,heldDownTime,this,heldItemIndex);
    }
    public void TryItemPickUp(Vector2 where, float searchRadius, bool playerIsTrying)
    {
        Collider2D[] results = Physics2D.OverlapCircleAll(where,searchRadius);
        
        Debug.DrawLine(where- new Vector2(0f,searchRadius), where+ new Vector2(0f,searchRadius), Color.green,2f);
        Debug.DrawLine(where- new Vector2(searchRadius,0f), where+ new Vector2(searchRadius,0f), Color.green,2f);

        if (results.Length > 0)
        {
            foreach (var col in results)
            {    
                if (col.gameObject.GetComponent<ItemPickUp>())
                {
                    ItemPickUp itemObj = col.gameObject.GetComponent<ItemPickUp>();
                    if (itemObj.itemEntity || playerIsTrying)
                    {
                        if (GiveItem(itemObj.storedItem))
                        {
                            itemObj.GetPickedUp();
                        }
                    }
                }
            }
        }
    }
}
