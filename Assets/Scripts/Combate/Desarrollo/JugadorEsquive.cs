using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Personaje que esquiva dentro del marco. Necesita Collider2D (Is Trigger).
// El Rigidbody2D (Kinematic) solo sirve para que Unity detecte los triggers.
[RequireComponent(typeof(Rigidbody2D))]
public class JugadorEsquive : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Combatiente stats;
    [SerializeField] private ControlesCombate controles;

    [Header("Movimiento")]
    [SerializeField, Min(0.1f)] private float velocidad = 5f;

    [Header("Parry (espacio)")]
    [SerializeField, Min(0.05f)] private float ventanaParry = 0.15f;
    [SerializeField, Min(0f)] private float enfriamientoParry = 0.6f;
    [SerializeField, Min(0)] private int energiaPorParry = 5;

    [Header("Dash")]
    [SerializeField, Min(0.1f)] private float distanciaDash = 1.5f;
    [SerializeField, Min(0.05f)] private float duracionDash = 0.15f;
    [SerializeField, Min(0f)] private float enfriamientoDash = 0.8f;
    [SerializeField] private bool invulnerableEnDash = true;

    [Header("Daño")]
    [SerializeField, Min(0f)] private float invulnerabilidadTrasGolpe = 1f;

    [Header("Eventos (animaciones / sonidos)")]
    [SerializeField] private UnityEvent alParrear;
    [SerializeField] private UnityEvent alRecibirGolpe;
    [SerializeField] private UnityEvent alHacerDash;

    private ArenaEsquive arena;
    private Vector3 posicionReposo;
    private Vector2 ultimaDireccion = Vector2.right;
    private bool controlActivo;
    private bool parryActivo;
    private bool dashEnCurso;
    private bool invulnerableTrasGolpe;
    private float proximoParry;
    private float proximoDash;

    public Vector3 PosicionReposo => posicionReposo;

    private bool EsInvulnerable =>
        invulnerableTrasGolpe || (dashEnCurso && invulnerableEnDash);

    private void Awake()
    {
        posicionReposo = transform.position;

        Rigidbody2D cuerpo = GetComponent<Rigidbody2D>();
        cuerpo.bodyType = RigidbodyType2D.Kinematic;
        cuerpo.gravityScale = 0f;
    }

    private void Update()
    {
        if (!controlActivo)
        {
            return;
        }

        Vector2 entrada = controles.Mover;

        if (entrada.sqrMagnitude > 0.01f)
        {
            ultimaDireccion = entrada.normalized;
        }

        if (!dashEnCurso)
        {
            Vector3 destino = transform.position +
                              (Vector3)(Vector2.ClampMagnitude(entrada, 1f) * velocidad * Time.deltaTime);
            transform.position = arena.Limitar(destino);
        }

        if (controles.ParryPresionado && Time.time >= proximoParry)
        {
            StartCoroutine(VentanaDeParry());
        }

        if (controles.DashPresionado && !dashEnCurso && Time.time >= proximoDash)
        {
            StartCoroutine(Dash());
        }
    }

    public void ActivarControl(ArenaEsquive nuevaArena)
    {
        arena = nuevaArena;
        controlActivo = true;
    }

    public void DesactivarControl()
    {
        StopAllCoroutines();
        controlActivo = false;
        parryActivo = false;
        dashEnCurso = false;
        invulnerableTrasGolpe = false;
    }

    // Lo llama el proyectil cuando toca al jugador.
    public void ProcesarImpacto(Proyectil proyectil)
    {
        if (!controlActivo || stats.EstaDerrotado)
        {
            return;
        }

        if (parryActivo && proyectil.EsParreable)
        {
            proyectil.Destruir();
            stats.RecuperarEnergia(energiaPorParry);
            alParrear.Invoke();
            return;
        }

        // Con invulnerabilidad el proyectil lo atraviesa.
        if (EsInvulnerable)
        {
            return;
        }

        stats.RecibirDanio(proyectil.Danio);
        proyectil.Destruir();
        alRecibirGolpe.Invoke();
        StartCoroutine(InvulnerabilidadTrasGolpe());
    }

    // Salto en arco entre el punto de reposo y el marco.
    public IEnumerator SaltarA(Vector3 destino, float altura, float duracion)
    {
        Vector3 inicio = transform.position;
        float progreso = 0f;

        while (progreso < 1f)
        {
            progreso += Time.deltaTime / duracion;
            float k = Mathf.Clamp01(progreso);

            transform.position = Vector3.Lerp(inicio, destino, k) +
                                 Vector3.up * (altura * 4f * k * (1f - k));
            yield return null;
        }

        transform.position = destino;
    }

    private IEnumerator VentanaDeParry()
    {
        proximoParry = Time.time + enfriamientoParry;
        parryActivo = true;
        yield return new WaitForSeconds(ventanaParry);
        parryActivo = false;
    }

    private IEnumerator Dash()
    {
        dashEnCurso = true;
        proximoDash = Time.time + enfriamientoDash;
        alHacerDash.Invoke();

        Vector3 inicio = transform.position;
        Vector3 destino = arena.Limitar(inicio + (Vector3)(ultimaDireccion * distanciaDash));
        float tiempo = 0f;

        while (tiempo < duracionDash)
        {
            tiempo += Time.deltaTime;
            transform.position = Vector3.Lerp(inicio, destino, tiempo / duracionDash);
            yield return null;
        }

        transform.position = destino;
        dashEnCurso = false;
    }

    private IEnumerator InvulnerabilidadTrasGolpe()
    {
        invulnerableTrasGolpe = true;
        yield return new WaitForSeconds(invulnerabilidadTrasGolpe);
        invulnerableTrasGolpe = false;
    }
}
