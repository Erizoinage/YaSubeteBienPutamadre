using UnityEngine;
using UnityEngine.InputSystem;

public class Input_BarController : MonoBehaviour
{
    [SerializeField] private InputActionReference hotBarSelectAction;

    void OnEnable()
    {
        hotBarSelectAction.action.Enable();
        hotBarSelectAction.action.performed += OnHotBarSelect;
    }

    void OnDisable()
    {
        hotBarSelectAction.action.Disable();
        hotBarSelectAction.action.performed -= OnHotBarSelect;
    }

    private void Update()
    {
        if (Mouse.current == null || InventoryManager.Instance == null)
            return;

        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll != 0f)
        {
            InventoryManager.Instance.SelectNextSlot(scroll > 0f ? -1 : 1);
        }
    }

    private void OnHotBarSelect(InputAction.CallbackContext context)
    {
        string keyName = context.control.name;

        if (int.TryParse(keyName,out int number)&& number >=1&& number<=5)
        {
            int slotIndex = number-1;
            InventoryManager.Instance.SelectSlot(slotIndex);
        }
    }



}
