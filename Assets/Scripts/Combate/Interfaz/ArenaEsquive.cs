using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controla el marco donde se esquivan los ataques del enemigo.
public class ArenaEsquive : MonoBehaviour
{
    [Header("Marco")]
    [SerializeField] private GameObject marcoVisual;
    [SerializeField] private Transform centroMarco;
    [SerializeField] private Vector2 tamano = new Vector2(6f, 3f);

    [Header("Jugador")]
    [SerializeField] private JugadorEsquive jugador;
    [SerializeField, Min(0f)] private float alturaSalto = 1.2f;
    [SerializeField, Min(0.05f)] private float duracionSalto = 0.5f;

    [Header("Proyectiles")]
    [SerializeField] private Transform contenedorProyectiles;
    [SerializeField, Min(0f)] private float margenSpawn = 1f;

    public Rect Limites => new Rect(
        (Vector2)centroMarco.position - tamano * 0.5f,
        tamano
    );

    public Vector3 Limitar(Vector3 posicion)
    {
        Rect limites = Limites;
        posicion.x = Mathf.Clamp(posicion.x, limites.xMin, limites.xMax);
        posicion.y = Mathf.Clamp(posicion.y, limites.yMin, limites.yMax);
        return posicion;
    }

    // El marco aparece y el jugador salta dentro.
    public IEnumerator Entrar()
    {
        marcoVisual.SetActive(true);
        yield return StartCoroutine(
            jugador.SaltarA(centroMarco.position, alturaSalto, duracionSalto)
        );
        jugador.ActivarControl(this);
    }

    // El jugador regresa a su sitio y el marco desaparece.
    public IEnumerator Salir()
    {
        jugador.DesactivarControl();
        LimpiarProyectiles();
        yield return StartCoroutine(
            jugador.SaltarA(jugador.PosicionReposo, alturaSalto, duracionSalto)
        );
        marcoVisual.SetActive(false);
    }

    // Para cuando el combate termina de golpe (por ejemplo, derrota).
    public void CerrarInmediato()
    {
        jugador.DesactivarControl();
        LimpiarProyectiles();
        jugador.transform.position = jugador.PosicionReposo;
        marcoVisual.SetActive(false);
    }

    public IEnumerator EjecutarPatron(PatronAtaqueDatos patron, Combatiente combatienteJugador)
    {
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
        if (evento.prefab == null)
        {
            return;
        }

        Proyectil proyectil = Instantiate(
            evento.prefab,
            PosicionSpawn(evento.lado, evento.posicion),
            Quaternion.identity,
            contenedorProyectiles
        );

        proyectil.Iniciar(DireccionHaciaDentro(evento.lado), evento.velocidad, evento.danio);
    }

    private Vector3 PosicionSpawn(LadoSpawn lado, float k)
    {
        Rect limites = Limites;
        float x = Mathf.Lerp(limites.xMin, limites.xMax, k);
        float y = Mathf.Lerp(limites.yMin, limites.yMax, k);
        float z = centroMarco.position.z;

        switch (lado)
        {
            case LadoSpawn.Izquierda:
                return new Vector3(limites.xMin - margenSpawn, y, z);
            case LadoSpawn.Derecha:
                return new Vector3(limites.xMax + margenSpawn, y, z);
            case LadoSpawn.Arriba:
                return new Vector3(x, limites.yMax + margenSpawn, z);
            default:
                return new Vector3(x, limites.yMin - margenSpawn, z);
        }
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
        Gizmos.DrawWireCube(centroMarco.position, tamano);
    }
}
