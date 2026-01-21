using UnityEngine;

public class ItemTest : MonoBehaviour
{
    [SerializeField] private Item item;

    [ContextMenu("Use Item")]
    private void UseItem()
    {
        var clone = item.Clone() as Item;
        clone.Setup(GetComponent<Entity>());

        clone.Use(GetComponent<Entity>());
    }
}
