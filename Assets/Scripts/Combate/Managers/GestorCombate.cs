using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class GestorCombate : MonoBehaviour
{
    public static bool EnCombate { get; private set; }

    [Header("Jugador")]
    [SerializeField] private MovimientoJugador movimientoJugador;
    [SerializeField] private EstadisticasJugador estadisticasJugador;
    [SerializeField] private Combatiente jugador;
    [SerializeField] private EquipamientoCombateManager equipamiento;
    [SerializeField] private EfectosTemporalesCombate efectos;

    [Header("Enemigo")]
    [SerializeField] private Combatiente enemigo;

    [Header("Piezas del combate")]
    [SerializeField] private PanelCombateUI panel;
    [SerializeField] private BarrasCombateUI barras;
    [SerializeField] private MenuAccionesUI menu;
    [SerializeField] private BarraMultiplicadorUI barraMultiplicador;
    [SerializeField] private ArenaEsquive arena;
    [SerializeField] private RawImage imagenJugadorCombate;

    [Header("Reglas")]
    [SerializeField, Min(0)] private int energiaPorTurno = 5;
    [SerializeField] private TextosSistema textos = new TextosSistema();

    public FaseCombate FaseActual { get; private set; } = FaseCombate.Inactivo;

    // Úsalos para enganchar animaciones, música, game over, recompensas, etc.
    public event Action<FaseCombate> FaseCambiada;
    public event Action<ResultadoCombate> CombateTerminado;

    private DatosEnemigo datos;
    private ResultadoCombate? resultado;
    private bool turnoConsumido;
    private int numeroDeTurno;
    private GameObject raizInterfazCombate;

    private void Awake()
    {
        if (panel != null)
        {
            raizInterfazCombate = panel.transform.root.gameObject;
        }

        if (estadisticasJugador == null && movimientoJugador != null)
        {
            estadisticasJugador = movimientoJugador.GetComponent<EstadisticasJugador>();
        }

        if (barras != null)
        {
            barras.gameObject.SetActive(false);

            if (estadisticasJugador != null)
            {
                barras.VincularJugador(estadisticasJugador);
            }
        }
    }

    public void IniciarCombate(DatosEnemigo datosEnemigo)
    {
        if (EnCombate || datosEnemigo == null || !ReferenciasValidas())
        {
            return;
        }

        datos = datosEnemigo;
        resultado = null;
        numeroDeTurno = 0;
        EnCombate = true;

        raizInterfazCombate.SetActive(true);
        arena.CerrarInmediato();
        EstablecerVisibilidadImagenJugador(true);
        movimientoJugador.DesactivarMovimiento();
        enemigo.Configurar(datos.vidaMaxima, 0);
        jugador.RestaurarEnergia();
        efectos.Limpiar();

        StartCoroutine(BucleCombate());
    }

    private void OnDestroy()
    {
        EnCombate = false;
    }
    private IEnumerator BucleCombate()
    {
        yield return Fase(FaseCombate.Intro, RutinaIntro());

        while (true)
        {
            yield return Fase(FaseCombate.TurnoJugador, RutinaTurnoJugador());

            if (resultado.HasValue)
            {
                break;
            }

            yield return Fase(FaseCombate.PreparaTurnoRival, RutinaPreparaTurnoRival());
            yield return Fase(FaseCombate.TurnoRival, RutinaTurnoRival());

            if (jugador.EstaDerrotado)
            {
                resultado = ResultadoCombate.Derrota;
                break;
            }

            yield return Fase(FaseCombate.PreparaTurnoJugador, RutinaPreparaTurnoJugador());
        }

        yield return Fase(FaseCombate.Final, RutinaFinal());
    }

    private Coroutine Fase(FaseCombate fase, IEnumerator rutina)
    {
        FaseActual = fase;
        FaseCambiada?.Invoke(fase);
        return StartCoroutine(rutina);
    }

    private IEnumerator RutinaIntro()
    {
        panel.Mostrar(datos);
        menu.Ocultar();
        barraMultiplicador.Ocultar();
        barras.Conectar(jugador, enemigo);

        yield return Decir(FrasesEnemigo.Elegir(datos.frases.intro), true);
    }

    private IEnumerator RutinaTurnoJugador()
    {
        EstablecerVisibilidadImagenJugador(true);
        efectos.ProcesarInicioDeTurno(jugador);
        jugador.RecuperarEnergia(energiaPorTurno);

        yield return Decir(FrasesEnemigo.Elegir(datos.frases.turnoJugador));

        turnoConsumido = false;

        while (!turnoConsumido && !resultado.HasValue)
        {
            yield return StartCoroutine(menu.EsperarAccion(equipamiento, jugador));
            AccionJugador accion = menu.AccionElegida;

            switch (accion.tipo)
            {
                case TipoAccion.Atacar:
                    yield return StartCoroutine(EjecutarAtaque(accion.slot));
                    break;

                case TipoAccion.UsarConsumible:
                    yield return StartCoroutine(EjecutarConsumible(accion.slot));
                    break;

                case TipoAccion.Huir:
                    resultado = ResultadoCombate.Huida;
                    break;
            }
        }

        if (!resultado.HasValue && enemigo.EstaDerrotado)
        {
            resultado = ResultadoCombate.Victoria;
        }
    }

    private IEnumerator RutinaPreparaTurnoRival()
    {
        barras.PrepararVisibleEnCanvas();
        yield return Decir(FrasesEnemigo.Elegir(datos.frases.prepararTurnoRival));
        yield return StartCoroutine(arena.Entrar(
            () => EstablecerVisibilidadImagenJugador(false)
        ));
        barras.PrepararVisibleEnCanvas();
    }

    private IEnumerator RutinaTurnoRival()
    {
        PatronAtaqueDatos patron = datos.ElegirPatron(numeroDeTurno);

        if (patron != null)
        {
            yield return StartCoroutine(arena.EjecutarPatron(patron, jugador));
        }
    }

    private IEnumerator RutinaPreparaTurnoJugador()
    {
        yield return StartCoroutine(arena.Salir(
            () => EstablecerVisibilidadImagenJugador(true)
        ));

        numeroDeTurno++;
        yield return Decir(FrasesEnemigo.Elegir(datos.frases.prepararTurnoJugador));
    }

    private IEnumerator RutinaFinal()
    {
        arena.CerrarInmediato();
        EstablecerVisibilidadImagenJugador(true);
        menu.Ocultar();

        string[] frases;

        switch (resultado.Value)
        {
            case ResultadoCombate.Victoria:
                frases = datos.frases.victoria;
                break;
            case ResultadoCombate.Derrota:
                frases = datos.frases.derrota;
                break;
            default:
                frases = datos.frases.huida;
                break;
        }

        yield return Decir(FrasesEnemigo.Elegir(frases));

        panel.OcultarConBarras(barras);
        if (estadisticasJugador != null)
        {
            barras.VincularJugador(estadisticasJugador);
        }
        efectos.Limpiar();
        movimientoJugador.ActivarMovimiento();

        raizInterfazCombate.SetActive(false);
        EnCombate = false;
        FaseActual = FaseCombate.Inactivo;
        CombateTerminado?.Invoke(resultado.Value);
    }

    private void EstablecerVisibilidadImagenJugador(bool visible)
    {
        if (imagenJugadorCombate != null)
        {
            imagenJugadorCombate.enabled = visible;
        }
    }

    private IEnumerator EjecutarAtaque(SlotEspecial slot)
    {
        if (slot == null || slot.EstaVacio() || !barraMultiplicador.PuedeEjecutarse)
        {
            Debug.LogError("No se puede atacar sin una barra multiplicadora configurada.", this);
            yield break;
        }

        DatosObjeto_Base objeto = slot.Objeto;
        IAtaqueCombate ataque = objeto as IAtaqueCombate;

        if (ataque == null)
        {
            yield break;
        }

        int cantidadHechizos = objeto is DatosHechizo hechizo
            ? hechizo.cantidadConsumidaPorUso
            : 0;

        if (cantidadHechizos > 0 && slot.Cantidad < cantidadHechizos)
        {
            yield break;
        }

        if (!jugador.GastarEnergia(ataque.CostoEnergia))
        {
            yield break;
        }

        yield return StartCoroutine(barraMultiplicador.Ejecutar());

        if (!barraMultiplicador.UltimaEjecucionConfirmada)
        {
            jugador.RecuperarEnergia(ataque.CostoEnergia);
            yield break;
        }

        if (cantidadHechizos > 0 &&
            !equipamiento.ConsumirDesdeSlot(slot, cantidadHechizos))
        {
            jugador.RecuperarEnergia(ataque.CostoEnergia);
            yield break;
        }

        int danio = Mathf.RoundToInt(
            ataque.DanioBase * barraMultiplicador.UltimoMultiplicador * efectos.MultiplicadorDanio
        );
        enemigo.RecibirDanio(danio);

        string texto = textos.usarAtaque
            .Replace("{objeto}", objeto.nombreObjeto)
            .Replace("{danio}", danio.ToString());

        yield return Decir(texto);
        turnoConsumido = true;
    }

    private IEnumerator EjecutarConsumible(SlotEspecial slot)
    {
        DatosObjeto_Base objeto = slot.Objeto;
        IConsumibleCombate consumible = objeto as IConsumibleCombate;

        if (consumible == null || !equipamiento.ConsumirDesdeSlot(slot, 1))
        {
            yield break;
        }

        jugador.Curar(consumible.CuracionInstantanea);
        jugador.RecuperarEnergia(consumible.EnergiaInstantanea);
        efectos.Agregar(consumible);

        yield return Decir(textos.usarConsumible.Replace("{objeto}", objeto.nombreObjeto));
        turnoConsumido = true;
    }

    private Coroutine Decir(string frase, bool esperarConfirmacion = false)
    {
        frase = (frase ?? string.Empty).Replace("{enemigo}", datos.nombre);
        return StartCoroutine(panel.MostrarTexto(frase, esperarConfirmacion));
    }

    private bool ReferenciasValidas()
    {
        if (movimientoJugador && jugador && equipamiento && efectos && enemigo &&
            panel && barras && menu && barraMultiplicador && arena &&
            imagenJugadorCombate)
        {
            return true;
        }

        Debug.LogError("Faltan referencias en el GestorCombate.", this);
        return false;
    }
}