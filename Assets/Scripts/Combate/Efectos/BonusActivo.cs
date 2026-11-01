using UnityEngine;

[System.Serializable]
public class BonusActivo
{
    [SerializeField] private DatosBonus bonus;
    [SerializeField] private int turnosRestantes;

    public DatosBonus Bonus => bonus;
    public int TurnosRestantes => turnosRestantes;
    public bool HaTerminado => turnosRestantes <= 0;

    public BonusActivo(DatosBonus nuevoBonus,int duracionTurnos)
    {
        bonus = nuevoBonus;
        turnosRestantes = Mathf.Max(1, duracionTurnos);
    }

    public void AvanzarTurno()
    {
        if (turnosRestantes > 0)
        {
            turnosRestantes--;
        }
    }
}