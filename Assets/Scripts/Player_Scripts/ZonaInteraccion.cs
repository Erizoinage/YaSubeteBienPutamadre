using System.Collections.Generic;
using UnityEngine;

public class ZonaInteraccion : MonoBehaviour
{
    private readonly List<ITocable> objetosDetectados =
        new List<ITocable>();

    private void OnTriggerEnter2D(Collider2D otro)
    {
        ITocable interactuable = otro.GetComponentInParent<ITocable>();

        if (interactuable == null)
            return;

        if (objetosDetectados.Contains(interactuable))
            return;

        objetosDetectados.Add(interactuable);

        Debug.Log(
            "Entró en la zona: " +
            interactuable.NombreObjeto
        );
    }

    private void OnTriggerExit2D(Collider2D otro)
    {
        ITocable interactuable = otro.GetComponentInParent<ITocable>();

        if (interactuable == null)
            return;

        objetosDetectados.Remove(interactuable);

        Debug.Log(
            "Salió de la zona: " +
            interactuable.NombreObjeto
        );
    }

    public ITocable ObtenerObjetoMasCercano()
    {
        LimpiarLista();

        ITocable objetoMasCercano = null;
        float distanciaMasCorta = Mathf.Infinity;

        foreach (ITocable objeto in objetosDetectados)
        {
            if (objeto == null)
                continue;

            float distancia = Vector2.Distance(
                transform.position,
                objeto.transform.position
            );

            if (distancia < distanciaMasCorta)
            {
                distanciaMasCorta = distancia;
                objetoMasCercano = objeto;
            }
        }

        return objetoMasCercano;
    }

    private void LimpiarLista()
    {
        objetosDetectados.RemoveAll(
            objeto => objeto == null
        );
    }

    private void OnDisable()
    {
        objetosDetectados.Clear();
    }
}