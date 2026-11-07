using System;
using UnityEngine;

// Frases propias de cada enemigo (se guardan en su DatosEnemigo).
// Cada lista puede tener varias frases: se elige una al azar.
// Puedes escribir {enemigo} para insertar el nombre del enemigo.
[Serializable]
public class FrasesEnemigo
{
    [TextArea] public string[] intro;
    [TextArea] public string[] turnoJugador;
    [TextArea] public string[] prepararTurnoRival;
    [TextArea] public string[] prepararTurnoJugador;
    [TextArea] public string[] victoria;
    [TextArea] public string[] derrota;
    [TextArea] public string[] huida;

    public static string Elegir(string[] frases)
    {
        if (frases == null || frases.Length == 0)
        {
            return string.Empty;
        }

        return frases[UnityEngine.Random.Range(0, frases.Length)];
    }
}

// Frases generales del sistema (se editan en el GestorCombate).
[Serializable]
public class TextosSistema
{
    [Tooltip("Puedes usar {objeto} y {danio}.")]
    public string usarAtaque = "Usaste {objeto} e hiciste {danio} de daño.";

    [Tooltip("Puedes usar {objeto}.")]
    public string usarConsumible = "Usaste {objeto}.";
}
