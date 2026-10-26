using Unity.Collections;
using UnityEngine;

public class PIckable : MonoBehaviour
{
    [Header("item")]
    [SerializeField] private ItemData itemData;
    [SerializeField] private int quantity =1;

    [Header("Behavior")]
    [SerializeField] private bool destroyOnPickUp = true;
    private bool _pickedUp = false;

    public ItemData ItemData => itemData;

    public int Quantity => quantity;

    public bool TryPickUp()
    {
        if (_pickedUp || itemData == null) return false;

        _pickedUp = true;
        OnPickedUp();
        return true;
    }

    private void OnPickedUp()
    {

        bool added = (InventoryManager.Instance.AddItem(itemData, quantity));
        if (added)
        {
            Debug.Log($"Recogido{itemData.Nombre} * {quantity}");
        if (destroyOnPickUp)
        Destroy(gameObject);
        }


        
    }
}
