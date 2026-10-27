using UnityEngine;

public class EnMano_Item : MonoBehaviour
{
    [SerializeField] private Transform Mano;
    private GameObject _currentSprite;

    private void Start()
    {
        InventoryManager.Instance.OnSelectedSlotChanged += RefreshManoItem;
        RefreshManoItem();
    }
    private void OnDisable()
    {
        if(InventoryManager.Instance != null)
        InventoryManager.Instance.OnSelectedSlotChanged -= RefreshManoItem;
    }
    private void RefreshManoItem()
    {
        if(_currentSprite != null)
        {
            Destroy(_currentSprite);
            _currentSprite = null;
        }
        InventorySlot selected = InventoryManager.Instance.GetSlot(InventoryManager.Instance.SelectedSlot);

        if (selected == null || selected.itemData.SpriteEnMano == null)
            return;

        _currentSprite = Instantiate(selected.itemData.SpriteEnMano,Mano);
        _currentSprite.transform.localPosition = Vector3.zero;
        _currentSprite.transform.localRotation = Quaternion.identity;
    }
}
