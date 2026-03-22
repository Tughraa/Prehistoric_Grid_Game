using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Inventory myInventory;
    public int invPosition;
    public Image itemImage;
    bool isMouseOver = false;
    [SerializeField] GameObject durabilityUI;
    [SerializeField] Color hoverColor = new Color(0.8f,0.8f,0.8f,1f);
    GameObject durabilityBar;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
        this.GetComponent<Image>().color = hoverColor;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        this.GetComponent<Image>().color = Color.white;
    }

    public void AssignAttributes(Container cont, Inventory inv, int pos)
    {
        myInventory = inv;
        invPosition = pos;
    }
    public void UpdateVisual()
    {
        if (myInventory.SlotHasItem(invPosition)) //if there is an item at all
        {
            itemImage.enabled = true;
            itemImage.sprite = myInventory.items[invPosition].itemData.sprite;
            if (myInventory.items[invPosition].HasBehaviour<DurabilityBehaviour>()) //if it's an item w/ durability
            {
                DurabilityBehaviour durab = myInventory.items[invPosition].GetBehaviour<DurabilityBehaviour>();
                if (durab.DurabilityFraction() < 1f && durabilityBar == null)
                {
                    durabilityBar = Instantiate(durabilityUI,this.transform.position,Quaternion.identity,this.transform);
                }
                if (durabilityBar != null)
                {
                    durabilityBar.transform.GetChild(1).GetComponent<Image>().fillAmount = durab.DurabilityFraction();
                }
            }
        }
        else
        {
            //remove sprites
            itemImage.enabled = false;
            Destroy(durabilityBar);
        }
    }
    void Update()
    {
        //decouple this system!
        if (Input.GetMouseButtonDown(0) && isMouseOver) //the said player is defined also in "clickedOn"
        {
            ClickedOn();
        }
        if (Input.GetMouseButtonUp(0) && isMouseOver)
        {
            StoppedClickingOn();
        }
        UpdateVisual(); //May not be the best solution, but will work for now.
    }
    public void ClickedOn()
    {
        //if empty do nothing, else make player pick up the item
        if (myInventory.SlotHasItem(invPosition) == false)
        {
            Debug.Log("slot "+(invPosition+1)+" was clicked.");
        }
        else if (CurrentPlayer() != null) //If slot aint empty, let the player pick the item up
        {
            CurrentPlayer().AssignMouseItem(myInventory.GetSlotItem(invPosition), this);
            RemoveItemThisSlot(); //Taking and removing may not be right, think about the way we moved blocks instead
        }
        UpdateVisual();
    }
    public void StoppedClickingOn()
    {
        if (CurrentPlayer() == null) //if you can't access the current player, stop
        {
            return;
        }
        PlayerGeneral player = CurrentPlayer();
        if (player.GetMouseItem() == null) //no item on player's mouse means stop
        {
            return;
        }
        if (myInventory.SlotHasItem(invPosition) == true) //if slot is full, SWITCH
        {
            ItemState itemBuffer = myInventory.GetSlotItem(invPosition);
            RemoveItemThisSlot();
            ReplaceItemThisSlot(player.GetMouseItem());
            player.GetMouseItemOrigin().ReplaceItemThisSlot(itemBuffer);
            player.RemoveMouseItem();
        }
        else if (myInventory.SlotHasItem(invPosition) == false) //if not full, then just place the item
        {
            ReplaceItemThisSlot(player.GetMouseItem());
            player.RemoveMouseItem();
        }
        UpdateVisual();
    }
    public void ReplaceItemThisSlot(ItemState itemToReplace)
    {
        myInventory.ReplaceItem(invPosition,itemToReplace);
        UpdateVisual();
    }
    public void RemoveItemThisSlot()
    {
        myInventory.ClearItemSlot(invPosition);
        UpdateVisual();
    }
    public PlayerGeneral CurrentPlayer()
    {
        if (myInventory.invEntity.GetComponent<PlayerGeneral>()) //it's a player slot
        {
            return myInventory.invEntity.GetComponent<PlayerGeneral>();
        }
        else if (myInventory.invEntity.GetComponent<Container>()) //it's a container slot
        {
            return myInventory.invEntity.GetComponent<Container>().currentOpener.GetComponent<PlayerGeneral>();
        }
        else
        {
            return null;
        }
    }
}
