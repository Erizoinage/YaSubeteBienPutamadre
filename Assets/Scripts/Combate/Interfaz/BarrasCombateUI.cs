using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarrasCombateUI : MonoBehaviour
{
    [Header("Jugador")]
    [SerializeField] private Slider barraVida;
    [SerializeField] private TMP_Text textoVida;
    [SerializeField] private Slider barraEnergia;
    [SerializeField] private TMP_Text textoEnergia;

    [Header("Enemigo (opcional)")]
    [SerializeField] private Slider barraVidaEnemigo;

    private Combatiente jugador;
    private Combatiente enemigo;

    public void Conectar(Combatiente combatienteJugador, Combatiente combatienteEnemigo)
    {
        Desconectar();

        jugador = combatienteJugador;
        enemigo = combatienteEnemigo;

        jugador.VidaCambiada += ActualizarVidaJugador;
        jugador.EnergiaCambiada += ActualizarEnergiaJugador;
        enemigo.VidaCambiada += ActualizarVidaEnemigo;

        ActualizarVidaJugador(jugador.Vida, jugador.VidaMaxima);
        ActualizarEnergiaJugador(jugador.Energia, jugador.EnergiaMaxima);
        ActualizarVidaEnemigo(enemigo.Vida, enemigo.VidaMaxima);
    }

    public void Desconectar()
    {
        if (jugador != null)
        {
            jugador.VidaCambiada -= ActualizarVidaJugador;
            jugador.EnergiaCambiada -= ActualizarEnergiaJugador;
        }

        if (enemigo != null)
        {
            enemigo.VidaCambiada -= ActualizarVidaEnemigo;
        }

        jugador = null;
        enemigo = null;
    }

    private void OnDestroy()
    {
        Desconectar();
    }

    private void ActualizarVidaJugador(int actual, int maximo)
    {
        ActualizarBarra(barraVida, textoVida, actual, maximo);
    }

    private void ActualizarEnergiaJugador(int actual, int maximo)
    {
        ActualizarBarra(barraEnergia, textoEnergia, actual, maximo);
    }

    private void ActualizarVidaEnemigo(int actual, int maximo)
    {
        ActualizarBarra(barraVidaEnemigo, null, actual, maximo);
    }

    private void ActualizarBarra(Slider barra, TMP_Text texto, int actual, int maximo)
    {
        if (barra != null)
        {
            barra.maxValue = maximo;
            barra.value = actual;
        }

        if (texto != null)
        {
            texto.text = actual + " / " + maximo;
        }
    }
}
