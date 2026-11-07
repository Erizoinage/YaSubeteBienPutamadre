using System;
using System.Collections.Generic;
using UnityEngine;

public enum LadoSpawn
{
    Izquierda,
    Derecha,
    Arriba,
    Abajo
}

[Serializable]
public class EventoProyectil
{
    [Min(0f)] public float tiempo;
    public Proyectil prefab;
    public LadoSpawn lado;

    [Tooltip("Posición a lo largo del lado: 0 = un extremo, 1 = el otro.")]
    [Range(0f, 1f)] public float posicion = 0.5f;

    [Min(0.1f)] public float velocidad = 4f;
    [Min(0)] public int danio = 5;
}

[CreateAssetMenu(fileName = "NuevoPatron", menuName = "Combate/Patrón de ataque")]
public class PatronAtaqueDatos : ScriptableObject
{
    [Min(1f)] public float duracion = 8f;
    public List<EventoProyectil> proyectiles = new List<EventoProyectil>();
}
