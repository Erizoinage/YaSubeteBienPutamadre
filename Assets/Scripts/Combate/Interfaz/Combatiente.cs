using System;
using UnityEngine;

// Vida y energía de quien participa en el combate (jugador o enemigo).
public class Combatiente : MonoBehaviour
{
    [SerializeField, Min(1)] private int vidaMaxima = 100;
    [SerializeField, Min(0)] private int energiaMaxima = 100;

    private bool inicializado;
    private int vida;
    private int energia;

    public int Vida { get { Inicializar(); return vida; } }
    public int Energia { get { Inicializar(); return energia; } }
    public int VidaMaxima => vidaMaxima;
    public int EnergiaMaxima => energiaMaxima;
    public bool EstaDerrotado => Vida <= 0;

    // (valor actual, valor máximo)
    public event Action<int, int> VidaCambiada;
    public event Action<int, int> EnergiaCambiada;

    private void Inicializar()
    {
        if (inicializado)
        {
            return;
        }

        inicializado = true;
        vida = vidaMaxima;
        energia = energiaMaxima;
    }

    public void Configurar(int nuevaVidaMaxima, int nuevaEnergiaMaxima)
    {
        inicializado = true;
        vidaMaxima = Mathf.Max(1, nuevaVidaMaxima);
        energiaMaxima = Mathf.Max(0, nuevaEnergiaMaxima);
        vida = vidaMaxima;
        energia = energiaMaxima;
        NotificarEstado();
    }

    public void RecibirDanio(int cantidad)
    {
        Inicializar();

        if (cantidad <= 0)
        {
            return;
        }

        vida = Mathf.Max(0, vida - cantidad);
        VidaCambiada?.Invoke(vida, vidaMaxima);
    }

    public void Curar(int cantidad)
    {
        Inicializar();

        if (cantidad <= 0)
        {
            return;
        }

        vida = Mathf.Min(vidaMaxima, vida + cantidad);
        VidaCambiada?.Invoke(vida, vidaMaxima);
    }

    public bool GastarEnergia(int cantidad)
    {
        Inicializar();

        if (cantidad < 0 || energia < cantidad)
        {
            return false;
        }

        energia -= cantidad;
        EnergiaCambiada?.Invoke(energia, energiaMaxima);
        return true;
    }

    public void RecuperarEnergia(int cantidad)
    {
        Inicializar();

        if (cantidad <= 0)
        {
            return;
        }

        energia = Mathf.Min(energiaMaxima, energia + cantidad);
        EnergiaCambiada?.Invoke(energia, energiaMaxima);
    }

    public void RestaurarEnergia()
    {
        Inicializar();
        energia = energiaMaxima;
        EnergiaCambiada?.Invoke(energia, energiaMaxima);
    }

    public void NotificarEstado()
    {
        VidaCambiada?.Invoke(Vida, vidaMaxima);
        EnergiaCambiada?.Invoke(Energia, energiaMaxima);
    }
}
