using UnityEngine;
using UnityEngine.UIElements;

public class BarController : MonoBehaviour
{
    [SerializeField] private PanelRenderer panelRenderer;
    [SerializeField] private int hotbarSize = 5;

    private VisualElement[] slotElements;

    private void OnEnable()
    {
        if (panelRenderer != null)
        {
            panelRenderer.RegisterUIReloadCallback(OnUIReload);
        }
        else
        {
            Debug.LogError("Asigna el componente PanelRenderer en el Inspector.", this);
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged += RefreshHotBar;
        }
    }

    private void OnDisable()
    {
        if (panelRenderer != null)
        {
            panelRenderer.UnregisterUIReloadCallback(OnUIReload);
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged -= RefreshHotBar;
            InventoryManager.Instance.OnSelectedSlotChanged -= RefreshSelection;
        }

    }

    private void OnUIReload(UnityEngine.UIElements.PanelRenderer renderer, VisualElement rootElement, int version)
    {
        if (rootElement == null)
            return;

        VisualElement grid = rootElement.Q<VisualElement>("BarrGreed");
        if (grid == null)
        {
            Debug.LogError("No se encontró el elemento 'BarrGreed' en InventarioUI.uxml.", this);
            return;
        }

        int slotCount = Mathf.Min(hotbarSize, grid.childCount);
        slotElements = new VisualElement[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            slotElements[i] = grid.ElementAt(i);
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged -= RefreshHotBar;
            InventoryManager.Instance.OnInventoryChanged += RefreshHotBar;
            InventoryManager.Instance.OnSelectedSlotChanged -= RefreshSelection;
            InventoryManager.Instance.OnSelectedSlotChanged += RefreshSelection;
        }

        RefreshHotBar();
        RefreshSelection();
    }

    private void RefreshHotBar()
    {
        if (InventoryManager.Instance == null || slotElements == null)
            return;

        for (int i = 0; i < slotElements.Length; i++)
        {
            InventorySlot slotData = InventoryManager.Instance.GetSlot(i);
            VisualElement slotElement = slotElements[i];

            if (slotElement == null)
                continue;

            Image icon = slotElement.Q<Image>("item");
            Label quantityLabel = slotElement.Q<Label>("Quantity");
            if (icon == null || quantityLabel == null)
            {
                Debug.LogError("Cada slot debe contener una Image 'item' y un Label 'Quantity'.", this);
                continue;
            }

            if (slotData != null && slotData.itemData != null)
            {
                icon.sprite = slotData.itemData.icon;
                quantityLabel.text = slotData.quantity > 1 ? slotData.quantity.ToString() : "";
            }
            else
            {
                icon.sprite = null;
                quantityLabel.text = "";
            }
        }
    }

    private void RefreshSelection()
    {
        if (InventoryManager.Instance == null || slotElements == null)
            return;

        int selected = InventoryManager.Instance.SelectedSlot;

        for (int i = 0; i < slotElements.Length; i++)
        {
            VisualElement slotElement = slotElements[i];
            if (slotElement == null)
                continue;

            bool isSelected = i == selected;
            if (isSelected)
                slotElement.AddToClassList("Slot_Style1--Selected");
            else
                slotElement.RemoveFromClassList("Slot_Style1--Selected");

            float borderWidth = isSelected ? 4f : 0f;
            slotElement.style.borderTopWidth = borderWidth;
            slotElement.style.borderRightWidth = borderWidth;
            slotElement.style.borderBottomWidth = borderWidth;
            slotElement.style.borderLeftWidth = borderWidth;
        }
    }
}
