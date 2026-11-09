using UnityEngine;

[CreateAssetMenu(fileName = "NuevoConsumible",menuName = "Inventario/Consumible")]
public class DatosConsumible : DatosObjeto_Base, IConsumibleCombate
{
    [Header("Curacion gradual")]

    [Min(0)]
    [Tooltip("Cantidad total de vida que recuperara.")]
    public int curacionTotal;

    [Min(0.1f)]
    [Tooltip(
        "Duracion total de la curacion en segundos. " +
        "Se aplica en pulsos al inicio de cada turno del jugador."
    )]
    public float duracionCuracionSegundos = 5f;

    [Min(0.1f)]
    [Tooltip("Tiempo entre cada pulso de curacion.")]
    public float intervaloCuracion = 1f;

    [Header("Recuperacion inmediata")]

    [Min(0)]
    [Tooltip("Energia recuperada al consumir el objeto.")]
    public int energiaInmediata;

    [Header("Bonus temporal")]

    [Tooltip("Bonus aplicado al consumir el objeto.")]
    public DatosBonus bonusTemporal;

    [Min(1)]
    [Tooltip("Cantidad de turnos que durara el bonus.")]
    public int duracionBonusEnTurnos = 1;

    public int CuracionInstantanea => 0;
    public int EnergiaInstantanea => energiaInmediata;
    public int CuracionGradualTotal => curacionTotal;
    public int TurnosCuracion => curacionTotal > 0
        ? Mathf.CeilToInt(
            Mathf.Max(0.1f, duracionCuracionSegundos) /
            Mathf.Max(0.1f, intervaloCuracion)
        )
        : 0;
    public int CuracionPorTurno => TurnosCuracion > 0
        ? Mathf.CeilToInt((float)curacionTotal / TurnosCuracion)
        : 0;
    public int TurnosBonus => TieneBonus() ? duracionBonusEnTurnos : 0;
    public int TurnosDeEfecto => Mathf.Max(TurnosCuracion, TurnosBonus);
    public float BonusDanio => bonusTemporal != null ? bonusTemporal.bonusDanio : 0f;

    public bool TieneBonus()
    {
        return bonusTemporal != null;
    }

    private void OnValidate()
    {
        curacionTotal = Mathf.Max(0, curacionTotal);
        duracionCuracionSegundos =
            Mathf.Max(0.1f, duracionCuracionSegundos);

        intervaloCuracion = Mathf.Clamp(
            intervaloCuracion,
            0.1f,
            duracionCuracionSegundos
        );

        energiaInmediata = Mathf.Max(
            0,
            energiaInmediata
        );

        duracionBonusEnTurnos = Mathf.Max(
            1,
            duracionBonusEnTurnos
        );
    }
}