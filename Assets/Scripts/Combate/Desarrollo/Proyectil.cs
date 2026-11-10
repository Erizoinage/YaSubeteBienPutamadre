using UnityEngine;
using UnityEngine.UI;

// Prefab de UI: RectTransform + Image, hijo del contenedor de proyectiles del Canvas.
[RequireComponent(typeof(RectTransform), typeof(Image))]
public class Proyectil : MonoBehaviour
{
    [SerializeField] private bool esParreable = true;
    [SerializeField, Min(0.5f)] private float tiempoDeVidaMaximo = 10f;

    private RectTransform rectTransform;
    private Vector2 direccion;
    private float velocidad;
    private int danio;
    private JugadorEsquive objetivo;
    private RectTransform espacioCoordenadas;
    private ArenaEsquive arena;
    private bool impactoProcesado;
    private bool errorCanvasReportado;

    public int Danio => danio;
    public bool EsParreable => esParreable;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        ResolverEspacioCoordenadas();
    }

    public void Iniciar(Vector2 nuevaDireccion, float nuevaVelocidad, int nuevoDanio)
    {
        Iniciar(
            nuevaDireccion,
            nuevaVelocidad,
            nuevoDanio,
            null,
            transform.parent as RectTransform,
            null
        );
    }

    public void Iniciar(
        Vector2 nuevaDireccion,
        float nuevaVelocidad,
        int nuevoDanio,
        JugadorEsquive nuevoObjetivo,
        RectTransform nuevoEspacio,
        ArenaEsquive nuevaArena
    )
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        if (nuevaDireccion.sqrMagnitude <= 0.0001f)
        {
            Debug.LogError("Un proyectil necesita una dirección distinta de cero.", this);
            Destroy(gameObject);
            return;
        }

        direccion = nuevaDireccion.normalized;
        velocidad = Mathf.Max(0f, nuevaVelocidad);
        danio = Mathf.Max(0, nuevoDanio);
        objetivo = nuevoObjetivo;
        espacioCoordenadas = nuevoEspacio;
        arena = nuevaArena;
        if (nuevaArena != null)
        {
            espacioCoordenadas = nuevaArena.EspacioCoordenadas;
        }

        ResolverEspacioCoordenadas();
        if (espacioCoordenadas == null)
        {
            Debug.LogError(
                "Proyectil: asígnalo como hijo de un contenedor UI dentro del Canvas de combate.",
                this
            );
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject, Mathf.Max(0.5f, tiempoDeVidaMaximo));
    }

    private void Update()
    {
        if (espacioCoordenadas == null)
        {
            ResolverEspacioCoordenadas();
            if (espacioCoordenadas == null)
            {
                if (!errorCanvasReportado)
                {
                    Debug.LogError(
                        "El proyectil no está conectado al Canvas de la arena. " +
                        "Debe ser hijo de un contenedor UI bajo el Canvas de combate.",
                        this
                    );
                    errorCanvasReportado = true;
                }

                Destroy(gameObject);
                return;
            }
        }

        Vector3 local = espacioCoordenadas.InverseTransformPoint(transform.position);
        Vector2 desplazamiento = arena != null
            ? arena.ConvertirUnidades(direccion * velocidad * Time.deltaTime)
            : direccion * velocidad * Time.deltaTime;
        local += (Vector3)desplazamiento;
        transform.position = espacioCoordenadas.TransformPoint(local);

        if (!impactoProcesado && objetivo != null && objetivo.Contiene(rectTransform))
        {
            impactoProcesado = objetivo.ProcesarImpacto(this);
        }
    }

    public void Destruir()
    {
        Destroy(gameObject);
    }

    private void ResolverEspacioCoordenadas()
    {
        if (espacioCoordenadas != null)
        {
            return;
        }

        if (arena != null && arena.EspacioCoordenadas != null)
        {
            espacioCoordenadas = arena.EspacioCoordenadas;
            return;
        }

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            espacioCoordenadas = canvas.rootCanvas.transform as RectTransform;
            return;
        }

        espacioCoordenadas = transform.parent as RectTransform;
    }
}
