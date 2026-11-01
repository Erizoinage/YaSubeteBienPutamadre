using System.Collections.Generic;
using UnityEngine;

public class EfectosManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField]
    private EstadisticasJugador estadisticasJugador;

    [Header("Estado del combate")]
    [SerializeField]
    private bool turnoEnemigoActivo;
    private readonly List<CuracionActiva>curacionesActivas = new List<CuracionActiva>();
    private readonly List<BonusActivo>bonusActivos = new List<BonusActivo>();
    public bool TurnoEnemigoActivo =>turnoEnemigoActivo;

    private void Awake()
    {
        if (estadisticasJugador == null)
        {
            estadisticasJugador =GetComponent<EstadisticasJugador>();
        }
    }

    private void Update()
    {
        if (!turnoEnemigoActivo)
        {
            return;
        }

        ProcesarCuracionesPorTiempo(Time.deltaTime);
    }

    public bool Consumir(DatosConsumible consumible)
    {
        if (consumible == null ||estadisticasJugador == null)
        {
            return false;
        }

        if (estadisticasJugador.EstaDerrotado)
        {
            return false;
        }

        estadisticasJugador.RecuperarEnergia(consumible.energiaInmediata);

        if (consumible.curacionTotal > 0)
        {
            AgregarCuracion(consumible.curacionTotal,consumible.duracionCuracionSegundos,consumible.intervaloCuracion);
        }

        if (consumible.TieneBonus())
        {
            AgregarBonus(consumible.bonusTemporal,consumible.duracionBonusEnTurnos);
        }

        Debug.Log("Se consumió: " +consumible.nombreObjeto);
        return true;
    }

    public bool ConsumirPocion(DatosPocion pocion)
    {
        if (pocion == null ||
            estadisticasJugador == null)
        {
            return false;
        }

        if (estadisticasJugador.EstaDerrotado)
        {
            return false;
        }

        estadisticasJugador.RecuperarVida(pocion.vidaRecuperada);

        estadisticasJugador.RecuperarEnergia(pocion.energiaRecuperada);

        Debug.Log("Se usó la poción: " +pocion.nombreObjeto);

        return true;
    }
    private void AgregarCuracion(int curacionTotal,float duracionSegundos,float intervaloSegundos)
    {
        if (curacionTotal <= 0)
        {
            return;
        }

        CuracionActiva nuevaCuracion =new CuracionActiva(curacionTotal,duracionSegundos,intervaloSegundos );

        curacionesActivas.Add(nuevaCuracion);
    }

    private void ProcesarCuracionesPorTiempo(float deltaTime)
    {
        if (estadisticasJugador == null)
        {
            return;
        }

        for (
            int i = curacionesActivas.Count - 1;
            i >= 0;
            i--
        )
        {
            CuracionActiva curacion =curacionesActivas[i];
            int vidaRecuperada =curacion.ProcesarTiempo(deltaTime);

            if (vidaRecuperada > 0)
            {
                estadisticasJugador.RecuperarVida(vidaRecuperada);
            }

            if (curacion.HaTerminado)
            {
                curacionesActivas.RemoveAt(i);
            }
        }
    }
    private void AgregarBonus(DatosBonus bonus,int duracionTurnos)
    {
        if (bonus == null)
        {
            return;
        }

        BonusActivo nuevoBonus =new BonusActivo(bonus,Mathf.Max(1, duracionTurnos));

        estadisticasJugador.AplicarBonus(bonus);
        bonusActivos.Add(nuevoBonus);
    }

    private void ProcesarBonusPorTurno()
    {
        for (int i = bonusActivos.Count - 1;i >= 0;i--)
        {
            BonusActivo bonusActivo =bonusActivos[i];

            bonusActivo.AvanzarTurno();

            if (!bonusActivo.HaTerminado)
            {
                continue;
            }

            estadisticasJugador.QuitarBonus(bonusActivo.Bonus);

            bonusActivos.RemoveAt(i);
        }
    }
    public void IniciarTurnoEnemigo()
    {
        if (estadisticasJugador == null ||estadisticasJugador.EstaDerrotado)
        {
            return;
        }

        turnoEnemigoActivo = true;

        Debug.Log( "Comenzó el turno enemigo. " +"La curación gradual está activa.");
    }

    public void FinalizarTurnoEnemigo()
    {
        if (!turnoEnemigoActivo)
        {
            return;
        }

        turnoEnemigoActivo = false;
        ProcesarBonusPorTurno();

        Debug.Log("Terminó el turno enemigo. " + "La curación gradual está pausada.");
    }

    public void LimpiarEfectosTemporales()
    {
        foreach (BonusActivo bonusActivo in bonusActivos)
        {
            estadisticasJugador.QuitarBonus(bonusActivo.Bonus);
        }

        bonusActivos.Clear();
        curacionesActivas.Clear();
        turnoEnemigoActivo = false;
    }
}