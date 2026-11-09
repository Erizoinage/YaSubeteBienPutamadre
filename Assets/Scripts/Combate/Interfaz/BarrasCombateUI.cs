using TMPro;
using UnityEngine;

public class BarrasCombateUI : MonoBehaviour
{
    [Header("Jugador")]
    [SerializeField] private BarraImagen barraVida;
    [SerializeField] private TMP_Text textoVida;
    [SerializeField] private BarraImagen barraEnergia;
    [SerializeField] private TMP_Text textoEnergia;

    [Header("Enemigo (opcional)")]
    [SerializeField] private BarraImagen barraVidaEnemigo;

    private Combatiente jugador;
    private Combatiente enemigo;
    private EstadisticasJugador estadisticasJugador;

    public bool PrepararVisibleEnCanvas()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError( "BarrasCombateUI debe estar dentro de un Canvas activo.",this);
            return false;
        }

        Canvas canvasRaiz = canvas.rootCanvas;
        if (!canvasRaiz.gameObject.activeSelf)
        {
            canvasRaiz.gameObject.SetActive(true);
        }

        canvas.overrideSorting = true;
        canvas.sortingOrder = Mathf.Max(canvas.sortingOrder, 100);

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        enabled = true;
        return true;
    }

    public void VincularJugador(EstadisticasJugador estadisticas)
    {
        DesconectarCombatientes();
        DesconectarEstadisticas();

        if (estadisticas == null)
        {
            Debug.LogError("BarrasCombateUI necesita las estadísticas del jugador.", this);
            return;
        }

        estadisticasJugador = estadisticas;
        estadisticasJugador.AlCambiarVida += ActualizarVidaJugador;
        estadisticasJugador.AlCambiarEnergia += ActualizarEnergiaJugador;
        ActualizarBarra(barraVida, textoVida,estadisticasJugador.VidaActual,estadisticasJugador.VidaMaxima,true );
        ActualizarBarra(
            barraEnergia,
            textoEnergia,
            estadisticasJugador.EnergiaActual,
            estadisticasJugador.EnergiaMaxima,
            true
        );
    }

    public void Conectar(Combatiente combatienteJugador, Combatiente combatienteEnemigo)
    {
        if (!PrepararVisibleEnCanvas())
        {
            return;
        }

        DesconectarCombatientes();
        DesconectarEstadisticas();

        if (combatienteJugador == null || combatienteEnemigo == null)
        {
            Debug.LogError("BarrasCombateUI necesita los dos combatientes.", this);
            return;
        }

        jugador = combatienteJugador;
        enemigo = combatienteEnemigo;

        jugador.VidaCambiada += ActualizarVidaJugador;
        jugador.EnergiaCambiada += ActualizarEnergiaJugador;
        enemigo.VidaCambiada += ActualizarVidaEnemigo;

        // Al conectar se muestran los valores actuales sin animación.
        ActualizarBarra(barraVida, textoVida, jugador.Vida, jugador.VidaMaxima, true);
        ActualizarBarra(barraEnergia, textoEnergia, jugador.Energia, jugador.EnergiaMaxima, true);
        ActualizarBarra(barraVidaEnemigo, null, enemigo.Vida, enemigo.VidaMaxima, true);
    }

    public void Desconectar()
    {
        DesconectarCombatientes();
        DesconectarEstadisticas();
    }

    private void DesconectarCombatientes()
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

    private void DesconectarEstadisticas()
    {
        if (estadisticasJugador != null)
        {
            estadisticasJugador.AlCambiarVida -= ActualizarVidaJugador;
            estadisticasJugador.AlCambiarEnergia -= ActualizarEnergiaJugador;
            estadisticasJugador = null;
        }
    }

    public bool ConservarVisibleFueraDe(GameObject raizQueSeOculta)
    {
        return PrepararVisibleEnCanvas();
    }

    private void OnDestroy()
    {
        Desconectar();
    }

    private void ActualizarVidaJugador(int actual, int maximo)
    {
        ActualizarBarra(barraVida, textoVida, actual, maximo, false);
    }

    private void ActualizarEnergiaJugador(int actual, int maximo)
    {
        ActualizarBarra(barraEnergia, textoEnergia, actual, maximo, false);
    }

    private void ActualizarVidaEnemigo(int actual, int maximo)
    {
        ActualizarBarra(barraVidaEnemigo, null, actual, maximo, false);
    }

    private void ActualizarBarra(BarraImagen barra, TMP_Text texto, int actual, int maximo, bool instantaneo)
    {
        if (barra != null)
        {
            barra.Establecer(maximo > 0 ? (float)actual / maximo : 0f, instantaneo);
        }

        if (texto != null)
        {
            texto.text = actual + " / " + maximo;
        }
    }
}