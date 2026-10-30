using UnityEngine;

public class ObjetoRecolectable : MonoBehaviour
{
    [Header("Datos del objeto")]
    [SerializeField] private DatosObjeto datosObjeto;

    [Header("Cantidad")]
    [Min(1)]
    [SerializeField] private int cantidad = 1;

    [Header("Inventario")]
    [SerializeField] private InventoryManager inventoryManager;

    private bool recogido;

    private void Awake()
    {
        // Busca el inventario automáticamente si no fue asignado.
        if (inventoryManager == null)
        {
            inventoryManager = FindFirstObjectByType<InventoryManager>();
        }
    }

    public void Recolectar()
    {
        // Evita recoger el mismo objeto dos veces.
        if (recogido)
        {
            return;
        }

        if (datosObjeto == null)
        {
            Debug.LogWarning(
                gameObject.name + " no tiene DatosObjeto asignado."
            );

            return;
        }

        if (inventoryManager == null)
        {
            Debug.LogWarning(
                "No se encontró un InventoryManager en la escena."
            );

            return;
        }

        bool agregado = inventoryManager.AgregarObjeto(
            datosObjeto,
            cantidad
        );

        // Solo elimina el objeto si se guardó correctamente.
        if (agregado)
        {
            recogido = true;

            Debug.Log(
                "Se recogió " +
                cantidad + " x " +
                datosObjeto.nombreObjeto
            );

            Destroy(gameObject);
        }
    }

    private void OnValidate()
    {
        if (cantidad < 1)
        {
            cantidad = 1;
        }

        // Un objeto no stackeable solo puede recogerse de uno en uno.
        if (datosObjeto != null && !datosObjeto.esStackeable)
        {
            cantidad = 1;
        }
    }
}