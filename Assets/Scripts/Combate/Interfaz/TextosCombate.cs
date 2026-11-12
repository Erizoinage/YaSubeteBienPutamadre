using System;
using UnityEngine;
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
[Serializable]
public class TextosSistema
{
    [Tooltip("Puedes usar {objeto} y {danio}.")]
    public string usarAtaque = "Usaste {objeto} e hiciste {danio} de daño.";

    [Tooltip("Puedes usar {objeto}.")]
    public string usarConsumible = "Usaste {objeto}.";
}
