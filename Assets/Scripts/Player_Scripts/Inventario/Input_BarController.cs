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
