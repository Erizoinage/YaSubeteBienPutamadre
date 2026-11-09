using System.Collections.Generic;
using UnityEngine;

// Lleva la cuenta de los efectos de los consumibles que duran varios turnos.
public class EfectosTemporalesCombate : MonoBehaviour
{
    private class Efecto
    {
        public int curacionPorTurno;
        public int curacionRestante;
        public int turnosCuracion;
        public float bonusDanio;
        public int turnosBonus;
        public bool bonusIniciado;
    }

    private readonly List<Efecto> efectos = new List<Efecto>();

    public float MultiplicadorDanio
    {
        get
        {
            float total = 1f;

            foreach (Efecto efecto in efectos)
            {
                if (efecto.turnosBonus > 0)
                {
                    total += efecto.bonusDanio;
                }
            }

            return total;
        }
    }

    public void Agregar(IConsumibleCombate consumible)
    {
        if (consumible == null ||
            (consumible.TurnosCuracion <= 0 && consumible.TurnosBonus <= 0))
        {
            return;
        }

        efectos.Add(new Efecto
        {
            curacionPorTurno = consumible.CuracionPorTurno,
            curacionRestante = consumible.CuracionGradualTotal,
            turnosCuracion = consumible.TurnosCuracion,
            bonusDanio = consumible.BonusDanio,
            turnosBonus = consumible.TurnosBonus
        });
    }

    // Se llama al inicio de cada turno del jugador.
    public void ProcesarInicioDeTurno(Combatiente objetivo)
    {
        for (int i = efectos.Count - 1; i >= 0; i--)
        {
            Efecto efecto = efectos[i];

            if (efecto.turnosCuracion <= 0 && efecto.turnosBonus <= 0)
            {
                efectos.RemoveAt(i);
                continue;
            }

            if (efecto.turnosCuracion > 0 && efecto.curacionRestante > 0)
            {
                int curacion = Mathf.Min(
                    efecto.curacionPorTurno,
                    efecto.curacionRestante
                );
                objetivo.Curar(curacion);
                efecto.curacionRestante -= curacion;
                efecto.turnosCuracion--;
            }

            if (efecto.turnosBonus > 0)
            {
                if (efecto.bonusIniciado)
                {
                    efecto.turnosBonus--;
                }
                else
                {
                    efecto.bonusIniciado = true;
                }
            }
        }
    }

    public void Limpiar()
    {
        efectos.Clear();
    }
}
