using UnityEngine;

public class SistemasJugador : MonoBehaviour
{
    [Header("Sistemas del jugador")]
    [SerializeField] private EstadisticasJugador estadisticas;
    [SerializeField] private EfectosManager efectos;
    [SerializeField] private VisualEquipamiento visualEquipamiento;
    [SerializeField] private InteraccionJugador interaccion;

    public EstadisticasJugador Estadisticas => estadisticas;
    public EfectosManager Efectos => efectos;
    public VisualEquipamiento VisualEquipamiento =>visualEquipamiento;

    private void Reset()
    {
        BuscarComponentes();
    }

    private void Awake()
    {
        BuscarReferenciasFaltantes();
    }

    [ContextMenu("Buscar componentes automáticamente")]
    private void BuscarComponentes()
    {
        estadisticas =GetComponent<EstadisticasJugador>();

        efectos = GetComponent<EfectosManager>();

        visualEquipamiento =GetComponent<VisualEquipamiento>();

        interaccion =GetComponent<InteraccionJugador>();
    }

    private void BuscarReferenciasFaltantes()
    {
        if (estadisticas == null)
        {
            estadisticas =GetComponent<EstadisticasJugador>();
        }

        if (efectos == null)
        {
            efectos =GetComponent<EfectosManager>();
        }

        if (visualEquipamiento == null)
        {
            visualEquipamiento =GetComponent<VisualEquipamiento>();
        }

        if (interaccion == null)
        {
            interaccion =GetComponent<InteraccionJugador>();
        }
    }

    public void IniciarTurnoEnemigo()
    {
        if (efectos != null)
        {
            efectos.IniciarTurnoEnemigo();
        }
    }

    public void FinalizarTurnoEnemigo()
    {
        if (efectos != null)
        {
            efectos.FinalizarTurnoEnemigo();
        }
    }

    public void RecibirDanio(int cantidad)
    {
        if (estadisticas != null)
        {
            estadisticas.RecibirDanio(cantidad);
        }
    }

    public void RecuperarVida(int cantidad)
    {
        if (estadisticas != null)
        {
            estadisticas.RecuperarVida(cantidad);
        }
    }

    public bool GastarEnergia(int cantidad)
    {
        if (estadisticas == null)
        {
            return false;
        }

        return estadisticas.GastarEnergia(cantidad);
    }
}