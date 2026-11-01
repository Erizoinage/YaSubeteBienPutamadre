using UnityEngine;

[CreateAssetMenu(fileName = "NuevaArma",menuName ="Inventario/Arma")]
public class DatosArma : DatosObjeto_Base
{
    [Header("Propiedades de combate")]
    [Min(0)]public int dañoArma =1;
    
    [Min(0)] public int CostoDeEnergia =1;

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
        return estadisticasJugador.TieneEnergia(CostoDeEnergia);
    }
    }

