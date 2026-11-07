using UnityEngine;

// Prefab del proyectil: SpriteRenderer + Collider2D (Is Trigger) + este script.
public class Proyectil : MonoBehaviour
{
    [SerializeField] private bool esParreable = true;
    [SerializeField, Min(0.5f)] private float tiempoDeVidaMaximo = 10f;

    private Vector2 direccion;
    private float velocidad;
    private int danio;

    public int Danio => danio;
    public bool EsParreable => esParreable;

    public void Iniciar(Vector2 nuevaDireccion, float nuevaVelocidad, int nuevoDanio)
    {
        direccion = nuevaDireccion.normalized;
        velocidad = nuevaVelocidad;
        danio = nuevoDanio;
        Destroy(gameObject, tiempoDeVidaMaximo);
    }

    private void Update()
    {
        transform.position += (Vector3)(direccion * velocidad * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out JugadorEsquive jugador))
        {
            jugador.ProcesarImpacto(this);
        }
    }

    public void Destruir()
    {
        Destroy(gameObject);
    }
}
