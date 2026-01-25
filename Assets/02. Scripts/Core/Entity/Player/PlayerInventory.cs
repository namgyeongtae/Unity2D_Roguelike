using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public delegate void OnChangedItemSlotHandler(InventorySlot inventorySlot, int index);

    private readonly int TOOLBAR_SLOT_COUNT = 12;
    private readonly int INVENTORY_SLOT_COUNT = 24;

    private List<InventorySlot> toolbarItems = new();
    private List<InventorySlot> inventoryItems = new();

    public bool IsFull => IsFullToolbar && IsFullInventory;
    public bool IsFullToolbar => toolbarItems.Count(x => !x.IsEmpty) >= TOOLBAR_SLOT_COUNT;
    public bool IsFullInventory => inventoryItems.Count(x => !x.IsEmpty) >= INVENTORY_SLOT_COUNT;

    public event OnChangedItemSlotHandler onChangedToolbarItemSlot;
    public event OnChangedItemSlotHandler onChangedInventoryItemSlot;
    
    void Start()
    {
        for(int i = 0; i < TOOLBAR_SLOT_COUNT; i++)
        {
            var slot = new InventorySlot(null, InventorySlotType.Toolbar);
            toolbarItems.Add(slot);
        }
        for (int i = 0; i < INVENTORY_SLOT_COUNT; i++)
        {
            var slot = new InventorySlot(null, InventorySlotType.Inventory);
            inventoryItems.Add(slot);
        }
    }

    public void AddItem(Item item, int count)
    {
        if (IsFull)
        {
            Debug.LogError("Inventory is full");
            return;
        }
    
        InventorySlot targetSlot = null;

        targetSlot = (!IsFullToolbar) ? 
                    toolbarItems.FirstOrDefault(slot => slot.IsEmpty) : 
                    inventoryItems.FirstOrDefault(slot => slot.IsEmpty);

        if (targetSlot != null)
        {
            targetSlot.AddItem(item, count);
            OnChangedItemSlot(targetSlot, toolbarItems.IndexOf(targetSlot));
        }
    }

    public void RemoveItem(Item item, int count = 0)
    {
        InventorySlot targetSlot = null;

        targetSlot = toolbarItems.FirstOrDefault(slot => slot.Item.ID == item.ID) ?? 
                     inventoryItems.FirstOrDefault(slot => slot.Item.ID == item.ID) ?? null;
        
        if (targetSlot != null)
        {
            targetSlot.SubtractItem(count);

            OnChangedItemSlot(targetSlot, toolbarItems.IndexOf(targetSlot));
        }
    }

    public void RemoveAllItem(Item item)
    {
        var targetSlots = toolbarItems.Where(slot => (!slot.IsEmpty && slot.Item.ID == item.ID)).ToList()
                    ?? inventoryItems.Where(slot => (!slot.IsEmpty && slot.Item.ID == item.ID)).ToList();
        targetSlots.ForEach(slot => slot.RemoveItem());
    }

    public void UseItem(int index)
    {
        if (index < 0 || index >= TOOLBAR_SLOT_COUNT)
        {
            Debug.LogError("Invalid index");
            return;
        }
        
        var slot = toolbarItems[index];
        if (slot.IsEmpty)
        {
            Debug.Log("Item is Empty");
            return;
        }

        slot.Item.TryUseItem(GetComponent<Entity>());

        RemoveItem(slot.Item, 1);
    }

    private void OnChangedItemSlot(InventorySlot slot, int index)
    {
        if (slot.SlotType == InventorySlotType.Toolbar)
        {
            onChangedToolbarItemSlot?.Invoke(slot, index);
        }
        else
        {
            onChangedInventoryItemSlot?.Invoke(slot, index);
        }
    }


    #region Test Methods

    [ContextMenu("Add Test Item")]
    private void AddTestItem()
    {
        var item = Resources.Load<Item>("Item/ITEM_HEAL_POTION");
        AddItem(item, 1);
    }
    [ContextMenu("Remove Test Item")]
    private void RemoveTestItem()
    {
        var item = Resources.Load<Item>("Item/ITEM_HEAL_POTION");

        RemoveItem(item, 1);
    }

    #endregion
}