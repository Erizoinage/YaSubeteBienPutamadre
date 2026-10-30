using UnityEngine;

[CreateAssetMenu(
    fileName = "NuevoObjeto",
    menuName = "Inventario/Objeto"
)]
public class DatosObjeto : ScriptableObject
{
    [Header("Informacion")]
    public string nombreObjeto;

    [TextArea]
    public string descripcion;

    [Header("Icono")]
    public Sprite icono;

    [Header("Apilamiento")]
    public bool esStackeable;

    [Min(1)]
    public int cantidadMaxima = 1;

    private void OnValidate()
    {
        // Si el objeto no es stackeable,
        // solamente puede haber uno por slot.
        if (!esStackeable)
        {
            cantidadMaxima = 1;
        }

        // Evita cantidades de cero o negativas.
        if (cantidadMaxima < 1)
        {
            cantidadMaxima = 1;
        }
    }
}