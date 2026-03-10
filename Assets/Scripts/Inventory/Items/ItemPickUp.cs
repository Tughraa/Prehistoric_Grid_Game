using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public ItemState storedItem;
    public bool itemEntity = true;
    public void GetPickedUp()
    {
        Destroy(this.gameObject);
    }
}
