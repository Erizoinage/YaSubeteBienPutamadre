using UnityEngine;

public class EnemigoCombate : MonoBehaviour
{
    [SerializeField] private DatosEnemigo datos;
    [SerializeField] private bool desaparecerAlVencer = true;

    private GestorCombate gestor;
    private bool combateEnCurso;

    private void Awake()
    {
        gestor = FindAnyObjectByType<GestorCombate>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (combateEnCurso || GestorCombate.EnCombate || gestor == null)
        {
            return;
        }

        if (other.GetComponentInParent<MovimientoJugador>() == null)
        {
            return;
        }

        combateEnCurso = true;
        gestor.CombateTerminado += AlTerminarCombate;
        gestor.IniciarCombate(datos);
    }

    private void AlTerminarCombate(ResultadoCombate resultado)
    {
        gestor.CombateTerminado -= AlTerminarCombate;
        combateEnCurso = false;

        if (resultado == ResultadoCombate.Victoria && desaparecerAlVencer)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (gestor != null)
        {
            gestor.CombateTerminado -= AlTerminarCombate;
        }
    }
}