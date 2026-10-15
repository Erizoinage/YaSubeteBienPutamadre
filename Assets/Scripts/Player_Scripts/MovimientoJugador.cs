using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class MovimientoJugador : MonoBehaviour
{
    [SerializeField] private float velocidad = 5f;

    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 movimiento;
    private Vector2 ultimaDireccion = Vector2.down;

    private const float UmbralInput = 0.1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movimiento * velocidad * Time.fixedDeltaTime);
    }

    // Llamado por PlayerInput (Send Messages)
    public void OnMove(InputValue value)
    {
        movimiento = value.Get<Vector2>();

        bool seMueve = movimiento.magnitude > UmbralInput;
        if (seMueve)
        {
            float ax = Mathf.Abs(movimiento.x);
            float ay = Mathf.Abs(movimiento.y);

            if (ax > ay)
                ultimaDireccion = new Vector2(Mathf.Sign(movimiento.x), 0);
            else if (ay > ax)
                ultimaDireccion = new Vector2(0, Mathf.Sign(movimiento.y));
        }

        animator.SetFloat("MoveX", ultimaDireccion.x);
        animator.SetFloat("MoveY", ultimaDireccion.y);
        animator.SetFloat("Speed", seMueve ? 1f : 0f);
        if (ultimaDireccion.x != 0)
        {
            Vector3 escala = transform.localScale;
            escala.x = Mathf.Abs(escala.x) * Mathf.Sign(ultimaDireccion.x);
            transform.localScale = escala;
        }
    }
}