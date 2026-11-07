using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControladorCombateInicio : MonoBehaviour
{
    [Header("Interfaz")]
    [SerializeField] private GameObject panelCombate;
    [SerializeField] private GameObject panelInventario;

    [Header("Sistemas")]
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private MovimientoJugador movimientoJugador;
    private PlayerInput entradaJugador;

    [Header("Estado")]
    [SerializeField] private bool combateActivo;
    [SerializeField] private bool movimientoJugadorBloqueado;

    private readonly List<SlotEspecial> slotsConsumibles =
        new List<SlotEspecial>();

    private readonly List<SlotEspecial> slotsAtaques =
        new List<SlotEspecial>();

    private SlotEspecial slotCasco;
    private SlotEspecial slotPechera;
    private SlotEspecial slotAmuleto;
    private bool activarEntradaAlFinalizarCombate;

    public bool CombateActivo => combateActivo;
    public bool MovimientoJugadorBloqueado => movimientoJugadorBloqueado;

    public IReadOnlyList<SlotEspecial> SlotsConsumibles => slotsConsumibles;
    public IReadOnlyList<SlotEspecial> SlotsAtaques =>slotsAtaques;
    public SlotEspecial SlotCasco => slotCasco;
    public SlotEspecial SlotPechera => slotPechera;
    public SlotEspecial SlotAmuleto => slotAmuleto;

    private void Awake()
    {
        BuscarReferenciasFaltantes();

        if (panelCombate != null)
        {
            panelCombate.SetActive(false);
        }
    }

    public bool IniciarCombate()
    {
        if (combateActivo)
        {
            return false;
        }

        if (inventoryManager == null)
        {
            Debug.LogError("No se puede iniciar el combate porque falta " +"InventoryManager.",this);

            return false;
        }

        LimpiarInformacionDeCombate();
        RecogerInformacionDelInventario();

        combateActivo = true;

        if (panelInventario != null)
        {
            panelInventario.SetActive(false);
        }

        if (panelCombate != null)
        {
            panelCombate.SetActive(true);
        }

        EstablecerMovimientoJugadorBloqueado(true);

        MostrarPreparacionEnConsola();

        return true;
    }

    private void RecogerInformacionDelInventario()
    {
        IReadOnlyList<SlotEspecial> slotsEspeciales =inventoryManager.SlotsEspeciales;

        if (slotsEspeciales == null)
        {
            return;
        }

        foreach (SlotEspecial slot in slotsEspeciales)
        {
            if (slot == null)
            {
                continue;
            }

            switch (slot.Uso)
            {
                case UsoSlotEspecial.Casco:
                    slotCasco = slot;
                    break;

                case UsoSlotEspecial.Pechera:
                    slotPechera = slot;
                    break;

                case UsoSlotEspecial.Amuleto:
                    slotAmuleto = slot;
                    break;

                case UsoSlotEspecial.ConsumibleCombate:
                    slotsConsumibles.Add(slot);
                    break;

                case UsoSlotEspecial.AtaqueCombate:
                    slotsAtaques.Add(slot);
                    break;
            }
        }
    }

    public void FinalizarCombate(bool jugadorGano)
    {
        if (!combateActivo)
        {
            return;
        }

        combateActivo = false;

        if (panelCombate != null)
        {
            panelCombate.SetActive(false);
        }

        EstablecerMovimientoJugadorBloqueado(false);

        inventoryManager?.ActualizarInventarioDespuesDeCambio();

        Debug.Log( jugadorGano ? "El combate terminó con victoria.": "El combate terminó sin victoria." );

        LimpiarInformacionDeCombate();
    }

    public SlotEspecial ObtenerSlotConsumible(int indice)
    {
        if (!IndiceValido(slotsConsumibles, indice))
        {
            return null;
        }

        return slotsConsumibles[indice];
    }

    public SlotEspecial ObtenerSlotAtaque(int indice)
    {
        if (!IndiceValido(slotsAtaques, indice))
        {
            return null;
        }

        return slotsAtaques[indice];
    }

    public DatosObjeto_Base ObtenerConsumible(int indice)
    {
        SlotEspecial slot = ObtenerSlotConsumible(indice);

        if (slot == null || slot.EstaVacio())
        {
            return null;
        }

        return slot.Objeto;
    }

    public DatosObjeto_Base ObtenerAtaque(int indice)
    {
        SlotEspecial slot = ObtenerSlotAtaque(indice);

        if (slot == null || slot.EstaVacio())
        {
            return null;
        }

        return slot.Objeto;
    }

    public bool ConsumirCantidad( SlotEspecial slot,int cantidad)
    {
        if (!combateActivo || slot == null || cantidad <= 0)
        {
            return false;
        }

        if (!slotsConsumibles.Contains(slot) &&
            !slotsAtaques.Contains(slot))
        {
            Debug.LogWarning("El slot indicado no pertenece a los slots " +"utilizables durante el combate.",this);

            return false;
        }

        bool cantidadRetirada =slot.RetirarCantidad(cantidad);

        if (!cantidadRetirada)
        {
            return false;
        }

        inventoryManager.ActualizarInventarioDespuesDeCambio();

        return true;
    }

    private void LimpiarInformacionDeCombate()
    {
        slotsConsumibles.Clear();
        slotsAtaques.Clear();

        slotCasco = null;
        slotPechera = null;
        slotAmuleto = null;
    }

    private bool IndiceValido(List<SlotEspecial> lista,int indice)
    {
        return lista != null &&
               indice >= 0 &&
               indice < lista.Count;
    }

    private void MostrarPreparacionEnConsola()
    {
        Debug.Log( "Combate iniciado. " +"Consumibles preparados: " + ContarSlotsOcupados(slotsConsumibles) + "/" +
            slotsConsumibles.Count + ". Ataques preparados: " +ContarSlotsOcupados(slotsAtaques) + "/" + slotsAtaques.Count +"."
        );

        MostrarEquipo("Casco", slotCasco);
        MostrarEquipo("Pechera", slotPechera);
        MostrarEquipo("Amuleto", slotAmuleto);
    }

    private int ContarSlotsOcupados(List<SlotEspecial> lista)
    {
        int cantidadOcupada = 0;

        foreach (SlotEspecial slot in lista)
        {
            if (slot != null && !slot.EstaVacio())
            {
                cantidadOcupada++;
            }
        }

        return cantidadOcupada;
    }

    private void MostrarEquipo(string nombreParte,SlotEspecial slot)
    {
        string contenido =slot != null && !slot.EstaVacio()? slot.Objeto.nombreObjeto: "vacío";

        Debug.Log(nombreParte + ": " + contenido);
    }

    private void BuscarReferenciasFaltantes()
    {
        if (inventoryManager == null)
        {
            inventoryManager =
                FindAnyObjectByType<InventoryManager>();
        }

        if (movimientoJugador == null)
        {
            movimientoJugador =
                FindAnyObjectByType<MovimientoJugador>();
        }

        if (movimientoJugador != null)
        {
            entradaJugador = movimientoJugador.GetComponent<PlayerInput>();
        }

        if (entradaJugador == null)
        {
            entradaJugador = FindAnyObjectByType<PlayerInput>();
        }
    }

    private void EstablecerMovimientoJugadorBloqueado(bool bloqueado)
    {
        movimientoJugadorBloqueado = bloqueado;

        if (movimientoJugador != null)
        {
            movimientoJugador.EstablecerMovimientoActivo(!bloqueado);
        }

        if (entradaJugador != null)
        {
            if (bloqueado)
            {
                activarEntradaAlFinalizarCombate =
                    entradaJugador.inputIsActive;
                entradaJugador.DeactivateInput();
            }
            else if (activarEntradaAlFinalizarCombate)
            {
                entradaJugador.ActivateInput();
            }
        }

        if (movimientoJugador == null && entradaJugador == null)
        {
            Debug.LogWarning(
                "No se encontró MovimientoJugador ni PlayerInput para bloquear el movimiento.",
                this
            );
        }
    }
}