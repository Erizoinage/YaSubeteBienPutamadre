using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [Header("Interfaz del inventario")]
    [SerializeField] private GameObject inventoryPanel;

    [Header("Jugador")]
    [SerializeField] private MovimientoJugador jugador;

    [Header("Slots")]
    [Min(1)]
    [SerializeField] private int cantidadSlots = 16;
    [SerializeField] private List<SlotInventario> slots = new List<SlotInventario>();
    private bool inventarioAbierto;
    public List<SlotInventario> Slots => slots;
    public bool InventarioAbierto => inventarioAbierto;

    [Header("Lo visual")]

    [SerializeField] private Transform contenedorSlots;
    [SerializeField] private SlotInventarioVisual prefabSlotVisual;

    private readonly List<SlotInventarioVisual> slotsVisuales = new List<SlotInventarioVisual>();


    private void Awake()
    {
        PrepararSlots();
        CrearSlotsVisuales();
    }

    private void Start()
    {
        inventarioAbierto = false;

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
    }

    private void PrepararSlots()
    {
        // Crea los slots que falten.
        while (slots.Count < cantidadSlots)
        {
            slots.Add(new SlotInventario());
        }

        // Elimina slots sobrantes solamente si están vacíos.
        while (slots.Count > cantidadSlots)
        {
            int ultimoIndice = slots.Count - 1;

            if (!slots[ultimoIndice].EstaVacio())
            {
                Debug.LogWarning("No se pudo reducir el inventario porque " +"uno de los slots contiene un objeto." );

                break;
            }

            slots.RemoveAt(ultimoIndice);
        }
    }

    // Esta función se conecta al Player Input.
    public void OnInventory(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        CambiarEstadoInventario();
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

    private void CrearSlotsVisuales()
    {
        if (contenedorSlots == null)
        {
            Debug.LogWarning("Falta asignar el contenedor de los slots.", this);
            return;
        }

        slotsVisuales.Clear();
        slotsVisuales.AddRange(
            contenedorSlots.GetComponentsInChildren<SlotInventarioVisual>(true)
        );

        if (slotsVisuales.Count < slots.Count &&
            prefabSlotVisual != null &&
            contenedorSlots.GetComponent<LayoutGroup>() != null)
        {
            while (slotsVisuales.Count < slots.Count)
            {
                SlotInventarioVisual nuevoSlotVisual =
                    Instantiate(prefabSlotVisual, contenedorSlots);

                slotsVisuales.Add(nuevoSlotVisual);
            }
        }

        if (slotsVisuales.Count < slots.Count)
        {
            Debug.LogWarning(
                "Hay " + slotsVisuales.Count + " slots visuales colocados, pero el inventario necesita "
                + slots.Count + ". Coloca los slots faltantes en el contenedor.",
                this
            );
        }

        int cantidadSlotsVisuales = Mathf.Min(slotsVisuales.Count, slots.Count);

        for (int i = 0; i < cantidadSlotsVisuales; i++)
        {
            slotsVisuales[i].AsignarSlot(slots[i]);
            slotsVisuales[i].AsignarInventoryManager(this);
        }
    }

    private void ActualizarSlotsVisuales()
    {
        foreach (SlotInventarioVisual slotVisual in slotsVisuales)
        {
            slotVisual.ActualizarVisual();
        }
    }

    public bool AgregarObjeto(DatosObjeto nuevoObjeto, int cantidadRecibida)
    {
        if (nuevoObjeto == null)
        {
            Debug.LogWarning("No se puede agregar un DatosObjeto vacío.");

            return false;
        }

        if (cantidadRecibida <= 0)
        {
            Debug.LogWarning( "La cantidad recibida debe ser mayor que cero.");

            return false;
        }

        // Primero comprueba si cabe toda la cantidad.
        // Así evitamos guardar una parte y duplicar el sobrante.
        if (!HayEspacioPara(nuevoObjeto, cantidadRecibida))
        {
            Debug.Log("No hay suficiente espacio para recoger " +cantidadRecibida + " x " +nuevoObjeto.nombreObjeto);
            
            return false;
        }

        int cantidadRestante = cantidadRecibida;

        // Primero llena los stacks existentes.
        if (nuevoObjeto.esStackeable)
        {
            foreach (SlotInventario slot in slots)
            {
                if (cantidadRestante <= 0)
                {
                    break;
                }

                if (slot.EstaVacio())
                {
                    continue;
                }

                if (!slot.TieneElMismoObjeto(nuevoObjeto))
                {
                    continue;
                }

                cantidadRestante = slot.AgregarObjeto(nuevoObjeto,cantidadRestante);
            }
        }

        // Después utiliza slots vacíos.
        foreach (SlotInventario slot in slots)
        {
            if (cantidadRestante <= 0)
            {
                break;
            }

            if (!slot.EstaVacio())
            {
                continue;
            }

            cantidadRestante = slot.AgregarObjeto(nuevoObjeto,cantidadRestante);
        }

        Debug.Log("Se guardó " +cantidadRecibida + " x " +nuevoObjeto.nombreObjeto);

        
        ActualizarSlotsVisuales();
        MostrarInventarioEnConsola();

        return cantidadRestante == 0;
    }

    private bool HayEspacioPara(DatosObjeto nuevoObjeto,int cantidadRecibida)
    {
        int espacioDisponible = 0;

        foreach (SlotInventario slot in slots)
        {
            if (slot.EstaVacio())
            {
                if (nuevoObjeto.esStackeable)
                {
                    espacioDisponible += nuevoObjeto.cantidadMaxima;
                }
                else
                {
                    espacioDisponible += 1;
                }
            }
            else if ( nuevoObjeto.esStackeable && slot.TieneElMismoObjeto(nuevoObjeto))
            {
                espacioDisponible +=nuevoObjeto.cantidadMaxima - slot.Cantidad;
            }

            if (espacioDisponible >= cantidadRecibida)
            {
                return true;
            }
        }

        return false;
    }

    private void MostrarInventarioEnConsola()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            SlotInventario slot = slots[i];

            if (slot.EstaVacio())
            {
                Debug.Log("Slot " + i + ": vacío");
                continue;
            }

            Debug.Log("Slot " + i + ": " +slot.Objeto.nombreObjeto +" x" + slot.Cantidad );
        }
    }

    private void OnValidate()
    {
        if (cantidadSlots < 1)
        {
            cantidadSlots = 1;
        }
    }
    public bool MoverObjeto(SlotInventario slotOrigen, SlotInventario slotDestino)
    {
     if (slotOrigen == null || slotDestino == null)
      {
        return false;
      }

     if (slotOrigen == slotDestino)
      {
        return false;
      }

     bool objetoMovido = slotOrigen.MoverA(slotDestino);

      if (objetoMovido)
       {
        ActualizarSlotsVisuales();
       }

     return objetoMovido;
    }
}