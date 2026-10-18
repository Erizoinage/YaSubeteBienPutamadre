using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SistemaDialogo : MonoBehaviour
{
    [Header("Panel principal")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Textos")]
    [SerializeField] private TMP_Text textoNombre;
    [SerializeField] private TMP_Text textoDialogo;

    [Header("Retrato")]
    [SerializeField] private GameObject contenedorRetrato;
    [SerializeField] private RectTransform movimientoRetrato;
    [SerializeField] private Image imagenRetrato;
    [SerializeField] private Animator animatorRetrato;

    [Header("Escritura")]
    [Tooltip("Segundos entre cada carácter.")]
    [SerializeField, Min(0.001f)]private float velocidadTexto = 0.035f;
    private bool avanzarConClic = true;
    [Header("Movimiento al hablar")]
    [SerializeField] private bool usarSaltoAlHablar = true;

    [Tooltip("Altura del salto en píxeles.")]
    [SerializeField, Min(0f)]
    private float alturaSalto = 10f;

    [Tooltip("Duración total del salto.")]
    [SerializeField, Min(0.05f)]
    private float duracionSalto = 0.18f;

    private string[] lineasActuales;
    private int indiceLinea;
    private Coroutine rutinaEscritura;
    private Coroutine rutinaSalto;
    private bool dialogoActivo;
    private bool escribiendoTexto;
    private string lineaCompleta;
    private Vector2 posicionOriginalRetrato;
    private bool posicionRetratoGuardada;

    public bool DialogoActivo => dialogoActivo;
    public bool EscribiendoTexto => escribiendoTexto;

    private void Start()
    {
        OcultarDialogo();
    }

    private void Update()
    {
        if (!dialogoActivo || !avanzarConClic)
            return;

        if (Mouse.current != null &&Mouse.current.leftButton.wasPressedThisFrame)
        {
            AvanzarDialogo();
        }
    }

    public void IniciarDialogo(string nombre, string[] lineas, Sprite retrato)
    {
        if (lineas == null || lineas.Length == 0)
        {
            Debug.LogWarning(
                "El diálogo de " + nombre +
                " no contiene líneas."
            );

            return;
        }

        DetenerEscritura();

        dialogoActivo = true;
        lineasActuales = lineas;
        indiceLinea = 0;

        MostrarPanel();
        ActualizarNombre(nombre);
        ActualizarRetrato(retrato);
        MostrarLineaActual();
    }

    public void AvanzarDialogo()
    {
        if (!dialogoActivo)
            return;

        if (escribiendoTexto)
        {
            CompletarLineaInmediatamente();
            return;
        }

        indiceLinea++;
        if (lineasActuales == null || indiceLinea >= lineasActuales.Length)
        {
            OcultarDialogo();
            return;
        }

        MostrarLineaActual();
    }

    private void MostrarLineaActual()
    {
        if (lineasActuales == null ||indiceLinea < 0 ||indiceLinea >= lineasActuales.Length)
        {
            OcultarDialogo();
            return;
        }

        lineaCompleta = lineasActuales[indiceLinea];

        DetenerEscritura();
        ReproducirMovimientoAlHablar();

        rutinaEscritura = StartCoroutine(
            EscribirTextoGradualmente(lineaCompleta)
        );
    }

    public void ReproducirMovimientoAlHablar()
    {
        if (!usarSaltoAlHablar || movimientoRetrato == null)
            return;

        if (!posicionRetratoGuardada)
        {
            posicionOriginalRetrato =movimientoRetrato.anchoredPosition;

            posicionRetratoGuardada = true;
        }

        if (rutinaSalto != null)
        {
            StopCoroutine(rutinaSalto);
            movimientoRetrato.anchoredPosition =posicionOriginalRetrato;
        }
        rutinaSalto = StartCoroutine(SaltarRetrato());
    }

    private IEnumerator SaltarRetrato()
    {
        Vector2 posicionInicial = posicionOriginalRetrato;
        Vector2 posicionElevada =
            posicionInicial + Vector2.up * alturaSalto;

        float mitadDuracion = duracionSalto * 0.5f;
        float tiempo = 0f;

        while (tiempo < mitadDuracion)
        {
            tiempo += Time.unscaledDeltaTime;

            float progreso = Mathf.Clamp01(tiempo / mitadDuracion);

            float suavizado = Mathf.SmoothStep( 0f, 1f, progreso);

            movimientoRetrato.anchoredPosition =Vector2.Lerp( posicionInicial, posicionElevada,suavizado);
            yield return null;
        }

        tiempo = 0f;
        while (tiempo < mitadDuracion)
        {
            tiempo += Time.unscaledDeltaTime;

            float progreso = Mathf.Clamp01( tiempo / mitadDuracion);

            float suavizado = Mathf.SmoothStep(  0f, 1f, progreso);

            movimientoRetrato.anchoredPosition = Vector2.Lerp(  posicionElevada, posicionInicial, suavizado);
            yield return null;
        }

        movimientoRetrato.anchoredPosition =posicionInicial;

        rutinaSalto = null;
    }

    private void DetenerMovimientoRetrato()
    {
        if (rutinaSalto != null)
        {
            StopCoroutine(rutinaSalto);
            rutinaSalto = null;
        }

        if (movimientoRetrato != null &&posicionRetratoGuardada)
        {
            movimientoRetrato.anchoredPosition =posicionOriginalRetrato;
        }
    }

    private IEnumerator EscribirTextoGradualmente(string texto)
    {
        escribiendoTexto = true;
        textoDialogo.text = string.Empty;

        foreach (char caracter in texto)
        {
            textoDialogo.text += caracter;

            yield return new WaitForSecondsRealtime(velocidadTexto);
        }

        escribiendoTexto = false;
        rutinaEscritura = null;
    }

    private void CompletarLineaInmediatamente()
    {
        DetenerEscritura();

        if (textoDialogo != null)
            textoDialogo.text = lineaCompleta;

        escribiendoTexto = false;
    }

    private void DetenerEscritura()
    {
        if (rutinaEscritura != null)
        {
            StopCoroutine(rutinaEscritura);
            rutinaEscritura = null;
        }

        escribiendoTexto = false;
    }

    private void MostrarPanel()
    {
        if (canvasGroup == null)
        {
            Debug.LogWarning("SistemaDialogo no tiene Canvas Group asignado.");
            return;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void ActualizarNombre(string nombre)
    {
        if (textoNombre != null)
            textoNombre.text = nombre;
    }

    private void ActualizarRetrato(Sprite retrato)
    {
        bool tieneRetrato = retrato != null;

        if (contenedorRetrato != null)
            contenedorRetrato.SetActive(tieneRetrato);

        if (imagenRetrato != null)
        {
            imagenRetrato.sprite = retrato;
            imagenRetrato.enabled = tieneRetrato;
            imagenRetrato.preserveAspect = true;
        }

        if (animatorRetrato != null)
        {
            animatorRetrato.enabled =tieneRetrato &&animatorRetrato.runtimeAnimatorController != null;
        }
    }

    public void OcultarDialogo()
{
    DetenerEscritura();
    DetenerMovimientoRetrato();

    dialogoActivo = false;
    lineasActuales = null;
    lineaCompleta = string.Empty;
    indiceLinea = 0;

    if (canvasGroup != null)
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    if (textoNombre != null)
        textoNombre.text = string.Empty;

    if (textoDialogo != null)
        textoDialogo.text = string.Empty;

    if (imagenRetrato != null)
    {
        imagenRetrato.sprite = null;
        imagenRetrato.enabled = false;
    }

    if (animatorRetrato != null)
        animatorRetrato.enabled = false;

    if (contenedorRetrato != null)
        contenedorRetrato.SetActive(false);
}
}