using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlotEspecial : SlotInventario
{
    [Header("Etiquetas permitidas")]
    [SerializeField] private TipoDeObjeto etiquetaPermitida;
    [SerializeField] private List<TipoDeObjeto> etiquetasAdicionales = new List<TipoDeObjeto>();

    [Header("Representacion visual")]
    [SerializeField] private SlotInventarioVisual visualSlot;

    public TipoDeObjeto EtiquetaPermitida => etiquetaPermitida;
    public SlotInventarioVisual VisualSlot => visualSlot;

    public override bool PuedeGuardar(DatosObjeto_Base nuevoObjeto)
    {
        if (nuevoObjeto == null ||
            !TipoEquipamientoCoincideEtiqueta(nuevoObjeto) ||
            !PuedeGuardarEtiqueta(nuevoObjeto))
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
}
