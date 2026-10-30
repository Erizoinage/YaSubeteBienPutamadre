using System;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private int inventorySize = 5;
    [SerializeField] private MovimientoJugador playerMovement;
    private InventorySlot[] slots;
    private bool fullInventoryOpen;
    private bool playerMovementWasActive;

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

    public void SetFullInventoryOpen(bool isOpen)
    {
        if (fullInventoryOpen == isOpen)
            return;

        fullInventoryOpen = isOpen;
        if (playerMovement == null)
        {
            Debug.LogError("Asigna el componente MovimientoJugador en el InventoryManager.", this);
            return;
        }

        if (isOpen)
        {
            playerMovementWasActive = playerMovement.EstaActivoElMovimiento();
            playerMovement.DesactivarMovimiento();
        }
        else
        {
            if (playerMovementWasActive)
                playerMovement.ActivarMovimiento();

            playerMovementWasActive = false;
        }
    }

    public bool AddItem(ItemData itemData, int quantity)
    {
        if (itemData == null || quantity <= 0 || slots == null)
            return false;

        int maxStackSize = Mathf.Max(1, itemData.maxStackSize);
        int availableSpace = 0;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
                availableSpace += maxStackSize;
            else if (slots[i].itemData == itemData)
                availableSpace += Mathf.Max(0, maxStackSize - slots[i].quantity);
        }

        if (availableSpace < quantity)
            return false;

        int remaining = quantity;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && slots[i].itemData == itemData)
            {
                int amountToAdd = Mathf.Min(remaining, maxStackSize - slots[i].quantity);
                slots[i].quantity += amountToAdd;
                remaining -= amountToAdd;
                if (remaining == 0)
                    break;
            }
        }

        for (int i = 0; i < slots.Length && remaining > 0; i++)
        {
            if (slots[i] != null)
                continue;

            int amountToAdd = Mathf.Min(remaining, maxStackSize);
            slots[i] = new InventorySlot(itemData, amountToAdd);
            remaining -= amountToAdd;
        }

        OnInventoryChanged?.Invoke();
        return true;
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

        int hotbarSize = Mathf.Min(5, slots.Length);
        int nextSlot = (SelectedSlot + direction + hotbarSize) % hotbarSize;
        SelectSlot(nextSlot);
    }

    public void DropSelectedItem(Vector3 playerPosition, Vector2 dropDirection)
    {
        DropItemFromSlot(SelectedSlot, playerPosition, dropDirection, 1);
    }

    public bool MoveItem(int fromIndex, int toIndex)
    {
        if (slots == null || fromIndex < 0 || fromIndex >= slots.Length ||
            toIndex < 0 || toIndex >= slots.Length || fromIndex == toIndex)
            return false;

        InventorySlot source = slots[fromIndex];
        if (source == null || source.itemData == null)
            return false;

        InventorySlot destination = slots[toIndex];
        if (destination == null)
        {
            slots[toIndex] = source;
            slots[fromIndex] = null;
        }
        else if (destination.itemData == source.itemData)
        {
            int maxStackSize = Mathf.Max(1, source.itemData.maxStackSize);
            int availableSpace = Mathf.Max(0, maxStackSize - destination.quantity);
            if (availableSpace == 0)
            {
                slots[toIndex] = source;
                slots[fromIndex] = destination;
            }
            else
            {
                int amountToMove = Mathf.Min(source.quantity, availableSpace);
                destination.quantity += amountToMove;
                source.quantity -= amountToMove;
                if (source.quantity == 0)
                    slots[fromIndex] = null;
            }
        }
        else
        {
            slots[toIndex] = source;
            slots[fromIndex] = destination;
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool DropItemFromSlot(int slotIndex, Vector3 playerPosition, Vector2 dropDirection, int quantity)
    {
        if (slots == null || slotIndex < 0 || slotIndex >= slots.Length)
            return false;

        InventorySlot selectedSlot = slots[slotIndex];
        if (selectedSlot == null || selectedSlot.itemData == null ||
            selectedSlot.quantity <= 0 || quantity <= 0)
            return false;

        quantity = Mathf.Min(quantity, selectedSlot.quantity);
        if (selectedSlot.itemData.icon == null)
        {
            Debug.LogError($"El objeto '{selectedSlot.itemData.Nombre}' no tiene un icono para representarlo en el mundo.", this);
            return false;
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
        pickable.Initialize(selectedSlot.itemData, quantity);

        selectedSlot.quantity -= quantity;
        if (selectedSlot.quantity == 0)
            slots[slotIndex] = null;

        OnInventoryChanged?.Invoke();
        return true;
    }
}
