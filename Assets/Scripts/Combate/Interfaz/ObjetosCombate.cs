// Implementa esta interfaz en tus armas y hechizos ofensivos
// (por ejemplo: public class DatosArma : DatosObjeto_Base, IAtaqueCombate).
public interface IAtaqueCombate
{
    int DanioBase { get; }
    int CostoEnergia { get; }
}

// Implementa esta interfaz en tus pociones y comida.
// Poción: solo CuracionInstantanea.
// Comida: CuracionPorTurno + TurnosDeEfecto (+ BonusDanio opcional).
public interface IConsumibleCombate
{
    int CuracionInstantanea { get; }
    int CuracionPorTurno { get; }
    int TurnosDeEfecto { get; }

    // 0.1 = +10% de daño mientras dure el efecto.
    float BonusDanio { get; }
}
