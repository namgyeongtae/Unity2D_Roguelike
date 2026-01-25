using UnityEngine;

public enum InventorySlotType
{
    Toolbar,
    Inventory
}

public class InventorySlot
{
    private Item item;
    private int itemCount;
    private InventorySlotType slotType;

    public bool IsEmpty => item == null;

    public Item Item => item;
    public int ItemCount => itemCount;
    public InventorySlotType SlotType => slotType;

    public InventorySlot(Item item, InventorySlotType slotType, int itemCount = 0)
    {
        if (item == null)
        {
            this.item = null;
            this.itemCount = 0;
        }
        else
        {
            this.item = item;
            this.itemCount = itemCount;
        }

        this.slotType = slotType;
    }

    public void AddItem(Item newItem, int count)
    {
        if (!IsEmpty)
        {
            if (item.ID == newItem.ID)
                itemCount += count;
        }
        else
        {
            item = newItem.Clone() as Item;
            itemCount = count;
        }
    }

    public void RemoveItem()
    {
        if (!IsEmpty)
        {
            item = null;
            itemCount = 0;
        }
    }

    public void SubtractItem(int count)
    {
        if (itemCount < count)
        {
            Debug.LogError($"Item {item.CodeName} has only {itemCount} items");
            return;
        }

        if (!IsEmpty)
        {
            itemCount -= count;
            if (itemCount <= 0)
            {
                RemoveItem();
            }
        }
    }
}
