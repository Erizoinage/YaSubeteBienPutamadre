using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class FullInventoryUIController : MonoBehaviour
{
    [SerializeField] private PanelRenderer panelRenderer;
    [SerializeField] private InputActionReference toggleInventory;
    [SerializeField] private PlayerInput playerInput;

    private VisualElement inventoryPanel;
    private VisualElement hotbarGrid;
    private readonly List<VisualElement> slotElements = new List<VisualElement>();
    private bool isOpen;

    private void OnEnable()
    {
        if (toggleInventory != null)
        {
            toggleInventory.action.performed += OnToggleInventory;
            toggleInventory.action.Enable();
        }
        else
        {
            Debug.LogError("Asigna la acción ToggleInventory en el Inspector.", this);
        }

        if (panelRenderer != null)
        {
            panelRenderer.RegisterUIReloadCallback(OnUIReload);
        }
        else
        {
            Debug.LogError("Asigna el componente PanelRenderer en el Inspector.", this);
        }

        SubscribeToInventoryChanges();
    }

    private void OnDisable()
    {
        if (toggleInventory != null)
        {
            toggleInventory.action.performed -= OnToggleInventory;
            toggleInventory.action.Disable();
        }

        if (panelRenderer != null)
            panelRenderer.UnregisterUIReloadCallback(OnUIReload);

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged -= RefreshInventory;
            if (isOpen)
                InventoryManager.Instance.SetFullInventoryOpen(false);
        }

        isOpen = false;
    }

    private void OnUIReload(PanelRenderer renderer, VisualElement rootElement, int version)
    {
        if (rootElement == null)
            return;

        inventoryPanel = rootElement.Q<VisualElement>("inventory-panel");
        hotbarGrid = rootElement.Q<VisualElement>("BarrGreed");
        VisualElement inventoryGrid = rootElement.Q<VisualElement>("Inventory-grid-full");

        if (inventoryPanel == null || hotbarGrid == null || inventoryGrid == null)
        {
            Debug.LogError("No se encontraron 'inventory-panel', 'BarrGreed' o 'Inventory-grid-full' en la UI.", this);
            return;
        }

        slotElements.Clear();
        foreach (VisualElement child in inventoryGrid.Children())
        {
            if (child.name == "Slot")
                slotElements.Add(child);
        }

        SubscribeToInventoryChanges();
        ApplyVisibility();
        RefreshInventory();
    }

    private void SubscribeToInventoryChanges()
    {
        if (InventoryManager.Instance == null)
            return;

        InventoryManager.Instance.OnInventoryChanged -= RefreshInventory;
        InventoryManager.Instance.OnInventoryChanged += RefreshInventory;
    }

    private void OnToggleInventory(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        isOpen = !isOpen;
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.SetFullInventoryOpen(isOpen);
        }
        else
        {
            Debug.LogError("No se encontró el InventoryManager para controlar el movimiento del jugador.", this);
        }

        ApplyVisibility();
    }

    private void ApplyVisibility()
    {
        if (inventoryPanel != null)
            inventoryPanel.style.display = isOpen ? DisplayStyle.Flex : DisplayStyle.None;

        if (hotbarGrid != null)
            hotbarGrid.style.display = isOpen ? DisplayStyle.None : DisplayStyle.Flex;

        UnityEngine.Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        UnityEngine.Cursor.visible = isOpen;
    }

    private void RefreshInventory()
    {
        if (InventoryManager.Instance == null || slotElements.Count == 0)
            return;

        const int columns = 5;
        int totalRows = Mathf.CeilToInt((float)slotElements.Count / columns);

        for (int i = 0; i < slotElements.Count; i++)
        {
            int row = i / columns;
            int column = i % columns;
            int inventoryIndex = (totalRows - 1 - row) * columns + column;

            VisualElement slot = slotElements[i];
            Image icon = slot.Q<Image>("item");
            Label quantity = slot.Q<Label>("Quantity");
            if (icon == null || quantity == null)
            {
                Debug.LogError("Cada ranura del inventario debe contener una Image 'item' y un Label 'Quantity'.", this);
                continue;
            }

            InventorySlot slotData = InventoryManager.Instance.GetSlot(inventoryIndex);
            if (slotData != null && slotData.itemData != null)
            {
                icon.sprite = slotData.itemData.icon;
                icon.style.visibility = Visibility.Visible;
                quantity.text = slotData.quantity > 1 ? slotData.quantity.ToString() : "";
            }
            else
            {
                icon.sprite = null;
                icon.style.visibility = Visibility.Hidden;
                quantity.text = "";
            }
        }
    }
}
