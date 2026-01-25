using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIInventory : CanvasPanel
{
    [SerializeField] private List<UIInventorySlot> toolbarSlots;
    [SerializeField] private List<UIInventorySlot> inventorySlots;

    private PlayerInventory playerInventory;

    public override void Initialize()
    {
        playerInventory = GameObject.FindFirstObjectByType<PlayerInventory>();
    }

    public override void Release()
    {
        
    }
}
