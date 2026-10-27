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

    public void SelectNextSlot(int direction)
    {
        if (slots == null || slots.Length == 0 || direction == 0) return;

        int nextSlot = (SelectedSlot + direction + slots.Length) % slots.Length;
        SelectSlot(nextSlot);
    }

    public void DropSelectedItem(Vector3 playerPosition, Vector2 dropDirection)
    {
        if (slots == null || SelectedSlot < 0 || SelectedSlot >= slots.Length)
            return;

        InventorySlot selectedSlot = slots[SelectedSlot];
        if (selectedSlot == null || selectedSlot.itemData == null || selectedSlot.quantity <= 0)
            return;

        if (selectedSlot.itemData.icon == null)
        {
            Debug.LogError($"El objeto '{selectedSlot.itemData.Nombre}' no tiene un icono para representarlo en el mundo.", this);
            return;
        }

        if (dropDirection.sqrMagnitude == 0f)
            dropDirection = Vector2.down;

        Vector3 dropPosition = playerPosition + (Vector3)(dropDirection.normalized * 2.5f);
        GameObject droppedObject = new GameObject($"Dropped {selectedSlot.itemData.Nombre}");
        droppedObject.transform.position = dropPosition;

        SpriteRenderer spriteRenderer = droppedObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = selectedSlot.itemData.icon;

        BoxCollider2D pickupCollider = droppedObject.AddComponent<BoxCollider2D>();
        pickupCollider.isTrigger = true;

        PIckable pickable = droppedObject.AddComponent<PIckable>();
        pickable.Initialize(selectedSlot.itemData, 1);

        selectedSlot.quantity--;
        if (selectedSlot.quantity == 0)
            slots[SelectedSlot] = null;

        OnInventoryChanged?.Invoke();
    }
}
