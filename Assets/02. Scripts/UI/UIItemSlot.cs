using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItemSlot : MonoBehaviour
{
    protected InventorySlot inventorySlot;
    
    [SerializeField] protected Image itemIcon;
    [SerializeField] protected TextMeshProUGUI itemCountText;

    public void SetInventorySlot(InventorySlot inventorySlot)
    {
        this.inventorySlot = inventorySlot;

        if (inventorySlot.IsEmpty)
        {
            itemIcon.sprite = null;
            itemIcon.gameObject.SetActive(false);
            itemCountText.gameObject.SetActive(false);
        }
        else
        {
            itemIcon.gameObject.SetActive(true);
            itemIcon.sprite = inventorySlot.Item.Icon;

            itemCountText.gameObject.SetActive(true);
            itemCountText.text = inventorySlot.ItemCount.ToString();
        }
    }
}
