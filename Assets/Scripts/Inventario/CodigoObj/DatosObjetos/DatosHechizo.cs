using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Hechizo", menuName = "Inventario/Hechizo")]

public class DatosHechizo : DatosObjeto_Base
{
    [Header("Propiedades de combate")]
    [Min(0)]public int dañoHechizo = 1;
    [Min(0)]public int costoEnergia = 1;
    [Min(0)]public int cantidadConsumidaPorUso = 1;

    public int CalcularDañoTotal(EstadisticasJugador estadisticasJugador)
    {
        if (estadisticasJugador == null)
        {
            return Mathf.Max(0, dañoHechizo);
        }
        return Mathf.Max(0, estadisticasJugador.AtaqueTotal + dañoHechizo);
    }

    public bool PuedeUsarse(EstadisticasJugador estadisticasJugador, int cantidadDisponible)
    {
        if (estadisticasJugador == null)
        {
            return false;
        }

        if (estadisticasJugador.EstaDerrotado)
        {
            return false;
        }

        if (cantidadDisponible < cantidadConsumidaPorUso)
        {
            return false;
        }
        return estadisticasJugador.TieneEnergia(costoEnergia);
    }

    public void OnValidate()
    {
        dañoHechizo = Mathf.Max(0, dañoHechizo);
        costoEnergia = Mathf.Max(0, costoEnergia);
        cantidadConsumidaPorUso = Mathf.Max(1, cantidadConsumidaPorUso);
    }
}
