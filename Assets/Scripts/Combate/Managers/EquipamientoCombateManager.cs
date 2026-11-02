using System.Collections.Generic;
using UnityEngine;

public class EquipamientoCombateManager : MonoBehaviour
{
    [Header("Inventario")]
    [SerializeField]
    private InventoryManager inventoryManager;

    [Header("Slots disponibles en combate")]
    [SerializeField]
    private List<SlotEspecial> slotsConsumibles =
        new List<SlotEspecial>();

    [SerializeField]
    private List<SlotEspecial> slotsAtaques =
        new List<SlotEspecial>();

    private void Awake()
    {
        BuscarSlotsDeCombate();
    }

    public IReadOnlyList<SlotEspecial> SlotsConsumibles =>
        slotsConsumibles;

    public IReadOnlyList<SlotEspecial> SlotsAtaques =>
        slotsAtaques;

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

    public bool ConsumirDesdeSlot(
        SlotEspecial slot,
        int cantidad
    )
    {
        if (slot == null || slot.EstaVacio())
        {
            return false;
        }

        if (cantidad <= 0)
        {
            return false;
        }

        if (!EsSlotUtilizableEnCombate(slot))
        {
            Debug.LogWarning(
                "El slot indicado no puede utilizarse en combate.",
                this
            );

            return false;
        }

        if (slot.Objeto is DatosArma)
        {
            Debug.LogWarning(
                "Las armas no se consumen al utilizarse.",
                this
            );

            return false;
        }

        bool retirado = slot.RetirarCantidad(cantidad);

        if (!retirado)
        {
            return false;
        }

        inventoryManager.ActualizarInventarioDespuesDeCambio();
        return true;
    }

    public DatosObjeto_Base ObtenerObjetoConsumible(
        int indice
    )
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

    private void BuscarSlotsDeCombate()
    {
        slotsConsumibles.Clear();
        slotsAtaques.Clear();

        if (inventoryManager == null)
        {
            Debug.LogWarning(
                "Asigna el InventoryManager para buscar los slots de combate.",
                this
            );
            return;
        }

        foreach (SlotEspecial slot in inventoryManager.SlotsEspeciales)
        {
            if (slot == null)
            {
                continue;
            }

            if (slot.AceptaEtiqueta(TipoDeObjeto.Arma) ||
                slot.AceptaEtiqueta(TipoDeObjeto.Echizo))
            {
                slotsAtaques.Add(slot);
            }

            if (slot.AceptaEtiqueta(TipoDeObjeto.Consumible) ||
                slot.AceptaEtiqueta(TipoDeObjeto.Posion))
            {
                slotsConsumibles.Add(slot);
            }
        }
    }

    private bool EsSlotUtilizableEnCombate(
        SlotEspecial slot
    )
    {
        return slotsConsumibles.Contains(slot) ||
               slotsAtaques.Contains(slot);
    }

    private bool IndiceValido(
        List<SlotEspecial> lista,
        int indice
    )
    {
        return lista != null &&
               indice >= 0 &&
               indice < lista.Count;
    }

    private void OnValidate()
    {
        EliminarReferenciasNulas(slotsConsumibles);
        EliminarReferenciasNulas(slotsAtaques);
    }

    private void EliminarReferenciasNulas(
        List<SlotEspecial> lista
    )
    {
        if (lista == null)
        {
            return;
        }

        lista.RemoveAll(slot => slot == null);
    }
}