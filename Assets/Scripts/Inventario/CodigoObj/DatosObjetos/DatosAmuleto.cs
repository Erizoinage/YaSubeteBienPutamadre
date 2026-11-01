using UnityEngine;

[CreateAssetMenu(fileName = "NuevoAmuleto",menuName = "Inventario/Amuleto")]
public class DatosAmuleto : DatosObjeto_Base
{
    [Header("Efectos del amuleto")]
    public DatosBonus bonusMientrasEstaEquipado;

    public bool TieneBonus()
    {
        return bonusMientrasEstaEquipado != null && bonusMientrasEstaEquipado.TieneAlgunaModificacion();
    }

    public void AplicarBonus(EstadisticasJugador estadisticasJugador)
    {
        if (estadisticasJugador == null || !TieneBonus())
        {
            return;
        }
        estadisticasJugador.AplicarBonus(bonusMientrasEstaEquipado);
        {
            if (estadisticasJugador == null || !TieneBonus())
            {
                return;
            }

            estadisticasJugador.QuitarBonus(bonusMientrasEstaEquipado);
        }
    }
}
