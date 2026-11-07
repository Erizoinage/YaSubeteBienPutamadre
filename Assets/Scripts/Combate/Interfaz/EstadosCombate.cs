public enum FaseCombate
{
    Inactivo,
    Intro,
    TurnoJugador,
    PreparaTurnoRival,
    TurnoRival,
    PreparaTurnoJugador,
    Final
}

public enum ResultadoCombate
{
    Victoria,
    Derrota,
    Huida
}

public enum TipoAccion
{
    Atacar,
    UsarConsumible,
    Huir
}

// Lo que el jugador eligió en el menú de acciones.
public readonly struct AccionJugador
{
    public readonly TipoAccion tipo;
    public readonly SlotEspecial slot;

    public AccionJugador(TipoAccion tipo, SlotEspecial slot)
    {
        this.tipo = tipo;
        this.slot = slot;
    }
}
