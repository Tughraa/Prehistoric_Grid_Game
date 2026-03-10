using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemBehaviour
{
    void Use(EntityGeneral owner, ItemState state, Vector3 mousePos, float heldFor, Inventory inventory, int slot);
    void RightClick(EntityGeneral owner, ItemState state, Vector3 mousePos, float heldFor, Inventory inventory, int slot);
    void StartHolding(EntityGeneral owner, ItemState state, Inventory inventory, int slot);
    void StopHolding(EntityGeneral owner, ItemState state, Inventory inventory, int slot);
}
