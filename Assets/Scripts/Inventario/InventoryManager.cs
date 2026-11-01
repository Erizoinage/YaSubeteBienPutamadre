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

    [Header("Slots Normales")]
    [Min(1)]
    [SerializeField] private int cantidadSlots = 16;
    [SerializeField] private List<SlotInventario> slots = new List<SlotInventario>();
    private bool inventarioAbierto;
    public List<SlotInventario> Slots => slots;
    public bool InventarioAbierto => inventarioAbierto;

    [Header("Slots Especiales")]
    [SerializeField] private List<SlotEspecial> slotEspeciales = new List<SlotEspecial>();

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
        List<SlotInventarioVisual> visualesLibres = new List<SlotInventarioVisual>(
            contenedorSlots.GetComponentsInChildren<SlotInventarioVisual>(true)
        );
        HashSet<SlotInventarioVisual> visualesReservados = new HashSet<SlotInventarioVisual>();
        HashSet<SlotInventarioVisual> visualesAsignados = new HashSet<SlotInventarioVisual>();

        foreach (SlotEspecial slotEspecial in slotEspeciales)
        {
            if (slotEspecial != null && slotEspecial.VisualSlot != null)
            {
                visualesReservados.Add(slotEspecial.VisualSlot);
            }
        }

        visualesLibres.RemoveAll(visual => visualesReservados.Contains(visual));

        for (int i = 0; i < slots.Count; i++)
        {
            SlotInventarioVisual visual = ObtenerVisualDisponible(visualesLibres);
            if (visual == null)
            {
                visual = CrearSlotVisual();
            }

            if (visual == null)
            {
                Debug.LogWarning("No hay un slot visual disponible para el slot normal " + i + ".", this);
                continue;
            }

            AsignarSlotVisual(visual, slots[i], visualesAsignados);
        }

        for (int i = 0; i < slotEspeciales.Count; i++)
        {
            SlotEspecial slotEspecial = slotEspeciales[i];
            if (slotEspecial == null)
            {
                Debug.LogWarning("El slot especial " + i + " no está asignado.", this);
                continue;
            }

            SlotInventarioVisual visual = slotEspecial.VisualSlot;
            if (visual == null)
            {
                visual = ObtenerVisualDisponible(visualesLibres);
                if (visual == null)
                {
                    visual = CrearSlotVisual();
                }
            }

            if (visual == null)
            {
                Debug.LogWarning(
                    "Asigna un slot visual a la entrada " + i + " de Slots Especiales, " +
                    "o agrega un slot visual al contenedor.",
                    this
                );
                continue;
            }

            AsignarSlotVisual(visual, slotEspecial, visualesAsignados);
        }
    }

    private SlotInventarioVisual ObtenerVisualDisponible(List<SlotInventarioVisual> visualesLibres)
    {
        if (visualesLibres.Count == 0)
        {
            return null;
        }

        SlotInventarioVisual visual = visualesLibres[0];
        visualesLibres.RemoveAt(0);
        return visual;
    }

    private SlotInventarioVisual CrearSlotVisual()
    {
        if (prefabSlotVisual == null || contenedorSlots.GetComponent<LayoutGroup>() == null)
        {
            return null;
        }

        return Instantiate(prefabSlotVisual, contenedorSlots);
    }

    private void AsignarSlotVisual(
        SlotInventarioVisual visual,
        SlotInventario slot,
        HashSet<SlotInventarioVisual> visualesAsignados
    )
    {
        if (!visualesAsignados.Add(visual))
        {
            Debug.LogWarning("Un slot visual está asignado a más de un slot de inventario.", visual);
            return;
        }

        visual.AsignarSlot(slot);
        visual.AsignarInventoryManager(this);
        slotsVisuales.Add(visual);
    }

    private void ActualizarSlotsVisuales()
    {
        foreach (SlotInventarioVisual slotVisual in slotsVisuales)
        {
            slotVisual.ActualizarVisual();
        }
    }

    public bool AgregarObjeto(DatosObjeto_Base nuevoObjeto, int cantidadRecibida)
    {
        if (nuevoObjeto == null)
        {
            Debug.LogWarning("No se puede agregar un DatosObjeto_Base vacío.");

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
            foreach (SlotInventario slot in ObtenerTodosLosSlots())
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

        foreach (SlotInventario slot in ObtenerTodosLosSlots())
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

    private bool HayEspacioPara(DatosObjeto_Base nuevoObjeto,int cantidadRecibida)
    {
        int espacioDisponible = 0;

        foreach (SlotInventario slot in ObtenerTodosLosSlots())
        {
            if (slot.EstaVacio() && slot.PuedeGuardar(nuevoObjeto))
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
            else if (nuevoObjeto.esStackeable &&
                     slot.TieneElMismoObjeto(nuevoObjeto) &&
                     slot.PuedeGuardar(nuevoObjeto))
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

    private IEnumerable<SlotInventario> ObtenerTodosLosSlots()
    {
        foreach (SlotInventario slot in slots)
        {
            yield return slot;
        }

        foreach (SlotEspecial slot in slotEspeciales)
        {
            yield return slot;
        }
    }

    private void MostrarInventarioEnConsola()
    {
        int indice = 0;
        foreach (SlotInventario slot in ObtenerTodosLosSlots())
        {
            if (slot.EstaVacio())
            {
                Debug.Log("Slot " + indice + ": vacío");
                indice++;
                continue;
            }

            Debug.Log("Slot " + indice + ": " +slot.Objeto.nombreObjeto +" x" + slot.Cantidad );
            indice++;
        }
    }

    private void OnValidate()
    {
        if (cantidadSlots < 1)
        {
            cantidadSlots = 1;
        }
    }
    public bool MoverObjeto( SlotInventario slotOrigen,SlotInventario slotDestino)
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