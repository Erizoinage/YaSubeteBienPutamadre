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
        // Busca automáticamente el inventario si no fue asignado.
        if (inventoryManager == null)
        {
            inventoryManager = FindAnyObjectByType<InventoryManager>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Solo se recoge si el objeto que entró tiene la etiqueta Player.
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        Recolectar();
    }

    private void Recolectar()
    {
        // Evita que el objeto se recolecte más de una vez.
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

        // Solo destruye el objeto si el inventario lo recibió.
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

        // Los objetos no stackeables se recogen de uno en uno.
        if (datosObjeto != null && !datosObjeto.esStackeable)
        {
            cantidad = 1;
        }
    }
}