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
        }
    }

    private void OnUIReload(
        UnityEngine.UIElements.PanelRenderer renderer,
        VisualElement rootElement,
        int version)
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
        }

        RefreshHotBar();
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
}
