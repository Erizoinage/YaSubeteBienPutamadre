using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    [Header("Panel del inventario")]
    [SerializeField] private GameObject inventoryPanel;

    [Header("Movimiento del jugador")]
    [SerializeField] private MovimientoJugador jugador;

    [Header("Objetos almacenados")]
    [SerializeField]
    private List<ObjetoGuardado> objetosGuardados
        = new List<ObjetoGuardado>();

    private bool inventarioAbierto;

    private void Start()
    {
        inventarioAbierto = false;

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            CambiarEstadoInventario();
        }
    }

    public void CambiarEstadoInventario()
    {
        inventarioAbierto = !inventarioAbierto;

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(inventarioAbierto);
        }

        if (jugador == null)
        {
            return;
        }

        if (inventarioAbierto)
        {
            jugador.DesactivarMovimiento();
        }
        else
        {
            jugador.ActivarMovimiento();
        }
    }

    public bool AgregarObjeto(DatosObjeto datosObjeto, int cantidad)
    {
        if (datosObjeto == null || cantidad <= 0)
        {
            return false;
        }

        // Busca si el objeto ya existe en el inventario.
        foreach (ObjetoGuardado objeto in objetosGuardados)
        {
            if (objeto.datosObjeto == datosObjeto)
            {
                objeto.cantidad += cantidad;

                Debug.Log(
                    "Se agregaron " + cantidad + " unidades de "
                    + datosObjeto.nombreObjeto
                );

                return true;
            }
        }

        // Si todavía no existe, crea una nueva entrada.
        ObjetoGuardado nuevoObjeto = new ObjetoGuardado();

        nuevoObjeto.datosObjeto = datosObjeto;
        nuevoObjeto.cantidad = cantidad;

        objetosGuardados.Add(nuevoObjeto);

        Debug.Log(
            "Se recogió " + datosObjeto.nombreObjeto
            + ". Cantidad: " + cantidad
        );

        return true;
    }
}

[System.Serializable]
public class ObjetoGuardado
{
    public DatosObjeto datosObjeto;
    public int cantidad;
}