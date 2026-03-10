using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

//Maybe seperate playerUI in a different class
public class PlayerGeneral : MonoBehaviour
{
    public AllSystems allSystems;
    
    public PlayerMovement playerMovement;
    public EntityGeneral playerEntity;
    public Inventory playerInventory;
    public GameObject playerCam;
    
    public Canvas playerCanvas;

    [SerializeField] Image playerHealthBar;
    [SerializeField] Image playerHealthAnim;

    [SerializeField] Image playerDeadScreen;

    [SerializeField] TMP_Text announceBarUI;
    public TMP_Text itemNameUI;

    float heldDownTime = 0f;

    public bool canUseItems = true;

    public ItemSlotUI[] inventorySlotImages;
    public Image selectedSlotImage;

    private ItemState mouseItem;
    [SerializeField] Image mouseItemImage;
    private ItemSlotUI mouseItemOrigin;

    public Container currentOpenContainer = null;

    [SerializeField] GameObject playerItemThrowBar; //turn on when clicking chargable item
    [SerializeField] Image playerItemCharge;        //update based on item throwstuffs

    void Start()
    {
        playerMovement = this.GetComponent<PlayerMovement>();
        playerEntity = this.GetComponent<EntityGeneral>();
        playerInventory = this.GetComponent<Inventory>();
        playerItemThrowBar.SetActive(false);
    }
    void Update()
    {
        ChangeSlots();
        CheckInteractions();
        CheckUseItems();
        UpdateInventoryVisual();
        HeldItemSpecifier();
        if (mouseItem != null)
        {
            MouseHoldingItem();
        }
        //Restart
        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (this.transform.position.y < -180f)
        {
            playerEntity.Damage(10f,"void");
        }
        playerHealthAnim.fillAmount += (playerHealthBar.fillAmount-playerHealthAnim.fillAmount)/10f;
    }
    
    //Health
    public void GetHurt(string damageType)
    {
        UpdateHealthBar();
        if (playerEntity.health <= 0f) //Dead
        {
            playerDeadScreen.gameObject.SetActive(true);
            TMP_Text deathReason = playerDeadScreen.transform.GetChild(0).GetComponent<TMP_Text>();
            deathReason.text = GetDamageDeathReason(damageType);
            Time.timeScale = 0;
        }
        else
        {
            playerCam.GetComponent<ScreenShake>().StartShaking(0.1f,0.32f);
        }
    }
    public void GetHeal()
    {
        UpdateHealthBar();
    }
    public void UpdateHealthBar()
    {
        playerHealthBar.fillAmount = playerEntity.health/playerEntity.maxHealth;
    }
    public string GetDamageDeathReason(string damageType)
    {
        switch (damageType)
        {
            case "fire":
                return "You burned to death.";
            case "poison":
                return "The poison got to you.";
            case "void":
                return "...you fell for so long that you died of boredom.";
            case "smashing":
                return "Something crushed you to death.";
            case "explosion":
                return "u blew up.";
            case "enemy":
                return "You got too exposed to that red circle.";
            default:
                return "You died because of a bug? I don't know why you died...";
        }
    }


    //Inventory
    public void ChangeSlots()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && !Input.GetKey(KeyCode.LeftShift))
        {
            playerInventory.HeldItemSwitch(-1);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && !Input.GetKey(KeyCode.LeftShift))
        {
            playerInventory.HeldItemSwitch(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)) //maybe a more efficient method in the future?
        {playerInventory.HeldItemJump(0);}
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {playerInventory.HeldItemJump(1);}
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {playerInventory.HeldItemJump(2);}
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {playerInventory.HeldItemJump(3);}
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {playerInventory.HeldItemJump(4);}
    }
    public void CheckUseItems()
    {
        if (playerInventory.HeldItem() == null) //no items
        {
            return;
        }
        if (Input.GetMouseButton(0))   //Wait until player lets go
        {
            heldDownTime += Time.deltaTime;
            if (playerInventory.HeldItem().itemData.tags.Contains("throwable")) //find held item see throwable tag
            {
                playerItemThrowBar.SetActive(true);
                //change fill
                playerItemCharge.fillAmount = heldDownTime/0.9f;
            }
        }
        if (Input.GetMouseButtonUp(0) && canUseItems) //Now interact
        {
            
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            playerInventory.ItemUsing(mousePos,heldDownTime);

            heldDownTime = 0f;
            
            playerItemThrowBar.SetActive(false);
        }
        if (Input.GetMouseButtonUp(1) && canUseItems) //Now interact
        {
            
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            playerInventory.ItemRightClick(mousePos,heldDownTime);

            heldDownTime = 0f;
        }
    }
    public void UpdateInventoryVisual()
    {
        selectedSlotImage.transform.position = inventorySlotImages[playerInventory.heldItemIndex].transform.position;
        if (!playerInventory.HasItems())
        {
            return;
        }
        for (int i = 0; i < playerInventory.items.Length; i++)
        {
            inventorySlotImages[i].UpdateViusal();
        }
    }
    public void HeldItemSpecifier()
    {
        if (playerInventory.HeldItem() != null)
        {
            itemNameUI.enabled = true;
            itemNameUI.text = playerInventory.HeldItem().itemData.itemName;
        }
        else
        {
            itemNameUI.enabled = false;
        }
    }
    //Interactions
    public void CheckInteractions()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //CheckItems
            playerInventory.TryItemPickUp(this.transform.position,1f,true); //dynamicaly adjust for playerSize
            if (currentOpenContainer != null)
            {
                currentOpenContainer.CloseInvCanvas();
            }
            CheckInteractions(this.transform.position,1f);
            
        }
    }
    public void CheckInteractions(Vector2 where, float searchRadius)
    {
        Collider2D[] results = Physics2D.OverlapCircleAll(where,searchRadius);
        
        Debug.DrawLine(where- new Vector2(0f,searchRadius), where+ new Vector2(0f,searchRadius), Color.green,2f);
        Debug.DrawLine(where- new Vector2(searchRadius,0f), where+ new Vector2(searchRadius,0f), Color.green,2f);

        if (results.Length > 0)
        {
            Collider2D closest = FindClosestCollider(where, results);   
            //Debug.Log(closest.gameObject.name+ " is interacted with"); 
            if (closest.gameObject.GetComponent<Container>())
            {
                Container interactCont = closest.gameObject.GetComponent<Container>();
                interactCont.InteractedWith(this);
            }
        }
    }
    public Collider2D FindClosestCollider(Vector2 origin, Collider2D[] colliders)
    {
        Collider2D colliderIte = colliders[0];
        foreach (var col in colliders)
        {    
            if (col.gameObject.GetComponent<EntityGeneral>() == false)
            {
                continue;
            }
            if ((Vector2.Distance(origin,(Vector2)col.transform.position) < Vector2.Distance(origin,(Vector2)colliderIte.transform.position) 
                || !colliderIte.gameObject.GetComponent<EntityGeneral>().interactable)
                && col.gameObject.GetComponent<EntityGeneral>().interactable)
            {
                colliderIte = col;
            }
        }
        return colliderIte;
    }
    //Mouse Item
    public void AssignMouseItem(ItemState itemToGet, ItemSlotUI originSlot)
    {
        mouseItem = itemToGet;
        Debug.Log("mouse has a: "+mouseItem.itemData.itemName);
        mouseItemOrigin = originSlot;
        
        mouseItemImage.sprite = itemToGet.itemData.sprite;
        mouseItemImage.enabled = true;
        mouseItemImage.transform.position = FindMousePos();
    }
    public void RemoveMouseItem()
    {
        mouseItem = null;
        mouseItemImage.enabled = false;
        mouseItemOrigin = null;
    }
    public ItemState GetMouseItem()
    {
        return mouseItem;
    }
    public ItemSlotUI GetMouseItemOrigin()
    {
        return mouseItemOrigin;
    }
    public Vector3 FindMousePos()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(playerCanvas.transform as RectTransform, Input.mousePosition, playerCanvas.worldCamera, out pos);
        return playerCanvas.transform.TransformPoint(pos);
    }
    public void MouseHoldingItem()
    {
        mouseItemImage.transform.position += (FindMousePos()-mouseItemImage.transform.position)*Time.deltaTime*10f;
        mouseItemImage.transform.eulerAngles = new Vector3(0f,0f,FindMousePos().x-mouseItemImage.transform.position.x)*15f;
    }
    //Other UI Elements
    public void Announce(string inText, float totalTime, Color inColor)
    {
        announceBarUI.text = inText;
        StartCoroutine(AnnounceIE(totalTime,inColor));
    }
    IEnumerator AnnounceIE(float totalTime, Color inColor)
    {
        float timer = totalTime;
        while (timer > 0f)
        {
            announceBarUI.color = new Color(inColor.r,inColor.g,inColor.b,timer/totalTime);
            yield return null;
            timer -= Time.deltaTime;
        }
        announceBarUI.color = new Color(inColor.r, inColor.g, inColor.b, 0f);
    }
}
