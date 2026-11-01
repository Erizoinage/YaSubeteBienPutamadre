using UnityEngine;

[System.Serializable]
public class CuracionActiva
{
    [SerializeField] private int curacionTotal;
    [SerializeField] private int curacionRestante;

    [SerializeField] private float duracionTotal;
    [SerializeField] private float tiempoRestante;
    [SerializeField] private float intervalo;

    [SerializeField] private float tiempoDesdeUltimoPulso;

    public bool HaTerminado =>curacionRestante <= 0 ||tiempoRestante <= 0f;

    public CuracionActiva(int cantidadTotal,float duracionSegundos,float intervaloSegundos)
    {
        curacionTotal = Mathf.Max(0, cantidadTotal);
        curacionRestante = curacionTotal;

        duracionTotal = Mathf.Max( 0.1f,duracionSegundos);

        tiempoRestante = duracionTotal;

        intervalo = Mathf.Clamp(intervaloSegundos,0.1f,duracionTotal);

        tiempoDesdeUltimoPulso = 0f;
    }

    public int ProcesarTiempo(float deltaTime)
    {
        if (HaTerminado || deltaTime <= 0f)
        {
            return 0;
        }

        tiempoRestante -= deltaTime;
        tiempoDesdeUltimoPulso += deltaTime;

        bool debeCurar = tiempoDesdeUltimoPulso >= intervalo ||tiempoRestante <= 0f;

        if (!debeCurar)
        {
            return 0;
        }

        tiempoDesdeUltimoPulso -= intervalo;

        if (tiempoDesdeUltimoPulso < 0f)
        {
            tiempoDesdeUltimoPulso = 0f;
        }

        int pulsosRestantes = Mathf.Max(1, Mathf.CeilToInt(Mathf.Max(0f, tiempoRestante) /intervalo) + 1
        );

        int cantidadEstePulso = Mathf.CeilToInt((float)curacionRestante /pulsosRestantes);

        cantidadEstePulso = Mathf.Clamp(cantidadEstePulso,0,curacionRestante);

        curacionRestante -= cantidadEstePulso;

        if (tiempoRestante <= 0f)
        {
            cantidadEstePulso += curacionRestante;
            curacionRestante = 0;
        }

        return cantidadEstePulso;
    }
}