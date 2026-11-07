using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelCombateUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject raiz;

    [Header("Enemigo")]
    [SerializeField] private Image imagenEnemigo;
    [SerializeField] private TMP_Text textoNombreEnemigo;

    [Header("Diálogo")]
    [SerializeField] private TMP_Text textoDialogo;
    [SerializeField, Min(1f)] private float caracteresPorSegundo = 45f;
    [SerializeField, Min(0f)] private float pausaTrasTexto = 0.8f;
    [SerializeField] private ControlesCombate controles;

    public void Mostrar(DatosEnemigo datos)
    {
        raiz.SetActive(true);

        if (imagenEnemigo != null)
        {
            imagenEnemigo.sprite = datos.sprite;
        }

        if (textoNombreEnemigo != null)
        {
            textoNombreEnemigo.text = datos.nombre;
        }

        textoDialogo.text = string.Empty;
    }

    public void Ocultar()
    {
        raiz.SetActive(false);
    }

    // Escribe el texto letra por letra. Confirmar lo muestra completo.
    public IEnumerator MostrarTexto(string texto, bool esperarConfirmacion = false)
    {
        if (string.IsNullOrEmpty(texto))
        {
            textoDialogo.text = string.Empty;
            yield break;
        }

        textoDialogo.text = texto;
        textoDialogo.maxVisibleCharacters = 0;
        textoDialogo.ForceMeshUpdate();

        int total = textoDialogo.textInfo.characterCount;
        float acumulado = 0f;

        while (textoDialogo.maxVisibleCharacters < total)
        {
            acumulado += caracteresPorSegundo * Time.deltaTime;
            textoDialogo.maxVisibleCharacters = Mathf.Min(total, (int)acumulado);

            if (controles.ConfirmarPresionado)
            {
                textoDialogo.maxVisibleCharacters = total;
            }

            yield return null;
        }

        textoDialogo.maxVisibleCharacters = total;

        if (esperarConfirmacion)
        {
            yield return null;

            while (!controles.ConfirmarPresionado)
            {
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(pausaTrasTexto);
        }
    }
}
