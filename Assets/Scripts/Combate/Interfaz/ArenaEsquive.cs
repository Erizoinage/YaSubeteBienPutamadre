using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Coordina el turno de esquive dentro del rectángulo de UI de un único Canvas.
public class ArenaEsquive : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private GameObject marcoVisual;
    [SerializeField] private Transform centroMarco;
    [SerializeField] private RectTransform espacioCoordenadas;
    [SerializeField] private Vector2 tamano = new Vector2(6f, 3f);

    [Header("Jugador")]
    [SerializeField] private JugadorEsquive jugador;
    [SerializeField, Min(0f)] private float alturaSalto = 1.2f;
    [SerializeField, Min(0.05f)] private float duracionSalto = 0.5f;

    [Header("Proyectiles")]
    [SerializeField] private RectTransform contenedorProyectiles;
    [SerializeField, Min(0f)] private float margenSpawn = 0.05f;

    public RectTransform EspacioCoordenadas
    {
        get
        {
            ResolverEspacioCoordenadas();
            return espacioCoordenadas;
        }
    }

    public Rect Limites
    {
        get
        {
            ResolverEspacioCoordenadas();

            if (centroMarco is RectTransform marco)
            {
                return ObtenerLimites(marco);
            }

            Vector3 centro = espacioCoordenadas != null
                ? espacioCoordenadas.InverseTransformPoint(centroMarco.position)
                : centroMarco.position;
            Vector2 dimensiones = Vector2.Scale(tamano, EscalaCanvas);
            return new Rect((Vector2)centro - dimensiones * 0.5f, dimensiones);
        }
    }

    private Vector2 EscalaCanvas
    {
        get
        {
            Rect limites = centroMarco is RectTransform marco
                ? ObtenerLimites(marco)
                : new Rect(Vector2.zero, tamano);

            return new Vector2(
                limites.width / Mathf.Max(0.01f, tamano.x),
                limites.height / Mathf.Max(0.01f, tamano.y)
            );
        }
    }

    public Vector2 ConvertirUnidades(Vector2 unidades)
    {
        Vector2 escala = EscalaCanvas;
        return Vector2.Scale(unidades, escala);
    }

    public Vector3 Limitar(Vector3 posicionMundo)
    {
        return Limitar(posicionMundo, Vector2.zero);
    }

    public Vector3 Limitar(Vector3 posicionMundo, RectTransform objeto)
    {
        Vector2 medioObjeto = objeto != null
            ? ObtenerTamanoEnEspacio(objeto) * 0.5f
            : Vector2.zero;
        return Limitar(posicionMundo, medioObjeto);
    }

    public Vector3 MoverEnCanvas(Vector3 posicionMundo, Vector2 desplazamiento)
    {
        if (espacioCoordenadas == null)
        {
            return posicionMundo;
        }

        Vector3 local = espacioCoordenadas.InverseTransformPoint(posicionMundo);
        local += (Vector3)desplazamiento;
        return espacioCoordenadas.TransformPoint(local);
    }

    public bool SeSuperponen(RectTransform primero, RectTransform segundo)
    {
        if (primero == null || segundo == null)
        {
            return false;
        }

        Rect a = ObtenerLimites(primero);
        Rect b = ObtenerLimites(segundo);
        return a.Overlaps(b);
    }

    // El marco aparece y el jugador salta dentro.
    public IEnumerator Entrar()
    {
        if (!ReferenciasValidas())
        {
            yield break;
        }

        marcoVisual.SetActive(true);
        yield return jugador.SaltarA(
            centroMarco.position,
            alturaSalto,
            duracionSalto,
            this
        );
        jugador.ActivarControl(this);
    }

    // El jugador regresa a su sitio y el marco desaparece.
    public IEnumerator Salir()
    {
        if (!ReferenciasValidas())
        {
            yield break;
        }

        jugador.DesactivarControl();
        LimpiarProyectiles();
        yield return jugador.SaltarA(
            jugador.PosicionReposo,
            alturaSalto,
            duracionSalto,
            this
        );
        marcoVisual.SetActive(false);
    }

    // Para cuando el combate termina de golpe (por ejemplo, derrota).
    public void CerrarInmediato()
    {
        if (!ReferenciasValidas())
        {
            return;
        }

        jugador.DesactivarControl();
        LimpiarProyectiles();
        jugador.transform.position = jugador.PosicionReposo;
        marcoVisual.SetActive(false);
    }

    public IEnumerator EjecutarPatron(PatronAtaqueDatos patron, Combatiente combatienteJugador)
    {
        if (patron == null || combatienteJugador == null || !ReferenciasValidas())
        {
            yield break;
        }

        if (patron.proyectiles == null)
        {
            yield break;
        }

        List<EventoProyectil> eventos = new List<EventoProyectil>(patron.proyectiles);
        eventos.Sort((a, b) => a.tiempo.CompareTo(b.tiempo));

        float tiempo = 0f;
        int siguiente = 0;

        while (tiempo < patron.duracion && !combatienteJugador.EstaDerrotado)
        {
            tiempo += Time.deltaTime;

            while (siguiente < eventos.Count && eventos[siguiente].tiempo <= tiempo)
            {
                Lanzar(eventos[siguiente]);
                siguiente++;
            }

            yield return null;
        }
    }

    private void Lanzar(EventoProyectil evento)
    {
        if (evento == null || evento.prefab == null)
        {
            return;
        }

        Proyectil proyectil = Instantiate(evento.prefab, contenedorProyectiles, false);
        RectTransform rectProyectil = proyectil.GetComponent<RectTransform>();
        rectProyectil.position = PosicionSpawn(evento.lado, evento.posicion);
        proyectil.transform.SetAsLastSibling();

        proyectil.Iniciar(
            DireccionHaciaDentro(evento.lado),
            evento.velocidad,
            evento.danio,
            jugador,
            espacioCoordenadas,
            this
        );
    }

    private Vector3 PosicionSpawn(LadoSpawn lado, float posicion)
    {
        Rect limites = Limites;
        Vector2 escala = EscalaCanvas;
        float k = Mathf.Clamp01(posicion);
        float x = Mathf.Lerp(limites.xMin, limites.xMax, k);
        float y = Mathf.Lerp(limites.yMin, limites.yMax, k);

        switch (lado)
        {
            case LadoSpawn.Izquierda:
                x = limites.xMin - margenSpawn * escala.x;
                break;
            case LadoSpawn.Derecha:
                x = limites.xMax + margenSpawn * escala.x;
                break;
            case LadoSpawn.Arriba:
                y = limites.yMax + margenSpawn * escala.y;
                break;
            default:
                y = limites.yMin - margenSpawn * escala.y;
                break;
        }

        Vector3 local = new Vector3(x, y, 0f);
        return espacioCoordenadas.TransformPoint(local);
    }

    private Vector3 Limitar(Vector3 posicionMundo, Vector2 medioObjeto)
    {
        Rect limites = Limites;
        Vector3 local = espacioCoordenadas != null
            ? espacioCoordenadas.InverseTransformPoint(posicionMundo)
            : posicionMundo;

        local.x = Mathf.Clamp(
            local.x,
            limites.xMin + medioObjeto.x,
            limites.xMax - medioObjeto.x
        );
        local.y = Mathf.Clamp(
            local.y,
            limites.yMin + medioObjeto.y,
            limites.yMax - medioObjeto.y
        );

        return espacioCoordenadas != null
            ? espacioCoordenadas.TransformPoint(local)
            : local;
    }

    private Rect ObtenerLimites(RectTransform rectTransform)
    {
        Vector3[] esquinas = new Vector3[4];
        rectTransform.GetWorldCorners(esquinas);
        Vector3 primera = ConvertirAEspacio(esquinas[0]);
        float xMin = primera.x;
        float xMax = primera.x;
        float yMin = primera.y;
        float yMax = primera.y;

        for (int i = 1; i < esquinas.Length; i++)
        {
            Vector3 esquina = ConvertirAEspacio(esquinas[i]);
            xMin = Mathf.Min(xMin, esquina.x);
            xMax = Mathf.Max(xMax, esquina.x);
            yMin = Mathf.Min(yMin, esquina.y);
            yMax = Mathf.Max(yMax, esquina.y);
        }

        return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
    }

    private Vector2 ObtenerTamanoEnEspacio(RectTransform rectTransform)
    {
        Rect limites = ObtenerLimites(rectTransform);
        return limites.size;
    }

    private Vector3 ConvertirAEspacio(Vector3 posicionMundo)
    {
        return espacioCoordenadas != null
            ? espacioCoordenadas.InverseTransformPoint(posicionMundo)
            : posicionMundo;
    }

    private void ResolverEspacioCoordenadas()
    {
        if (espacioCoordenadas != null)
        {
            return;
        }

        Canvas canvas = centroMarco != null
            ? centroMarco.GetComponentInParent<Canvas>()
            : GetComponentInParent<Canvas>();

        if (canvas != null)
        {
            espacioCoordenadas = canvas.rootCanvas.transform as RectTransform;
        }
        else if (centroMarco != null)
        {
            espacioCoordenadas = centroMarco.parent as RectTransform;
        }
    }

    private bool ReferenciasValidas()
    {
        ResolverEspacioCoordenadas();

        if (marcoVisual != null && centroMarco != null && jugador != null &&
            contenedorProyectiles != null && espacioCoordenadas != null)
        {
            Canvas canvasBase = espacioCoordenadas.GetComponentInParent<Canvas>();
            if (canvasBase != null &&
                MismoCanvas(centroMarco, canvasBase) &&
                MismoCanvas(jugador.transform, canvasBase) &&
                MismoCanvas(contenedorProyectiles, canvasBase))
            {
                return true;
            }
        }

        Debug.LogError(
            "ArenaEsquive necesita marcoVisual, centroMarco, jugador, " +
            "contenedorProyectiles y un Canvas común.",
            this
        );
        return false;
    }

    private static bool MismoCanvas(Component componente, Canvas canvasBase)
    {
        Canvas canvas = componente.GetComponentInParent<Canvas>();
        return canvas != null && canvas.rootCanvas == canvasBase.rootCanvas;
    }

    private static Vector2 DireccionHaciaDentro(LadoSpawn lado)
    {
        switch (lado)
        {
            case LadoSpawn.Izquierda:
                return Vector2.right;
            case LadoSpawn.Derecha:
                return Vector2.left;
            case LadoSpawn.Arriba:
                return Vector2.down;
            default:
                return Vector2.up;
        }
    }

    private void LimpiarProyectiles()
    {
        if (contenedorProyectiles == null)
        {
            return;
        }

        for (int i = contenedorProyectiles.childCount - 1; i >= 0; i--)
        {
            Destroy(contenedorProyectiles.GetChild(i).gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (centroMarco == null)
        {
            return;
        }

        Gizmos.color = Color.cyan;
        if (centroMarco is RectTransform rectTransform)
        {
            Vector3[] esquinas = new Vector3[4];
            rectTransform.GetWorldCorners(esquinas);
            for (int i = 0; i < esquinas.Length; i++)
            {
                Gizmos.DrawLine(esquinas[i], esquinas[(i + 1) % esquinas.Length]);
            }
        }
        else
        {
            Gizmos.DrawWireCube(centroMarco.position, tamano);
        }
    }
}
