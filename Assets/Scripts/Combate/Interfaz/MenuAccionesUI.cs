using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Los botones se conectan por código cuando empieza el turno del jugador:
// NO añadas eventos OnClick a mano en el Inspector.
public class MenuAccionesUI : MonoBehaviour
{
    [Header("Menú principal")]
    [SerializeField] private GameObject contenedorPrincipal;
    [SerializeField] private Button botonAtacar;
    [SerializeField] private Button botonConsumible;
    [SerializeField] private Button botonHuir;

    [Header("Submenú (el elemento 0 de cada lista es el primer botón)")]
    [SerializeField] private GameObject contenedorSubmenu;
    [SerializeField] private Button[] botonesSlot = new Button[3];
    [SerializeField] private TMP_Text[] textosSlot = new TMP_Text[3];
    [Tooltip("Opcional: asigna la Image de cada espacio; se usa tanto para armas como para consumibles.")]
    [SerializeField] private Image[] iconosSlot = new Image[3];
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
        Activar(contenedorPrincipal, false);
        Activar(contenedorSubmenu, false);
    }

    private void PrepararListeners()
    {
        if (listenersListos)
        {
            return;
        }

        listenersListos = true;

        ConectarBoton(botonAtacar, "botonAtacar", () => AbrirSubmenu(TipoAccion.Atacar));
        ConectarBoton(botonConsumible, "botonConsumible", () => AbrirSubmenu(TipoAccion.UsarConsumible));
        ConectarBoton(botonHuir, "botonHuir", () => Elegir(new AccionJugador(TipoAccion.Huir, null)));
        ConectarBoton(botonVolver, "botonVolver", MostrarPrincipal);
    }

    private void ConectarBoton(Button boton, string nombreCampo, UnityAction accion)
    {
        if (boton == null)
        {
            Debug.LogWarning("MenuAccionesUI: falta asignar " + nombreCampo + ".", this);
            return;
        }

        boton.onClick.AddListener(accion);
    }

    private void MostrarPrincipal()
    {
        Activar(contenedorSubmenu, false);
        Activar(contenedorPrincipal, true);
        Seleccionar(botonAtacar);
    }

    private void AbrirSubmenu(TipoAccion tipo)
    {
        IReadOnlyList<SlotEspecial> slots = tipo == TipoAccion.Atacar
            ? equipamiento.SlotsAtaques
            : equipamiento.SlotsConsumibles;

        Activar(contenedorPrincipal, false);
        Activar(contenedorSubmenu, true);

        Button primerUtilizable = null;

        for (int i = 0; i < botonesSlot.Length; i++)
        {
            Button boton = botonesSlot[i];

            if (boton == null)
            {
                continue;
            }

            SlotEspecial slot = i < slots.Count ? slots[i] : null;
            bool utilizable = Describir(slot, tipo, out string texto, out Sprite icono);

            if (i < textosSlot.Length && textosSlot[i] != null)
            {
                textosSlot[i].text = texto;
            }

            Image imagenIcono = ObtenerImagenIconoAsignado(i);
            if (imagenIcono != null)
            {
                imagenIcono.sprite = icono;
                imagenIcono.preserveAspect = true;
                imagenIcono.enabled = icono != null;
                imagenIcono.raycastTarget = false;
                ReservarEspacioParaIcono(boton);
            }

            boton.interactable = utilizable;
            boton.onClick.RemoveAllListeners();

            if (!utilizable)
            {
                continue;
            }

            SlotEspecial slotElegido = slot;
            boton.onClick.AddListener(() => Elegir(new AccionJugador(tipo, slotElegido)));

            if (primerUtilizable == null)
            {
                primerUtilizable = boton;
            }
        }

        Seleccionar(primerUtilizable != null ? primerUtilizable : botonVolver);
    }

    private Image ObtenerImagenIconoAsignado(int indice)
    {
        if (iconosSlot != null &&
            indice < iconosSlot.Length &&
            iconosSlot[indice] != null)
        {
            return iconosSlot[indice];
        }

        return null;
    }

    private void ReservarEspacioParaIcono(Button boton)
    {
        TMP_Text texto = boton.GetComponentInChildren<TMP_Text>(true);
        if (texto == null)
        {
            return;
        }

        RectTransform rectTransform = texto.rectTransform;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = new Vector2(64f, rectTransform.offsetMin.y);
        rectTransform.offsetMax = new Vector2(-8f, rectTransform.offsetMax.y);
        texto.alignment = TextAlignmentOptions.MidlineLeft;
    }

    // Devuelve true si el slot se puede usar ahora.
    private bool Describir(SlotEspecial slot, TipoAccion tipo, out string texto, out Sprite icono)
    {
        texto = "—";
        icono = null;

        if (slot == null || slot.EstaVacio())
        {
            return false;
        }

        DatosObjeto_Base objeto = slot.Objeto;
        texto = objeto.nombreObjeto;
        icono = objeto.icono;

        if (tipo == TipoAccion.Atacar)
        {
            if (objeto is IAtaqueCombate ataque)
            {
                texto = objeto.nombreObjeto + " (" + ataque.CostoEnergia + " EN)";
                bool cantidadDisponible = !(objeto is DatosHechizo hechizo) ||
                    slot.Cantidad >= hechizo.cantidadConsumidaPorUso;
                return cantidadDisponible && jugador.Energia >= ataque.CostoEnergia;
            }

            Debug.LogWarning(objeto.nombreObjeto + " no implementa IAtaqueCombate, por eso su botón está desactivado.", objeto);
            return false;
        }

        if (objeto is IConsumibleCombate)
        {
            texto = objeto.nombreObjeto + " x" + slot.Cantidad;
            return true;
        }

        Debug.LogWarning(objeto.nombreObjeto + " no implementa IConsumibleCombate, por eso su botón está desactivado.", objeto);
        return false;
    }

    private void Elegir(AccionJugador accion)
    {
        AccionElegida = accion;
        hayEleccion = true;
    }

    private void Activar(GameObject objeto, bool activo)
    {
        if (objeto != null)
        {
            objeto.SetActive(activo);
        }
    }

    private void Seleccionar(Button boton)
    {
        if (EventSystem.current != null && boton != null)
        {
            EventSystem.current.SetSelectedGameObject(boton.gameObject);
        }
    }
}