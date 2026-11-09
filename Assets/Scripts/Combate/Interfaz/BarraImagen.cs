using UnityEngine;
using UnityEngine.UI;
public class BarraImagen : MonoBehaviour
{
    [SerializeField] private Image relleno;
    [SerializeField] private Image rellenoRetraso;
    [SerializeField, Min(0.01f)] private float velocidadRetraso = 0.5f;

    private float objetivo = 1f;

    private void Awake()
    {
        ConfigurarRelleno(relleno);
        ConfigurarRelleno(rellenoRetraso);
    }

    public void Establecer(float valor01, bool instantaneo = false)
    {
        objetivo = Mathf.Clamp01(valor01);

        if (relleno != null)
        {
            relleno.fillAmount = objetivo;
        }

        if (rellenoRetraso != null &&
            (instantaneo || rellenoRetraso.fillAmount < objetivo))
        {
            rellenoRetraso.fillAmount = objetivo;
        }
    }

    private void Update()
    {
        if (rellenoRetraso != null && rellenoRetraso.fillAmount > objetivo)
        {
            rellenoRetraso.fillAmount = Mathf.MoveTowards(
                rellenoRetraso.fillAmount,
                objetivo,
                velocidadRetraso * Time.deltaTime
            );
        }
    }

    private static void ConfigurarRelleno(Image imagen)
    {
        if (imagen == null)
        {
            return;
        }

        imagen.type = Image.Type.Filled;
        imagen.fillMethod = Image.FillMethod.Horizontal;
        imagen.fillOrigin = (int)Image.OriginHorizontal.Left;
    }
}