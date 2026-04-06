using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public Inventory inventory;
    public GameObject invCanvas;
    public EntityGeneral containerEntity;
    public Vector3 UIoffset;

    public GameObject slotParent;
    public GameObject slotUIfab;

    public PlayerGeneral temp_player;
    public Vector2Int containSize;
    public float slotMargin = 1.1f;

    public GameObject currentUICanv;
    public bool currentlyOpen = false;
    public AllSystems allSystems;
    ItemSystem itemSystem;
    public GameObject currentOpener;
    [SerializeField] float closeDistance = 4f;
    public float UIsizeMult = 1f;
    //[SerializeField] ItemData potion
    public List<string> lootTable;
    public bool randomLoot = true;

    public bool locked = false;
    void Start()
    {
        //currentUICanv = OpenInvCanvas(temp_player);
        containerEntity = this.GetComponent<EntityGeneral>();
        allSystems = inventory.allSystems;
        itemSystem = allSystems.itemSystem;
        inventory.items = new ItemState[containSize.x*containSize.y];
        if (randomLoot)
        {
            MultipleRandomLoot(1,3);
        }
        /*inventory.GiveItem(new ItemState(itemSystem.allItems["throw_rock"],containerEntity,allSystems,true));
        inventory.GiveItem(new ItemState(itemSystem.allItems["block_bomb"],containerEntity,allSystems,true));
        inventory.GiveItem(new ItemState(itemSystem.allItems["potion"],containerEntity,allSystems,true));
        inventory.GiveItem(new ItemState(itemSystem.allItems["potion"],containerEntity,allSystems,true));
        inventory.GiveItem(new ItemState(itemSystem.allItems["potion"],containerEntity,allSystems,true));
        inventory.GiveItem(new ItemState(itemSystem.allItems["potion"],containerEntity,allSystems,true));
        inventory.GiveItem(new ItemState(itemSystem.allItems["potion"],containerEntity,allSystems,true));
        inventory.GiveItem(new ItemState(itemSystem.allItems["potion"],containerEntity,allSystems,true));
        inventory.GiveItem(new ItemState(itemSystem.allItems["plant_seed"],containerEntity,allSystems,true));*/
    }
    void Update()
    {
        if (currentlyOpen)
        {
            bool distanceTooMuch = (Vector3.Distance(currentOpener.transform.position,this.transform.position) > closeDistance);
            bool pressedESC = Input.GetKeyDown("escape");
            bool pressedTab = Input.GetKeyDown(KeyCode.Tab);
            bool pressedInteract = Input.GetKeyDown(KeyCode.E);
            if (pressedTab)
            {
                Inventory currentOpenerInv = currentOpener.GetComponent<Inventory>();
                TransferItem(inventory,currentOpenerInv,inventory.GetFirstFullSlot());
            }
            if (distanceTooMuch || pressedESC)
            {
                CloseInvCanvas();
            }
        }
    }
    public void TransferItem(Inventory from, Inventory to, int slot)
    {
        if (from.HasItems() == false)
        {
            return; //No Items to Take, maybe make a sound effect
        }
        if (to.IsFull())
        {
            return; //The player's already full
        }
        to.GiveItem(from.GetSlotItem(slot));
        from.ClearItemSlot(slot);
    }

    public void InteractedWith(PlayerGeneral byWhom)
    {
        if (currentlyOpen)
        {
            //CloseInvCanvas();
        }
        else if (!locked) //interacted with
        {
            currentUICanv = OpenInvCanvas(byWhom);
            currentUICanv.transform.localScale = new Vector3(currentUICanv.transform.localScale.x*UIsizeMult,currentUICanv.transform.localScale.y*UIsizeMult,currentUICanv.transform.localScale.z);
        }
        else
        {
            //locked
            Debug.Log("locked!");
        }
    }

    public GameObject OpenInvCanvas(PlayerGeneral whoOpenedIt) //maybe return the said canvas
    {
        GameObject canv = Instantiate(invCanvas,whoOpenedIt.containerUIParent.position,Quaternion.identity,whoOpenedIt.containerUIParent);
        currentOpener = whoOpenedIt.gameObject;
        //Resize It's Backdrop
        canv.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector3(containSize.x,containSize.y,1f)*1.15f;
        //canv.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition += new Vector2(-0.43f,-0.43f); 
        
        //Make Player Unable To Use Items
        whoOpenedIt.canUseItems = false;

        //Introduce Yourself To The Player As Their Current Container
        whoOpenedIt.currentOpenContainer = this;
        whoOpenedIt.containerUIParent.parent.gameObject.SetActive(true); //Open their ability to see you

        //Make slots
        slotParent = canv.transform.GetChild(1).gameObject; //this is where the inventory parent is
        CreateAllSlotsUI();
        currentlyOpen = true;
        return canv;
    }

    public void CreateAllSlotsUI() //return as ItemSlotUI array
    {
        Vector3 originPos = new Vector3(containSize.x,containSize.y)/-2f;
        for (int iy = 0; iy < containSize.y; iy++)
        {
            for (int ix = 0; ix < containSize.x; ix++)
            {
                CreateSlotUI((containSize.y-iy-1)*containSize.x+ix,originPos+new Vector3(ix*slotMargin,iy*slotMargin,0f));
            }
        }
    }

    public void CreateSlotUI(int orderPos, Vector3 instOffset)
    {
        GameObject createdSlot = Instantiate(slotUIfab,slotParent.transform.position+instOffset,Quaternion.identity,slotParent.transform); 
        ItemSlotUI slotUI = createdSlot.GetComponent<ItemSlotUI>();
        slotUI.AssignAttributes(this,inventory,orderPos);
        slotUI.UpdateVisual();
    }
    public void CloseInvCanvas()
    {
        PlayerGeneral openerPlayer = currentOpener.GetComponent<PlayerGeneral>();
        currentlyOpen = false;
        openerPlayer.canUseItems = true;
        openerPlayer.currentOpenContainer = null;
        openerPlayer.containerUIParent.parent.gameObject.SetActive(false);
        currentOpener = null;
        Destroy(currentUICanv);
    }
    public void MultipleRandomLoot(int amountMin,int amountMax)
    {
        int amount = containerEntity.allSystems.randomSystem.lootRNG.Next(amountMin,amountMax+1);
        int count = 0;
        while(count < amount)
        {
            GiveRandomLoot();
            count++;
        }
    }
    public void GiveRandomLoot()
    {
        int randomInTable = containerEntity.allSystems.randomSystem.lootRNG.Next(0,lootTable.Count);
        string itemToHave = lootTable[randomInTable];
        inventory.GiveItem(new ItemState(itemSystem.allItems[itemToHave],containerEntity,allSystems,true));
    }
}
