using UnityEngine;


[System.Serializable]
public class InventorySlot 
{
    public ItemData itemData;
    public int quantity;

    public InventorySlot(ItemData data, int qty)
    {
        itemData = data;
        quantity = qty;
    }
}
