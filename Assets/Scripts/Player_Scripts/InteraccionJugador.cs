using UnityEngine;
using UnityEngine.InputSystem;

public class InteraccionJugador : MonoBehaviour
{
    [SerializeField] private MovimientoJugador movimientoJugador;
    [SerializeField] private Transform puntoInteraccion;
    [SerializeField] private float distanciaInteraccion = 0.6f;
    [SerializeField] private float radioInteraccion = 0.35f;
    [SerializeField] private LayerMask capaNPC;

    private bool interaccionActiva;

    private void Start()
    {
        ActivarInteraccion();
        ActualizarPuntoInteraccion();
    }

    private void Update()
    {
        ActualizarPuntoInteraccion();
    }

    private void ActualizarPuntoInteraccion()
    {
        if (movimientoJugador == null || puntoInteraccion == null)
            return;

        Vector2 direccion = movimientoJugador.UltimaDireccion;

        puntoInteraccion.localPosition =direccion * distanciaInteraccion;
    }

    public void OnInteract(InputAction.CallbackContext contexto)
    {
        if (!interaccionActiva)
            return;

        if (!contexto.performed)
            return;

        BuscarNPC();
    }

    private void BuscarNPC()
    {
        if (puntoInteraccion == null)
            return;

        Collider2D resultado = Physics2D.OverlapCircle(
            puntoInteraccion.position,
            radioInteraccion,
            capaNPC
        );

        if (resultado == null)
        {
            Debug.Log("No hay un NPC delante");
            return;
        }

        Debug.Log("NPC detectado: " + resultado.gameObject.name);
    }

    public void ActivarInteraccion()
    {
        interaccionActiva = true;
    }

    public void DesactivarInteraccion()
    {
        interaccionActiva = false;
    }

    public void EstablecerInteraccionActiva(bool estado)
    {
        interaccionActiva = estado;
    }

    public bool EstaActivaLaInteraccion()
    {
        return interaccionActiva;
    }

    private void OnDrawGizmosSelected()
    {
        if (puntoInteraccion == null)
            return;

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            puntoInteraccion.position,
            radioInteraccion
        );
    }
}