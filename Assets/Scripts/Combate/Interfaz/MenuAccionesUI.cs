using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuAccionesUI : MonoBehaviour
{
    [Header("Menú principal")]
    [SerializeField] private GameObject contenedorPrincipal;
    [SerializeField] private Button botonAtacar;
    [SerializeField] private Button botonConsumible;
    [SerializeField] private Button botonHuir;

    [Header("Submenú (3 slots de armas/hechizos o consumibles)")]
    [SerializeField] private GameObject contenedorSubmenu;
    [SerializeField] private Button[] botonesSlot = new Button[3];
    [SerializeField] private TMP_Text[] textosSlot = new TMP_Text[3];
    [SerializeField] private Button botonVolver;

    public AccionJugador AccionElegida { get; private set; }

    private bool listenersListos;
    private bool hayEleccion;
    private EquipamientoCombateManager equipamiento;
    private Combatiente jugador;

    // Espera a que el jugador elija; el resultado queda en AccionElegida.
    public IEnumerator EsperarAccion(EquipamientoCombateManager equipamientoCombate, Combatiente combatienteJugador)
    {
        PrepararListeners();

        equipamiento = equipamientoCombate;
        jugador = combatienteJugador;
        hayEleccion = false;

        MostrarPrincipal();

        while (!hayEleccion)
        {
            yield return null;
        }

        Ocultar();
    }

    public void Ocultar()
    {
        contenedorPrincipal.SetActive(false);
        contenedorSubmenu.SetActive(false);
    }

    private void PrepararListeners()
    {
        if (listenersListos)
        {
            return;
        }

        listenersListos = true;

        botonAtacar.onClick.AddListener(() => AbrirSubmenu(TipoAccion.Atacar));
        botonConsumible.onClick.AddListener(() => AbrirSubmenu(TipoAccion.UsarConsumible));
        botonHuir.onClick.AddListener(() => Elegir(new AccionJugador(TipoAccion.Huir, null)));
        botonVolver.onClick.AddListener(MostrarPrincipal);
    }

    private void MostrarPrincipal()
    {
        contenedorSubmenu.SetActive(false);
        contenedorPrincipal.SetActive(true);
        Seleccionar(botonAtacar);
    }

    private void AbrirSubmenu(TipoAccion tipo)
    {
        IReadOnlyList<SlotEspecial> slots = tipo == TipoAccion.Atacar
            ? equipamiento.SlotsAtaques
            : equipamiento.SlotsConsumibles;

        contenedorPrincipal.SetActive(false);
        contenedorSubmenu.SetActive(true);

        Button primerUtilizable = null;

        for (int i = 0; i < botonesSlot.Length; i++)
        {
            SlotEspecial slot = i < slots.Count ? slots[i] : null;
            string descripcion = Describir(slot, tipo, out bool utilizable);

            textosSlot[i].text = descripcion;
            botonesSlot[i].interactable = utilizable;
            botonesSlot[i].onClick.RemoveAllListeners();

            if (utilizable)
            {
                SlotEspecial slotElegido = slot;
                botonesSlot[i].onClick.AddListener(() => Elegir(new AccionJugador(tipo, slotElegido)));

                if (primerUtilizable == null)
                {
                    primerUtilizable = botonesSlot[i];
                }
            }
        }

        Seleccionar(primerUtilizable != null ? primerUtilizable : botonVolver);
    }

    private string Describir(SlotEspecial slot, TipoAccion tipo, out bool utilizable)
    {
        utilizable = false;

        if (slot == null || slot.EstaVacio())
        {
            return "—";
        }

        DatosObjeto_Base objeto = slot.Objeto;

        if (tipo == TipoAccion.Atacar && objeto is IAtaqueCombate ataque)
        {
            utilizable = jugador.Energia >= ataque.CostoEnergia;
            return objeto.nombreObjeto + " (" + ataque.CostoEnergia + " EN)";
        }

        if (tipo == TipoAccion.UsarConsumible && objeto is IConsumibleCombate)
        {
            utilizable = true;
            return objeto.nombreObjeto + " x" + slot.Cantidad;
        }

        return objeto.nombreObjeto;
    }

    private void Elegir(AccionJugador accion)
    {
        AccionElegida = accion;
        hayEleccion = true;
    }

    private void Seleccionar(Button boton)
    {
        if (EventSystem.current != null && boton != null)
        {
            EventSystem.current.SetSelectedGameObject(boton.gameObject);
        }
    }
}
