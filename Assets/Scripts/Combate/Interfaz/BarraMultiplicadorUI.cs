using System.Collections;
using UnityEngine;

// El marcador va de lado a lado; al confirmar, cuanto más cerca del centro
// esté, mayor es el multiplicador (editable con la curva).
// El marcador debe ser hijo de la barra, con el ancla en el centro.
public class BarraMultiplicadorUI : MonoBehaviour
{
    [SerializeField] private GameObject raiz;
    [SerializeField] private RectTransform barra;
    [SerializeField] private RectTransform marcador;
    [SerializeField] private ControlesCombate controles;

    [Header("Ajustes")]
    [SerializeField, Min(0.1f)] private float velocidad = 1.5f;
    [SerializeField, Min(0f)] private float tiempoGracia = 0.25f;
    [SerializeField, Min(0f)] private float pausaResultado = 0.5f;

    [Tooltip("X: cercanía al centro (0 = borde, 1 = centro). Y: multiplicador de daño.")]
    [SerializeField] private AnimationCurve curvaMultiplicador = AnimationCurve.Linear(0f, 0.5f, 1f, 1.5f);

    public float UltimoMultiplicador { get; private set; } = 1f;
    public bool UltimaEjecucionConfirmada { get; private set; }

    public bool PuedeEjecutarse =>
        raiz != null && barra != null && marcador != null && controles != null &&
        marcador.parent == barra;

    public IEnumerator Ejecutar()
    {
        UltimaEjecucionConfirmada = false;
        if (!PuedeEjecutarse)
        {
            Debug.LogError(
                "BarraMultiplicadorUI necesita raiz, barra, marcador hijo directo y controles.",
                this
            );
            yield break;
        }

        marcador.anchorMin = new Vector2(0.5f, marcador.anchorMin.y);
        marcador.anchorMax = new Vector2(0.5f, marcador.anchorMax.y);
        marcador.pivot = new Vector2(0.5f, marcador.pivot.y);

        raiz.SetActive(true);
        Canvas.ForceUpdateCanvases();
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(barra);
        yield return null;

        if (barra.rect.width <= 0f)
        {
            Debug.LogError("La barra multiplicadora no tiene ancho visible.", this);
            Ocultar();
            yield break;
        }

        UltimoMultiplicador = 1f;

        float tiempo = 0f;
        bool detenido = false;

        while (!detenido)
        {
            tiempo += Time.deltaTime;

            float posicion = Mathf.PingPong(tiempo * velocidad, 1f);
            MoverMarcador(posicion);

            // El tiempo de gracia evita contar la misma pulsación que eligió el ataque.
            if (tiempo >= tiempoGracia && controles.ConfirmarPresionado)
            {
                detenido = true;
                float cercania = 1f - Mathf.Abs(posicion - 0.5f) * 2f;
                UltimoMultiplicador = curvaMultiplicador.Evaluate(cercania);
                UltimaEjecucionConfirmada = true;
            }

            yield return null;
        }

        yield return new WaitForSeconds(pausaResultado);
        Ocultar();
    }

    public void Ocultar()
    {
        if (raiz != null)
        {
            raiz.SetActive(false);
        }
    }

    private void MoverMarcador(float posicion)
    {
        float recorrido = Mathf.Max(0f, barra.rect.width - marcador.rect.width);
        float x = (posicion - 0.5f) * recorrido;
        Vector2 posicionMarcador = marcador.anchoredPosition;
        posicionMarcador.x = x;
        marcador.anchoredPosition = posicionMarcador;
    }
}