// Implementa esta interfaz en tus armas y hechizos ofensivos
// (por ejemplo: public class DatosArma : DatosObjeto_Base, IAtaqueCombate).
public interface IAtaqueCombate
{
    int DanioBase { get; }
    int CostoEnergia { get; }
}

// Implementa esta interfaz en pociones y consumibles de curación gradual.
public interface IConsumibleCombate
{
    int CuracionInstantanea { get; }
    int EnergiaInstantanea { get; }
    int CuracionGradualTotal { get; }
    int CuracionPorTurno { get; }
    int TurnosDeEfecto { get; }
    int TurnosCuracion { get; }
    int TurnosBonus { get; }

    // 0.1 = +10% de daño mientras dure el efecto.
    float BonusDanio { get; }
}
