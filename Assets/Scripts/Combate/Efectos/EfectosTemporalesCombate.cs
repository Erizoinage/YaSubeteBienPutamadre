using System.Collections.Generic;
using UnityEngine;

// Lleva la cuenta de los efectos de los consumibles que duran varios turnos.
public class EfectosTemporalesCombate : MonoBehaviour
{
    private class Efecto
    {
        public int curacionPorTurno;
        public int turnosRestantes;
        public float bonusDanio;
    }

    private readonly List<Efecto> efectos = new List<Efecto>();

    public float MultiplicadorDanio
    {
        get
        {
            float total = 1f;

            foreach (Efecto efecto in efectos)
            {
                total += efecto.bonusDanio;
            }

            return total;
        }
    }

    public void Agregar(IConsumibleCombate consumible)
    {
        if (consumible == null || consumible.TurnosDeEfecto <= 0)
        {
            return;
        }

        efectos.Add(new Efecto
        {
            curacionPorTurno = consumible.CuracionPorTurno,
            turnosRestantes = consumible.TurnosDeEfecto,
            bonusDanio = consumible.BonusDanio
        });
    }

    // Se llama al inicio de cada turno del jugador.
    public void ProcesarInicioDeTurno(Combatiente objetivo)
    {
        for (int i = efectos.Count - 1; i >= 0; i--)
        {
            Efecto efecto = efectos[i];

            if (efecto.turnosRestantes <= 0)
            {
                efectos.RemoveAt(i);
                continue;
            }

            objetivo.Curar(efecto.curacionPorTurno);
            efecto.turnosRestantes--;
        }
    }

    public void Limpiar()
    {
        efectos.Clear();
    }
}
