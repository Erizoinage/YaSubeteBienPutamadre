using UnityEngine;
using UnityEngine.InputSystem;

public class InteraccionJugador : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private MovimientoJugador movimientoJugador;
    [SerializeField] private Transform zonaInteraccion;
    [SerializeField] private ZonaInteraccion detectorInteraccion;

    [Header("Configuración")]
    [SerializeField] private float distanciaInteraccion = 0.6f;

    private bool interaccionActiva;

    private void Start()
    {
        ActivarInteraccion();
        ActualizarZonaInteraccion();
    }

    private void Update()
    {
        ActualizarZonaInteraccion();
    }

    private void ActualizarZonaInteraccion()
    {
        if (movimientoJugador == null || zonaInteraccion == null)
            return;

        Vector2 direccion =
            movimientoJugador.UltimaDireccion;

        zonaInteraccion.localPosition =
            direccion * distanciaInteraccion;
    }

    public void OnInteract(
        InputAction.CallbackContext contexto
    )
    {
        if (!interaccionActiva)
            return;

        if (!contexto.performed)
            return;

        IntentarInteractuar();
    }

    private void IntentarInteractuar()
    {
        if (detectorInteraccion == null)
        {
            Debug.LogWarning(
                "No se asignó el detector de interacción."
            );

            return;
        }

        ITocable objetoDetectado =
            detectorInteraccion.ObtenerObjetoMasCercano();

        if (objetoDetectado == null)
        {
            Debug.Log(
                "No hay ningún objeto interactuable delante."
            );

            return;
        }

        objetoDetectado.Interactuar();
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
}