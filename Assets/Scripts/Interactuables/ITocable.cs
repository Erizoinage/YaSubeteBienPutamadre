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

    [Header("Información")]
    [SerializeField] private CosasInteractuables tipo;
    [SerializeField] private string nombreObjeto;

    [Header("Eventos")]
    [SerializeField] private UnityEvent alInteractuar;
    [SerializeField] private UnityEvent alSalirDeLaZona;

    public CosasInteractuables Tipo => tipo;
    public string NombreObjeto => nombreObjeto;

    public void Interactuar()
    {
        Debug.Log( "Interactuando con: " + nombreObjeto + " | Tipo: " + tipo );

        alInteractuar?.Invoke();
    }

    public void NotificarSalidaDeZona()
    {
        alSalirDeLaZona?.Invoke();
    }
}