using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Controla al jugador dentro del Canvas de la arena.
[RequireComponent(typeof(RectTransform), typeof(UnityEngine.UI.Image))]
public class JugadorEsquive : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Combatiente stats;
    [SerializeField] private ControlesCombate controles;

    [Header("Movimiento")]
    [SerializeField, Min(0.1f)] private float velocidad = 9f;

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
    [SerializeField] private UnityEvent alParrear = new UnityEvent();
    [SerializeField] private UnityEvent alRecibirGolpe = new UnityEvent();
    [SerializeField] private UnityEvent alHacerDash = new UnityEvent();

    private RectTransform rectTransform;
    private ArenaEsquive arena;
    private Vector3 posicionReposoLocal;
    private Vector2 ultimaDireccion = Vector2.right;
    private bool controlActivo;
    private bool parryActivo;
    private bool dashEnCurso;
    private bool invulnerableTrasGolpe;
    private float proximoParry;
    private float proximoDash;

    public Vector3 PosicionReposo =>
        transform.parent != null
            ? transform.parent.TransformPoint(posicionReposoLocal)
            : posicionReposoLocal;

    private bool EsInvulnerable =>
        invulnerableTrasGolpe || (dashEnCurso && invulnerableEnDash);

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        posicionReposoLocal = transform.localPosition;
    }

    private void Update()
    {
        if (!controlActivo)
        {
            return;
        }

        if (controles == null || arena == null || stats == null)
        {
            Debug.LogError("JugadorEsquive necesita controles, arena y stats.", this);
            DesactivarControl();
            return;
        }

        Vector2 entrada = Vector2.ClampMagnitude(controles.Mover, 1f);

        if (entrada.sqrMagnitude > 0.01f)
        {
            ultimaDireccion = entrada.normalized;
        }

        if (!dashEnCurso)
        {
            Vector2 desplazamiento = arena.ConvertirMovimiento(
                entrada * velocidad * Time.deltaTime
            );
            transform.position = arena.Limitar(
                arena.MoverEnCanvas(transform.position, desplazamiento),
                rectTransform
            );
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
        if (nuevaArena == null)
        {
            Debug.LogError("No se puede activar JugadorEsquive sin una arena.", this);
            return;
        }

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

    // Lo llama el proyectil cuando su rectángulo visual toca al jugador.
    public void ProcesarImpacto(Proyectil proyectil)
    {
        if (!controlActivo || stats == null || stats.EstaDerrotado)
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

        if (EsInvulnerable)
        {
            return;
        }

        stats.RecibirDanio(proyectil.Danio);
        proyectil.Destruir();
        alRecibirGolpe.Invoke();
        StartCoroutine(InvulnerabilidadTrasGolpe());
    }

    public bool Contiene(RectTransform otro)
    {
        return arena != null && arena.SeSuperponen(rectTransform, otro);
    }

    public IEnumerator SaltarA(
        Vector3 destino,
        float altura,
        float duracion,
        ArenaEsquive arenaMovimiento
    )
    {
        if (arenaMovimiento == null)
        {
            yield break;
        }

        RectTransform espacio = arenaMovimiento.EspacioCoordenadas;
        Vector3 inicioLocal = espacio.InverseTransformPoint(transform.position);
        Vector3 destinoLocal = espacio.InverseTransformPoint(destino);
        Vector2 desplazamientoVertical = arenaMovimiento.ConvertirUnidades(
            new Vector2(0f, altura)
        );
        float progreso = 0f;
        float duracionSegura = Mathf.Max(0.01f, duracion);

        while (progreso < 1f)
        {
            progreso += Time.deltaTime / duracionSegura;
            float k = Mathf.Clamp01(progreso);
            Vector3 local = Vector3.Lerp(inicioLocal, destinoLocal, k);
            local.y += desplazamientoVertical.y * 4f * k * (1f - k);
            transform.position = espacio.TransformPoint(local);
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

        Vector2 desplazamiento = arena.ConvertirMovimiento(
            ultimaDireccion * distanciaDash
        );
        Vector3 inicio = transform.position;
        Vector3 destino = arena.Limitar(
            arena.MoverEnCanvas(inicio, desplazamiento),
            rectTransform
        );
        float tiempo = 0f;
        float duracionSegura = Mathf.Max(0.01f, duracionDash);

        while (tiempo < duracionSegura)
        {
            tiempo += Time.deltaTime;
            transform.position = Vector3.Lerp(inicio, destino, tiempo / duracionSegura);
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
