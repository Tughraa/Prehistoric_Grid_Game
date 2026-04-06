using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Inventory myInventory;
    public int invPosition;
    public Image itemImage;
    bool isMouseOver = false;
    [SerializeField] GameObject durabilityUI;
    [SerializeField] Color hoverColor = new Color(0.8f,0.8f,0.8f,1f);
    GameObject durabilityBar;
    [SerializeField] GameObject itemDescFab;
    Transform currentItemDesc;
    float doubleClickTimer = 0f;
    public float doubleClickWindow = 0.3f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
        this.GetComponent<Image>().color = hoverColor;
        //Create an item description if theres an item
        if (myInventory.SlotHasItem(invPosition))
        {
            //myInventory.items[invPosition]
            GameObject descFab = Instantiate(itemDescFab,this.transform.position+new Vector3(0f,1.5f,1f),Quaternion.identity,this.transform.parent);
            currentItemDesc = descFab.transform;
            //Change the things within the item description
            currentItemDesc.GetChild(0).GetComponent<TMP_Text>().text = myInventory.items[invPosition].itemData.itemName;
            //If description is added, add it here or at least symbolize it through images given here.
            //currentItemDesc.GetChild(1).GetComponent<TMP_Text>().text = myInventory.items[invPosition].itemData.itemDesc;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        this.GetComponent<Image>().color = Color.white;
        //Delete the item description if theres one
        if (currentItemDesc != null)
        {
            Destroy(currentItemDesc.gameObject);
            currentItemDesc = null;
        }
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
        UpdateVisual(); //May not be the best solution to put this at update, but will work for now.
        if (doubleClickTimer > 0f)
        {
            doubleClickTimer -= Time.deltaTime;
        }
    }
    public void ClickedOn()
    {
        //if empty do nothing, else make player pick up the item
        if (Input.GetKey(KeyCode.LeftShift))
        {
            TransferCurrentItem();
        }
        DetectDoubleClick();
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
    void DetectDoubleClick()
    {
        if (doubleClickTimer > 0f)
        {
            TransferCurrentItem();
            doubleClickTimer = 0f;
        }
        else
        {
            doubleClickTimer = doubleClickWindow;
        }
    }
    void TransferCurrentItem()
    {
        if (myInventory.SlotHasItem(invPosition) == false)
        {
            return; //No item to transfer
        }
        if (myInventory.GetComponent<Container>()) //if it's a container
        {
            Container myContainer = myInventory.GetComponent<Container>();
            myContainer.TransferItem(myInventory,myContainer.currentOpener.GetComponent<Inventory>(),invPosition);
        }
        else if (myInventory.GetComponent<PlayerGeneral>()) //if it's a player
        {
            PlayerGeneral slotsPlayer = myInventory.GetComponent<PlayerGeneral>();
            if (slotsPlayer.currentOpenContainer == null)
            {
                //Might be best not to do anythin, but could also make a sound effect
            }
            else
            {
                Container openContainer = slotsPlayer.currentOpenContainer;
                openContainer.TransferItem(myInventory,openContainer.GetComponent<Inventory>(),invPosition);
            }
        }
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
