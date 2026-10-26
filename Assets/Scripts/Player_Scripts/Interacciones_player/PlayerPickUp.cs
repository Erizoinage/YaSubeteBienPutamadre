using UnityEngine;

public class PlayerPickUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PIckable pickable = other.GetComponentInParent<PIckable>();
        if (pickable != null)
        {
            pickable.TryPickUp();
        }
    }
}
