using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NuevaArma",menuName ="Inventario/Arma")]
public class DatosArma : DatosObjeto_Base, IAtaqueCombate
{
    [Header("Propiedades de combate")]
    [Min(0)]public int dañoArma =1;
    
    [FormerlySerializedAs("CostoDeEnergia")]
    [Min(0)] public int costoDeEnergia =1;

    public int DanioBase => dañoArma;
    public int CostoEnergia => costoDeEnergia;

    public int CalcularDañoTotal(EstadisticasJugador estadisticasJugador)
    {
        if (estadisticasJugador == null)
        {
            return Mathf.Max(0,dañoArma);
        }

        return Mathf.Max(0,estadisticasJugador.AtaqueTotal + dañoArma);
    }

    public bool PuedeUsarse(EstadisticasJugador estadisticasJugador)
    {
        if (estadisticasJugador == null)
        {
            return false;
        }
        if (estadisticasJugador.EstaDerrotado)
        {
            return false;
        }
        return estadisticasJugador.TieneEnergia(costoDeEnergia);
    }
    }
