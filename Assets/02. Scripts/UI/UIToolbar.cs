using UnityEngine;
using System.Collections.Generic;
public class UIToolbar : MonoBehaviour
{
    [SerializeField] private List<UIToolbarSlot> toolbarSlots;

    private PlayerInventory player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
    }
    
    void OnEnable()
    {
        player.onChangedToolbarItemSlot += SetItemSlot;
    }

    void OnDisable()
    {
        player.onChangedToolbarItemSlot -= SetItemSlot;
    }

    private void SetItemSlot(InventorySlot inventorySlot, int index)
    {
        toolbarSlots[index].SetInventorySlot(inventorySlot);
    }
}
