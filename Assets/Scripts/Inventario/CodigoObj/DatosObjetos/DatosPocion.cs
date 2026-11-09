using UnityEngine;
[CreateAssetMenu(fileName = "NuevaPocion",menuName = "Inventario/Objeto Pocion")]
public class DatosPocion : DatosObjeto_Base, IConsumibleCombate
{
    [Header("Efectos Inmediatos")]

    [Min(0)] public int vidaRecuperada;

    [Min(0)] public int energiaRecuperada;

    public int CuracionInstantanea => vidaRecuperada;
    public int EnergiaInstantanea => energiaRecuperada;
    public int CuracionGradualTotal => 0;
    public int CuracionPorTurno => 0;
    public int TurnosDeEfecto => 0;
    public int TurnosCuracion => 0;
    public int TurnosBonus => 0;
    public float BonusDanio => 0f;
}
