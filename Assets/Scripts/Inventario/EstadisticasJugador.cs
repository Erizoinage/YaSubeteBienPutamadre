using System;
using UnityEngine;

public class EstadisticasJugador : MonoBehaviour
{
    [Header("Vida")]
    [Min(1)][SerializeField] private int vidaMaximaBase = 100;
    [SerializeField] private int vidaActual;

    [Header("Energia")]
    [Min(1)]
    [SerializeField] private int energiaMaximaBase = 100;
    [SerializeField] private int energiaActual;

    [Header("Estadisticas base")]
    [Min(0)]
    [SerializeField] private int ataqueBase = 5;
    [Min(0)]
    [SerializeField] private int defensaBase;
    [Min(0.1f)]
    [SerializeField] private float velocidadBase = 5f;

    [Header("Bonificaciones actuales")]
    [SerializeField] private int ataqueExtra;
    [SerializeField] private int defensaExtra;
    [SerializeField] private int vidaMaximaExtra;
    [SerializeField] private int energiaMaximaExtra;
    [SerializeField] private float velocidadExtra;

    [Header("Limites de movimiento")]
    [Min(0.1f)]
    [SerializeField] private float velocidadMinima = 1f;
    [Min(0.1f)]
    [SerializeField] private float velocidadMaxima = 12f;

    private bool estaDerrotado;
    public int VidaMaxima =>Mathf.Max(1, vidaMaximaBase + vidaMaximaExtra);
    public int EnergiaMaxima =>Mathf.Max(1, energiaMaximaBase + energiaMaximaExtra);
    public int AtaqueTotal =>Mathf.Max(0, ataqueBase + ataqueExtra);
    public int DefensaTotal =>Mathf.Max(0, defensaBase + defensaExtra);
    public float VelocidadTotal =>Mathf.Clamp(velocidadBase + velocidadExtra,velocidadMinima,velocidadMaxima);
    public int VidaActual => vidaActual;
    public int EnergiaActual => energiaActual;
    public bool EstaDerrotado => estaDerrotado;
    public int AtaqueBase => ataqueBase;
    public int DefensaBase => defensaBase;
    public float VelocidadBase => velocidadBase;
    public event Action<int, int> AlCambiarVida;
    public event Action<int, int> AlCambiarEnergia;
    public event Action AlCambiarEstadisticas;
    public event Action AlSerDerrotado;

    private void Awake()
    {
        InicializarEstadisticas();
    }

    private void InicializarEstadisticas()
    {
        vidaActual = VidaMaxima;
        energiaActual = EnergiaMaxima;
        estaDerrotado = false;

        NotificarCambios();
    }
    public void RecuperarVida(int cantidad)
    {
        if (cantidad <= 0 || estaDerrotado)
        {
            return;
        }

        vidaActual = Mathf.Clamp(vidaActual + cantidad, 0, VidaMaxima);

        AlCambiarVida?.Invoke(vidaActual, VidaMaxima);
    }

    public void RecibirDanio(int ataqueEnemigo)
    {
        if (ataqueEnemigo <= 0 || estaDerrotado)
        {
            return;
        }

        int danioRecibido = Mathf.Max(1,ataqueEnemigo - DefensaTotal);

        vidaActual = Mathf.Clamp(vidaActual - danioRecibido,0,VidaMaxima);

        Debug.Log(gameObject.name +" recibió " +danioRecibido +" de daño. Vida: " + vidaActual +"/" + VidaMaxima);

        AlCambiarVida?.Invoke(vidaActual, VidaMaxima);

        if (vidaActual <= 0)
        {
            DerrotarJugador();
        }
    }

    private void DerrotarJugador()
    {
        if (estaDerrotado)
        {
            return;
        }

        estaDerrotado = true;
        vidaActual = 0;

        Debug.Log(gameObject.name + " ha sido derrotado.");

        AlSerDerrotado?.Invoke();
    }
    public void RecuperarEnergia(int cantidad)
    {
        if (cantidad <= 0 || estaDerrotado)
        {
            return;
        }

        energiaActual = Mathf.Clamp(energiaActual + cantidad, 0,EnergiaMaxima);

        AlCambiarEnergia?.Invoke(energiaActual,EnergiaMaxima);
    }

    public bool TieneEnergia(int cantidadNecesaria)
    {
        if (cantidadNecesaria < 0)
        {
            return false;
        }

        return energiaActual >= cantidadNecesaria;
    }

    public bool GastarEnergia(int cantidad)
    {
        if (cantidad < 0 || estaDerrotado)
        {
            return false;
        }

        if (!TieneEnergia(cantidad))
        {
            Debug.Log("No hay suficiente energía.");
            return false;
        }

        energiaActual -= cantidad;

        AlCambiarEnergia?.Invoke( energiaActual,EnergiaMaxima);

        return true;
    }
    public void AplicarBonus(DatosBonus bonus)
    {
        if (bonus == null)
        {
            return;
        }

        ataqueExtra += bonus.ataqueExtra;
        defensaExtra += bonus.defensaExtra;
        vidaMaximaExtra += bonus.vidaMaximaExtra;
        energiaMaximaExtra += bonus.energiaMaximaExtra;
        velocidadExtra += bonus.velocidadExtra;

        AjustarValoresActuales();
        NotificarCambios();
    }

    public void QuitarBonus(DatosBonus bonus)
    {
        if (bonus == null)
        {
            return;
        }

        ataqueExtra -= bonus.ataqueExtra;
        defensaExtra -= bonus.defensaExtra;
        vidaMaximaExtra -= bonus.vidaMaximaExtra;
        energiaMaximaExtra -= bonus.energiaMaximaExtra;
        velocidadExtra -= bonus.velocidadExtra;

        AjustarValoresActuales();
        NotificarCambios();
    }

    public void LimpiarBonificaciones()
    {
        ataqueExtra = 0;
        defensaExtra = 0;
        vidaMaximaExtra = 0;
        energiaMaximaExtra = 0;
        velocidadExtra = 0f;

        AjustarValoresActuales();
        NotificarCambios();
    }
    private void AjustarValoresActuales()
    {
        vidaActual = Mathf.Clamp(vidaActual,0, VidaMaxima);

        energiaActual = Mathf.Clamp(energiaActual,0,EnergiaMaxima);
    }

    private void NotificarCambios()
    {
        AlCambiarVida?.Invoke(vidaActual,VidaMaxima);

        AlCambiarEnergia?.Invoke(energiaActual,EnergiaMaxima);

        AlCambiarEstadisticas?.Invoke();
    }

    private void OnValidate()
    {
        vidaMaximaBase = Mathf.Max(1, vidaMaximaBase);
        energiaMaximaBase = Mathf.Max(1, energiaMaximaBase);
        ataqueBase = Mathf.Max(0, ataqueBase);
        defensaBase = Mathf.Max(0, defensaBase);
        velocidadBase = Mathf.Max(0.1f, velocidadBase);

        velocidadMinima = Mathf.Max(0.1f,velocidadMinima);

        velocidadMaxima = Mathf.Max(velocidadMinima,velocidadMaxima);

        AjustarValoresActuales();
    }
}