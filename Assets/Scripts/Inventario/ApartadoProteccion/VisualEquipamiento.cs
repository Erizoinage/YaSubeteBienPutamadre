using UnityEngine;

public class VisualEquipamiento : MonoBehaviour
{
    private SpriteRenderer visualCasco;
   private SpriteRenderer visualArmadura;

    [Header("Puntos de equipamiento")]
    [SerializeField] private Transform puntoCasco;
    [SerializeField] private Transform puntoArmadura;

    private DatosEquipable cascoEquipado;
    private DatosEquipable armaduraEquipada;

    private DireccionJugador direccionActual = DireccionJugador.Derecha;

    private void Awake()
    {
        SpriteRenderer rendererJugador = GetComponent<SpriteRenderer>();

        if (visualCasco == null)
        {
            visualCasco = CrearRenderer(
                "VisualCasco",
                ObtenerPuntoValido(puntoCasco, "casco"),
                rendererJugador
            );
        }
        else if (puntoCasco != null)
        {
            visualCasco.transform.SetParent(
                ObtenerPuntoValido(puntoCasco, "casco"),
                false
            );
        }

        if (visualArmadura == null)
        {
            visualArmadura = CrearRenderer(
                "VisualArmadura",
                ObtenerPuntoValido(puntoArmadura, "armadura"),
                rendererJugador
            );
        }
        else if (puntoArmadura != null)
        {
            visualArmadura.transform.SetParent(
                ObtenerPuntoValido(puntoArmadura, "armadura"),
                false
            );
        }
    }

    private void Start()
    {
        ActualizarVisuales();
    }

    public void EstablecerEquipamiento(DatosEquipable nuevoCasco,DatosEquipable nuevaArmadura)
    {
        if (nuevoCasco != null && cascoEquipado != nuevoCasco)
        {
            direccionActual = DireccionJugador.Derecha;
        }

        cascoEquipado = nuevoCasco;
        armaduraEquipada = nuevaArmadura;

        if (cascoEquipado != null && !TieneSpritesConfigurados(cascoEquipado))
        {
            Debug.LogWarning(cascoEquipado.nombreObjeto + " no tiene sprites direccionales asignados.", this);
        }

        if (armaduraEquipada != null && !TieneSpritesConfigurados(armaduraEquipada))
        {
            Debug.LogWarning(armaduraEquipada.nombreObjeto + " no tiene sprites direccionales asignados.", this);
        }

        ActualizarVisuales();
    }

    public void Equipar(DatosEquipable nuevoEquipamiento)
    {
        if (nuevoEquipamiento == null)
        {
            return;
        }

        switch (nuevoEquipamiento.tipoEquipamiento)
        {
            case TipoEquipamiento.Casco:
                if (cascoEquipado != nuevoEquipamiento)
                {
                    direccionActual = DireccionJugador.Derecha;
                }

                cascoEquipado = nuevoEquipamiento;
                break;

            case TipoEquipamiento.Pechera:
                armaduraEquipada = nuevoEquipamiento;
                break;

            default:
                Debug.LogWarning(nuevoEquipamiento.nombreObjeto +" todavía no tiene una capa visual configurada.");

                return;
        }

        ActualizarVisuales();
    }

    public void Desequipar(TipoEquipamiento tipo)
    {
        switch (tipo)
        {
            case TipoEquipamiento.Casco:
                cascoEquipado = null;
                break;

            case TipoEquipamiento.Pechera:
                armaduraEquipada = null;
                break;
        }

        ActualizarVisuales();
    }

    public void CambiarDireccion(DireccionJugador nuevaDireccion)
    {
        if (direccionActual == nuevaDireccion)
        {
            return;
        }

        direccionActual = nuevaDireccion;
        ActualizarVisuales();
    }

    private void ActualizarVisuales()
    {
        ActualizarOrdenCapas();

        ActualizarRenderer(visualCasco,cascoEquipado);

        ActualizarRenderer(visualArmadura,armaduraEquipada);
    }

    private SpriteRenderer CrearRenderer(string nombre,Transform puntoVisual,SpriteRenderer rendererJugador)
    {
        if (rendererJugador == null)
        {
            Debug.LogWarning("El jugador necesita un SpriteRenderer para mostrar el equipamiento.", this);
            return null;
        }

        GameObject capaVisual = new GameObject(nombre);
        capaVisual.transform.SetParent(
            puntoVisual != null ? puntoVisual : transform,
            false
        );

        SpriteRenderer rendererVisual = capaVisual.AddComponent<SpriteRenderer>();
        rendererVisual.sortingLayerID = rendererJugador.sortingLayerID;
        rendererVisual.sharedMaterial = rendererJugador.sharedMaterial;
        rendererVisual.enabled = false;

        return rendererVisual;
    }

    private Transform ObtenerPuntoValido(Transform punto, string nombrePunto)
    {
        if (punto == null)
        {
            return transform;
        }

        if (punto == transform || punto.IsChildOf(transform))
        {
            return punto;
        }

        Debug.LogWarning("El punto de " + nombrePunto +" debe ser hijo del objeto que tiene VisualEquipamiento.", this);

        return transform;
    }

    private void ActualizarOrdenCapas()
    {
        SpriteRenderer rendererJugador = GetComponent<SpriteRenderer>();
        if (rendererJugador == null)
        {
            return;
        }

        bool mirandoArriba = direccionActual == DireccionJugador.Arriba;
        int ordenBase = rendererJugador.sortingOrder;

        if (visualArmadura != null)
        {
            visualArmadura.sortingLayerID = rendererJugador.sortingLayerID;
            bool usandoSpriteEspaldaArmadura =
                mirandoArriba &&
                armaduraEquipada != null &&
                armaduraEquipada.spriteEspalda != null;

            visualArmadura.sortingOrder = ordenBase +
                (usandoSpriteEspaldaArmadura ? 1 : mirandoArriba ? -2 : 1);
        }

        if (visualCasco != null)
        {
            visualCasco.sortingLayerID = rendererJugador.sortingLayerID;
            bool usandoSpriteEspalda =
                mirandoArriba &&
                cascoEquipado != null &&
                cascoEquipado.spriteEspalda != null;

            visualCasco.sortingOrder = ordenBase +
                (usandoSpriteEspalda ? 1 : mirandoArriba ? -1 : 2);
        }
    }

    private bool TieneSpritesConfigurados(DatosEquipable equipamiento)
    {
        return equipamiento.spriteFrente != null ||
               equipamiento.spriteEspalda != null ||
               equipamiento.spriteLaterales != null;
    }

    private void ActualizarRenderer(SpriteRenderer rendererVisual, DatosEquipable equipamiento)
    {
        if (rendererVisual == null)
        {
            return;
        }

        if (equipamiento == null)
        {
            rendererVisual.sprite = null;
            rendererVisual.enabled = false;
            return;
        }

        Sprite spriteDireccion =equipamiento.ObtenerSprite(direccionActual);

        rendererVisual.sprite = spriteDireccion;
        rendererVisual.enabled = spriteDireccion != null;
    }
}