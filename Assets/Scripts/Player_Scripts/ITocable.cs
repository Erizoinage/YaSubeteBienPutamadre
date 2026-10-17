using UnityEngine;
using UnityEngine.Events;

public class ITocable : MonoBehaviour
{
    public enum CosasInteractuables
    {
        NPC,
        Cofre,
        Puerta,
        ObjRecogible,
        Tienda
    }
    [SerializeField] private CosasInteractuables tipo;
    [SerializeField] private string nombreObjeto;
    [SerializeField] private UnityEvent alInteractuar;

    public CosasInteractuables Tipo => tipo;
    public string NombreObjeto => nombreObjeto;

    public void Interactuar()
    {
        Debug.Log(
            "Interactuando con: " +
            nombreObjeto +
            " | Tipo: " +
            tipo
        );

        alInteractuar?.Invoke();
    }
}