using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class CampoDeVision_Enemigo : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform jugador;

    [Header("Vision")]
    [Min(0.1f)]
    [SerializeField] private float rangoVision = 5f;

    [Tooltip("Capas que bloquean la vista del enemigo, como paredes.")]
    [SerializeField] private LayerMask capasObstaculos;

    [Header("Movimiento")]
    [Min(0.1f)]
    [SerializeField] private float velocidadMovimiento = 2.5f;

    [Min(0f)]
    [SerializeField] private float distanciaParaDetenerse = 0.1f;

    [Min(0.1f)]
    [SerializeField] private float distanciaParaIniciarCombate = 0.5f;

    [Header("Combate")]
    [SerializeField] private ControladorCombateInicio controladorCombate;

    [Tooltip("Desactiva al enemigo del escenario cuando comienza el combate.")]
    [SerializeField] private bool ocultarAlIniciarCombate = true;

    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 direccionMovimiento;
    private bool jugadorDetectado;
    private bool combateIniciado;

    public bool JugadorDetectado => jugadorDetectado;
    public bool CombateIniciado => combateIniciado;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        BuscarReferenciasFaltantes();
        ConfigurarRigidbody();
    }

    private void Update()
    {
        if (combateIniciado)
        {
            DetenerMovimiento();
            return;
        }

        EvaluarVision();
        ActualizarAnimacion();
    }

    private void FixedUpdate()
    {
        if (!jugadorDetectado || combateIniciado)
        {
            return;
        }

        MoverHaciaJugador();
    }

    private void EvaluarVision()
    {
        if (jugador == null)
        {
            jugadorDetectado = false;
            DetenerMovimiento();
            return;
        }

        float distanciaAlJugador = Vector2.Distance(
            rb.position,
            jugador.position
        );

        if (distanciaAlJugador <= distanciaParaIniciarCombate)
        {
            IntentarIniciarCombate(jugador);
            DetenerMovimiento();
            return;
        }

        bool estaDentroDelRango =
            distanciaAlJugador <= rangoVision;

        bool demasiadoCerca =
            distanciaAlJugador <= distanciaParaDetenerse;

        bool visionBloqueada =
            HayObstaculoHaciaJugador();

        jugadorDetectado =estaDentroDelRango &&!demasiadoCerca &&!visionBloqueada;

        if (!jugadorDetectado)
        {
            DetenerMovimiento();
            return;
        }

        direccionMovimiento =
            ((Vector2)jugador.position - rb.position).normalized;
    }

    private void MoverHaciaJugador()
    {
        Vector2 nuevaPosicion =rb.position +direccionMovimiento *velocidadMovimiento *
            Time.fixedDeltaTime;

        rb.MovePosition(nuevaPosicion);
    }

    private bool HayObstaculoHaciaJugador()
    {
        if (jugador == null || capasObstaculos.value == 0)
        {
            return false;
        }

        Vector2 origen = rb.position;
        Vector2 destino = jugador.position;
        Vector2 direccion = (destino - origen).normalized;
        float distancia = Vector2.Distance(origen, destino);

        RaycastHit2D impacto = Physics2D.Raycast(
            origen,
            direccion,
            distancia,
            capasObstaculos
        );

        return impacto.collider != null;
    }

    private void DetenerMovimiento()
    {
        jugadorDetectado = false;
        direccionMovimiento = Vector2.zero;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void ActualizarAnimacion()
    {
        if (animator == null)
        {
            return;
        }

        bool seMueve =
            jugadorDetectado &&
            direccionMovimiento.sqrMagnitude > 0.01f;

        animator.SetFloat(
            "MoveX",
            direccionMovimiento.x
        );

        animator.SetFloat(
            "MoveY",
            direccionMovimiento.y
        );

        animator.SetFloat(
            "Speed",
            seMueve ? 1f : 0f
        );
    }

    private void OnCollisionEnter2D(Collision2D colision)
    {
        IntentarIniciarCombate(colision.transform);
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        IntentarIniciarCombate(otro.transform);
    }

    private void IntentarIniciarCombate(Transform objetoDetectado)
    {
        if (combateIniciado || objetoDetectado == null)
        {
            return;
        }

        Transform raizObjeto = objetoDetectado.root;

        if (!raizObjeto.CompareTag("Player"))
        {
            return;
        }

        if (controladorCombate == null)
        {
            controladorCombate =
                FindAnyObjectByType<ControladorCombateInicio>();
        }

        if (controladorCombate == null)
        {
            Debug.LogWarning(
                name + " no encuentra un ControladorCombateInicio.",
                this
            );

            return;
        }

        if (!controladorCombate.IniciarCombate())
        {
            return;
        }

        combateIniciado = true;
        DetenerMovimiento();

        if (ocultarAlIniciarCombate)
        {
            gameObject.SetActive(false);
        }
    }

    private void BuscarReferenciasFaltantes()
    {
        if (jugador == null)
        {
            GameObject objetoJugador =
                GameObject.FindGameObjectWithTag("Player");

            if (objetoJugador != null)
            {
                jugador = objetoJugador.transform;
            }
        }

        if (controladorCombate == null)
        {
            controladorCombate =
                FindAnyObjectByType<ControladorCombateInicio>();
        }
    }

    private void ConfigurarRigidbody()
    {
        if (rb == null)
        {
            return;
        }

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    public void ReactivarEnemigo()
    {
        combateIniciado = false;
        jugadorDetectado = false;
        direccionMovimiento = Vector2.zero;

        gameObject.SetActive(true);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position,
            rangoVision
        );

        if (jugador != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(
                transform.position,
                jugador.position
            );
        }
    }

    private void OnValidate()
    {
        rangoVision = Mathf.Max(0.1f, rangoVision);
        velocidadMovimiento =
            Mathf.Max(0.1f, velocidadMovimiento);

        distanciaParaDetenerse =
            Mathf.Max(0f, distanciaParaDetenerse);

        distanciaParaIniciarCombate =
            Mathf.Max(0.1f, distanciaParaIniciarCombate);
    }
}