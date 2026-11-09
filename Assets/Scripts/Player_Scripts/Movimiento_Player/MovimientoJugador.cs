using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class MovimientoJugador : MonoBehaviour
{
    [Header("Configuración de movimiento")]
    [SerializeField] private float velocidad = 5f;

    [Header("Equipamiento visual")]
    [SerializeField] private VisualEquipamiento visualEquipamiento;

    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 movimiento;
    private Vector2 ultimaDireccion = Vector2.down;
    public Vector2 UltimaDireccion => ultimaDireccion;

    private bool movimientoActivo;

    private const float UmbralInput = 0.1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (visualEquipamiento == null)
        {
            visualEquipamiento = GetComponent<VisualEquipamiento>();
        }
    }

    private void Start()
    {
        ActivarMovimiento();
    }

    private void FixedUpdate()
    {
        if (!movimientoActivo)
            return;

        rb.MovePosition(
            rb.position + movimiento * velocidad * Time.fixedDeltaTime
        );
    }

    // Esta función se conecta en Player Input usando Invoke Unity Events.
    public void OnMove(InputAction.CallbackContext contexto)
    {
        if (!movimientoActivo)
            return;

        movimiento = contexto.ReadValue<Vector2>();

        bool seMueve = movimiento.magnitude > UmbralInput;

        if (seMueve)
        {
            float movimientoX = Mathf.Abs(movimiento.x);
            float movimientoY = Mathf.Abs(movimiento.y);

            if (movimientoX >= movimientoY && movimientoX > UmbralInput)
            {
                ultimaDireccion = new Vector2(
                    Mathf.Sign(movimiento.x),
                    0
                );
            }
            else if (movimientoY > movimientoX)
            {
                ultimaDireccion = new Vector2(
                    0,
                    Mathf.Sign(movimiento.y)
                );
            }
        }

        ActualizarAnimaciones(seMueve);
        ActualizarDireccionVisual();
        ActualizarDireccionEquipamiento();
    }

    private void ActualizarAnimaciones(bool seMueve)
    {
        if (animator == null)
            return;

        animator.SetFloat("MoveX", ultimaDireccion.x);
        animator.SetFloat("MoveY", ultimaDireccion.y);
        animator.SetFloat("Speed", seMueve ? 1f : 0f);
    }

    private void ActualizarDireccionVisual()
    {
        if (ultimaDireccion.x == 0)
            return;

        Vector3 escala = transform.localScale;

        escala.x = Mathf.Abs(escala.x) *
                   Mathf.Sign(ultimaDireccion.x);

        transform.localScale = escala;
    }

    public void ActivarMovimiento()
    {
        movimientoActivo = true;
    }
    public void DesactivarMovimiento()
    {
        movimientoActivo = false;
        movimiento = Vector2.zero;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        ActualizarAnimaciones(false);
    }

    public void EstablecerMovimientoActivo(bool estado)
    {
        if (estado)
            ActivarMovimiento();
        else
            DesactivarMovimiento();
    }

    public bool EstaActivoElMovimiento()
    {
        return movimientoActivo;
    }

    public void EstablecerEquipamientoVisual(
        DatosEquipable casco,
        DatosEquipable armadura
    )
    {
        if (visualEquipamiento == null)
        {
            visualEquipamiento = GetComponent<VisualEquipamiento>();
        }

        if (visualEquipamiento == null)
        {
            Debug.LogWarning("El jugador no tiene VisualEquipamiento configurado.", this);
            return;
        }

        visualEquipamiento.EstablecerEquipamiento(casco, armadura);
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (GestorCombate.EnCombate) return;

            InventoryManager inventoryManager =
                FindAnyObjectByType<InventoryManager>();

            if (inventoryManager == null)
            {
                Debug.LogWarning(
                    "No se encontró un InventoryManager en la escena.",
                    this
                );

                return;
            }

            inventoryManager.CambiarEstadoInventario();
        }
    }

    private void ActualizarDireccionEquipamiento()
    {
        if (visualEquipamiento == null)
        {
            return;
        }

        DireccionJugador direccion = DireccionJugador.Abajo;

        if (ultimaDireccion.x != 0)
        {
            direccion = ultimaDireccion.x > 0
                ? DireccionJugador.Derecha
                : DireccionJugador.Izquierda;
        }
        else if (ultimaDireccion.y > 0)
        {
            direccion = DireccionJugador.Arriba;
        }

        visualEquipamiento.CambiarDireccion(direccion);
    }
}