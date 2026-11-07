using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NuevoEnemigo", menuName = "Combate/Enemigo")]
public class DatosEnemigo : ScriptableObject
{
    [Header("Información")]
    public string nombre = "Enemigo";
    public Sprite sprite;

    [Header("Estadísticas")]
    [Min(1)] public int vidaMaxima = 50;

    [Header("Ataques")]
    public List<PatronAtaqueDatos> patrones = new List<PatronAtaqueDatos>();

    [Tooltip("Si está activo usa los patrones en orden; si no, al azar.")]
    public bool patronesEnOrden;

    [Header("Frases")]
    public FrasesEnemigo frases = new FrasesEnemigo();

    public PatronAtaqueDatos ElegirPatron(int numeroDeTurno)
    {
        if (patrones == null || patrones.Count == 0)
        {
            return null;
        }

        int indice = patronesEnOrden
            ? numeroDeTurno % patrones.Count
            : Random.Range(0, patrones.Count);

        return patrones[indice];
    }
}
