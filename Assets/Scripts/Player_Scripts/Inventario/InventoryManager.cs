using System;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private int inventorySize = 5;
    private InventorySlot[] slots;

    public Action OnInventoryChanged;
    public int SelectedSlot { get; private set; } = 0;
    public Action OnSelectedSlotChanged;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        slots = new InventorySlot[inventorySize];
    }

    public bool AddItem(ItemData itemData, int quantity)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && slots[i].itemData == itemData)
            {
                slots[i].quantity += quantity;
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                slots[i] = new InventorySlot(itemData, quantity);
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        return false;
    }

    public InventorySlot GetSlot(int index)
    {
        if (index < 0 || index >= slots.Length) return null;
        return slots[index];
    }

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= slots.Length) return;
        if (SelectedSlot == index) return;

        SelectedSlot = index;
        OnSelectedSlotChanged?.Invoke();
    }
}
