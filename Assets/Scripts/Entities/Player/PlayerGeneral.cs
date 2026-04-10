using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

//Maybe seperate playerUI in a different class
public class PlayerGeneral : MonoBehaviour
{
    public GameObject enemyObj;
    public AllSystems allSystems;
    
    public PlayerMovement playerMovement;
    public EntityGeneral playerEntity;
    public Inventory playerInventory;
    public GameObject playerCam;

    float heldDownTime = 0f;

    public bool canUseItems = true;
    
    public Canvas playerCanvas;

    [SerializeField] Image playerHealthBar;
    [SerializeField] Image playerHealthAnim;

    [SerializeField] Image playerDeadScreen;

    [SerializeField] TMP_Text announceBarUI;
    public TMP_Text itemNameUI;
    public Transform heldItemAnchor;
    private SpriteRenderer heldItemSprite;

    

    public ItemSlotUI[] inventorySlotImages;
    public Image selectedSlotImage;
    
    private float interactCooldown = 0f;

    public Image mouseReplace;
    public string mouseMode = "world"; //inventory, dragging, cross, hide, world, consume(make transparent for now)
    [SerializeField] Sprite crossSprite;
    [SerializeField] Sprite inventoryMouseSprite;
    [SerializeField] Sprite draggingMouseSprite;
    private ItemState mouseItem;
    [SerializeField] Image mouseItemImage;
    private ItemSlotUI mouseItemOrigin;

    public EntityGeneral currentInteractable;
    public Container currentOpenContainer = null;
    public Transform containerUIParent;

    [SerializeField] GameObject playerItemThrowBar; //turn on when clicking chargable item
    [SerializeField] Image playerItemCharge;        //update based on item throwstuffs

    [SerializeField] GameObject fireStartIcon; 
    
    [SerializeField] Transform statusEffectIconsParent; 
    [SerializeField] GameObject statusEffectIcon; 

    void Start()
    {
        playerMovement = this.GetComponent<PlayerMovement>();
        playerEntity = this.GetComponent<EntityGeneral>();
        playerInventory = this.GetComponent<Inventory>();
        playerItemThrowBar.SetActive(false);
        heldItemSprite = heldItemAnchor.GetChild(0).GetComponent<SpriteRenderer>();
        Cursor.visible = false;
    }
    void Update()
    {
        ChangeSlots();
        CheckInteractions();
        CheckUseItems();
        UpdateInventoryVisual();
        HeldItemSpecifier();
        OrderStatusIcons();
        if (mouseItem != null)
        {
            MouseHoldingItem();
        }
        if (Input.GetKeyDown(KeyCode.G) && !enemyObj.activeSelf)
        {
            Announce("The Red Circle Is Out To Get You!",4.5f,new Color(0.84f,0.05f,0.08f,0.95f));
            enemyObj.SetActive(true);
        }
        //Restart
        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Stopping the game
            //For the love of god make a seperate UI class
            //..no
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            Vector3Int blockAbove = allSystems.mapManager.FloatToGridPos(this.transform.position+new Vector3(0f,1f,0f));
            allSystems.entitySummonSystem.BlockToRigidblock(blockAbove);
        }
        if (this.transform.position.y < -180f)
        {
            playerEntity.Damage(10f,"void");
        }
        playerHealthAnim.fillAmount += (playerHealthBar.fillAmount-playerHealthAnim.fillAmount)/10f;
        if (playerInventory.HeldItem() != null)
        {
            if (playerInventory.HeldItem().itemData.id == "fire_starter") //make this more general
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int mouseIntPos = allSystems.mapManager.FloatToGridPos(mousePos);
                if (playerInventory.HeldItem().GetBehaviour<BlockPlacerBehaviour>().CanFireStart(mouseIntPos,allSystems))
                {
                    //inst the thing
                    Debug.Log("do the fire");
                    Instantiate(fireStartIcon,mouseIntPos+new Vector3(0f,0f,-1f),Quaternion.identity);
                }
            }
        }
    }
    void FixedUpdate()
    {
        MouseIconReplace();
    }
    
    //Health
    void OnEnable()
    {
        playerEntity.EntityDamaged += GetHurt; //Subscribe and start to listen
        playerEntity.EntityHealed += GetHeal;
    }

    void OnDisable()
    {
        playerEntity.EntityDamaged -= GetHurt; //stop listening
        playerEntity.EntityHealed -= GetHeal;
    }
    public void GetHurt(EntityGeneral entity, float amount, string damageType)
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
    public void GetHeal(EntityGeneral entity, float amount)
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
            case "poking":
                return "How did you get poked to death??";
            case "fall":
                return "You found out I added fall damage to this game.";
            default:
                return "You died because of a bug? I don't know why you died...";
        }
    }

    /*------All About UI-------*/
    /*-Move This Somewhere Else*/

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
            ResetItemThrowBar();
            return;
        }
        if (GetMouseItem() != null) //Currently dragging an item
        {return;}
        if (Input.GetMouseButton(0) && canUseItems)   //Wait until player lets go
        {
            heldDownTime += Time.deltaTime;
            if (playerInventory.HeldItem().itemData.tags.Contains("charging")) //find held item see throwable tag
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

            ResetItemThrowBar();
        }
        if (Input.GetMouseButtonUp(1) && canUseItems) //Right Click for alternative item use
        {
            
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            playerInventory.ItemRightClick(mousePos,heldDownTime);
            ResetItemThrowBar();
        }
        if (canUseItems == false)
        {
            ResetItemThrowBar();
        }
    }
    void ResetItemThrowBar() 
    {
        heldDownTime = 0f;
        playerItemCharge.fillAmount = 0f;
        playerItemThrowBar.SetActive(false);
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
            inventorySlotImages[i].UpdateVisual();
        }
    }
    void MouseIconReplace() //On FixedUpdate
    {
        mouseReplace.transform.position = FindMousePos()+new Vector3(0f,0f,0.5f);
        //Change the icon maybe here, maybe at helditemspecifier
        UpdateMouseMode();
        switch (mouseMode)
        {
            case "hide":
                mouseReplace.enabled = false;
                break;
            case "consume":
                mouseReplace.enabled = false;
                break;
            case "cross":
                mouseReplace.enabled = true;
                mouseReplace.sprite = crossSprite;
                break;
            case "inventory":
                mouseReplace.enabled = true;
                mouseReplace.sprite = inventoryMouseSprite;
                break;
            case "dragging":
                mouseReplace.enabled = true;
                mouseReplace.sprite = draggingMouseSprite;
                break;
            default:
                mouseReplace.enabled = true;
                mouseReplace.sprite = crossSprite; //We made default into crossSprite for now
                break;
        }
    }
    public void HeldItemSpecifier() //The diegetic play holding showcase
    {
        if (playerInventory.HeldItem() != null) //if we're indeed holding something
        {
            //Write its name on the hotbar 
            itemNameUI.enabled = true;
            itemNameUI.text = playerInventory.HeldItem().itemData.itemName;
            
            //Put it diegetically on the player character
            heldItemSprite.enabled = true;
            heldItemSprite.sprite = playerInventory.HeldItem().itemData.sprite;
            if (playerInventory.HeldItem().itemData.tags.Contains("mouse_follow")) //Make it follow the player
            {
                Vector3 mouseDirVec = -(FindMousePos()-heldItemAnchor.position).normalized;
                float mouseDirAngle = Mathf.Atan2(-mouseDirVec.x,mouseDirVec.y)* Mathf.Rad2Deg-90f;
                //float transAngle = heldItemAnchor.eulerAngles.z+(mouseDirAngle-heldItemAnchor.eulerAngles.z)*Time.deltaTime;
                heldItemAnchor.eulerAngles = new Vector3(0f,0f,mouseDirAngle);
            }
            else
            {
                float itemHoldAngle = -5.27f;
                heldItemAnchor.eulerAngles = new Vector3(0f,0f,itemHoldAngle);
            }
        }
        else
        {
            itemNameUI.enabled = false;
            heldItemSprite.enabled = false;
        }
    }

    //Interactions
    public void CheckInteractions()
    {
        if (interactCooldown > 0f)
        {
            interactCooldown -= Time.deltaTime;
        }
        CheckInteractables(this.transform.position,playerEntity.entityReach);
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentOpenContainer != null)
            {
                currentOpenContainer.CloseInvCanvas(); //Closes the canvas if it's open
                interactCooldown = 0.2f;
            }
            if (currentInteractable != null && interactCooldown <= 0f)
            {
                if (currentInteractable.GetComponent<Container>())
                {
                    Container interactCont = currentInteractable.GetComponent<Container>();
                    interactCont.InteractedWith(this);
                }
                if (currentInteractable.GetComponent<ItemPickUp>())
                {
                    Debug.Log("item should be picked up");
                    ItemPickUp itemObj = currentInteractable.GetComponent<ItemPickUp>();
                    playerInventory.PickUpThisItem(itemObj);
                }
            }
        }
    }
    public void CheckInteractables(Vector2 where, float searchRadius)
    {
        Collider2D[] results = Physics2D.OverlapCircleAll(where,searchRadius);
        Debug.DrawLine(where- new Vector2(0f,searchRadius), where+ new Vector2(0f,searchRadius), Color.green,2f);
        Debug.DrawLine(where- new Vector2(searchRadius,0f), where+ new Vector2(searchRadius,0f), Color.green,2f);

        Collider2D closest = FindClosestInteractable(where, results); 
        if (closest == null)
        {
            DisableCurrentInteract();
            return; //No interactable nearby
        }
        if (closest.gameObject.GetComponent<EntityGeneral>() == false)
        {
            DisableCurrentInteract();
            return; //Not an entity somehow???
        }
        if (closest.gameObject.GetComponent<EntityGeneral>().interactable == false)
        {
            DisableCurrentInteract();
            return; //Not an interactable somehow???
        }
        DisableCurrentInteract();
        currentInteractable = closest.GetComponent<EntityGeneral>();
        currentInteractable.currentInteract = true;
        currentInteractable.ChangeMaterial(false);

    }/*
    public void CheckInteractables(Vector2 where, float searchRadius)
    {
        Collider2D[] results = Physics2D.OverlapCircleAll(where,searchRadius);
        
        Debug.DrawLine(where- new Vector2(0f,searchRadius), where+ new Vector2(0f,searchRadius), Color.green,2f);
        Debug.DrawLine(where- new Vector2(searchRadius,0f), where+ new Vector2(searchRadius,0f), Color.green,2f);

        if (results.Length > 0)
        {
            bool interactableInRange = false;
            foreach (var col in results)
            {
                if (col.gameObject.GetComponent<EntityGeneral>() == false)
                {
                    DisableCurrentInteract();
                    //interactableInRange = false;
                    continue;
                }
                if (col.gameObject.GetComponent<EntityGeneral>().interactable)
                {
                    interactableInRange = true;
                }
            }
            if (interactableInRange)
            {
                Collider2D closest = FindClosestInteractable(where, results); 
                DisableCurrentInteract();

                currentInteractable = closest.GetComponent<EntityGeneral>();
                currentInteractable.currentInteract = true;
                currentInteractable.ChangeMaterial(false);
            }
            else
            {
                DisableCurrentInteract();
            }
        }
        else if (currentInteractable != null)
        {
            DisableCurrentInteract();
        }
    }*/ //Old form
    void DisableCurrentInteract()
    {
        if (currentInteractable != null)
        {
            currentInteractable.currentInteract = false;
            currentInteractable.ChangeMaterial(false);
            currentInteractable = null;
        }
    }
    public Collider2D FindClosestInteractable(Vector2 origin, Collider2D[] colliders)
    {
        Collider2D colliderIte = null;
        foreach (var col in colliders)
        {    
            if (col.gameObject.GetComponent<EntityGeneral>() == false)
            {
                continue;
            }
            if (col.gameObject.GetComponent<EntityGeneral>().interactable == false)
            {
                continue;
            }
            //Anything that goes further is an interactable entity
            if (colliderIte == null) //if this is the first one
            {
                colliderIte = col;
            }
            else
            {
                float currentDist = Vector2.Distance(origin,(Vector2)colliderIte.transform.position);
                float iteDist = Vector2.Distance(origin,(Vector2)col.transform.position);
                if ((iteDist < currentDist)) //if this one is closer than the current one
                {
                    colliderIte = col;
                }
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

        UpdateMouseMode();
    }
    void UpdateMouseMode()
    {
        if (GetMouseItem() != null) //Item Dragging something
        {
            mouseMode = "dragging";
        }
        else if (currentOpenContainer != null) //Player in container
        {
            mouseMode = "inventory";
        }
        else if (playerInventory.HeldItem() == null) //Player not holding anything
        {
            mouseMode = "hide";
        }
        else //Player holding an item and not in a container and not dragging item
        {
            mouseMode = playerInventory.HeldItem().itemData.mouseMode;
        }
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
    //Status Icons
    public void MakeStatusIcon(IStatusEffect effect)
    {
        float offset = -1.6f;
        Vector3 pos = statusEffectIconsParent.position + new Vector3(4f,statusEffectIconsParent.childCount*offset,0f);
        GameObject madeIcon = Instantiate(statusEffectIcon,pos,Quaternion.identity,statusEffectIconsParent);
        madeIcon.GetComponent<StatusIcon>().effect = effect;
        madeIcon.GetComponent<StatusIcon>().behaviourAdder = allSystems.behaviourAdder;
    }
    void OrderStatusIcons()
    {
        if (statusEffectIconsParent.childCount <= 0) //no children
        {
            return;
        }
        float offset = -1.6f;
        for (int i = 0; statusEffectIconsParent.childCount > i; i++) //iterate over children
        {
            Vector3 pos = statusEffectIconsParent.position + new Vector3(0f,i*offset,0f);
            statusEffectIconsParent.GetChild(i).GetComponent<StatusIcon>().desiredPos = pos;
            //statusEffectIconsParent.GetChild(i).position = pos;
        }
    }
}
