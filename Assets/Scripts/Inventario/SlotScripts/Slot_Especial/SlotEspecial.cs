using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public enum FuncionSlotEspecial
{
    Equipamiento,
    ConsumibleCombate,
    AtaqueCombate
}
[System.Serializable]
public class SlotEspecial : SlotInventario
{
    [Header("Etiquetas permitidas")]
    [SerializeField] private TipoDeObjeto etiquetaPermitida;
    [SerializeField] private List<TipoDeObjeto> etiquetasAdicionales = new List<TipoDeObjeto>();

    [Header("Representacion visual")]
    [SerializeField] private SlotInventarioVisual visualSlot;

    [Header("Funcion del Slot")]

    [SerializeField] private FuncionSlotEspecial funcion;
    private FuncionSlotEspecial Funcion => funcion;

    public TipoDeObjeto EtiquetaPermitida => etiquetaPermitida;
    public SlotInventarioVisual VisualSlot => visualSlot;

    public bool AceptaEtiqueta(TipoDeObjeto etiqueta)
    {
        return PuedeGuardarEtiqueta(etiqueta);
    }

    public override bool PuedeGuardar(DatosObjeto_Base nuevoObjeto)
    {
        if (nuevoObjeto == null ||!TipoEquipamientoCoincideEtiqueta(nuevoObjeto) ||!PuedeGuardarEtiqueta(nuevoObjeto))
        {
            return false;
        }

        return base.PuedeGuardar(nuevoObjeto);
    }

    private bool TipoEquipamientoCoincideEtiqueta(DatosObjeto_Base nuevoObjeto)
    {
        if (!(nuevoObjeto is DatosEquipable equipamiento))
        {
            return true;
        }

        TipoDeObjeto etiquetaRequerida;
        switch (equipamiento.tipoEquipamiento)
        {
            case TipoEquipamiento.Casco:
                etiquetaRequerida = TipoDeObjeto.Casco;
                break;

            case TipoEquipamiento.Pechera:
                etiquetaRequerida = TipoDeObjeto.Armadura;
                break;

            default:
                return false;
        }

        return nuevoObjeto.TieneEtiqueta(etiquetaRequerida);
    }

    private bool PuedeGuardarEtiqueta(DatosObjeto_Base nuevoObjeto)
    {
        if (nuevoObjeto.TieneEtiqueta(etiquetaPermitida))
        {
            return true;
        }

        if (etiquetasAdicionales == null)
        {
            return false;
        }

        foreach (TipoDeObjeto etiqueta in etiquetasAdicionales)
        {
            if (nuevoObjeto.TieneEtiqueta(etiqueta))
            {
                return true;
            }
        }

        return false;
    }

    private bool PuedeGuardarEtiqueta(TipoDeObjeto etiqueta)
    {
        if (etiquetaPermitida == etiqueta)
        {
            return true;
        }

        return etiquetasAdicionales != null && etiquetasAdicionales.Contains(etiqueta);
    }
    
    public bool UtilizableEnCombate()
    {
        return funcion == FuncionSlotEspecial.ConsumibleCombate || funcion == FuncionSlotEspecial.AtaqueCombate;
    }

    public bool ConsumirCantidadEnCombate(int cantidadAConsumir)
    {
    if (!UtilizableEnCombate())
       {
         Debug.LogWarning("Este slot especial no se puede consumir en combate.");

         return false;
       }

        return base.RetirarCantidad(cantidadAConsumir);
    }
}
