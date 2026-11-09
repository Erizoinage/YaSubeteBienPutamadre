using UnityEngine;

[CreateAssetMenu(fileName = "NuevoBonus" ,menuName = "Combate/Bonus")]
public class DatosBonus : ScriptableObject
{
    [Header("Nombre y descripcion del bonus")]
    public string nombreBonus;

    [TextArea(1,3)]
    public string descripcion;

    [Header("Modificadores de estadisticas")]
    public int ataqueExtra;
    public int defensaExtra;
    public int vidaMaximaExtra;
    public int energiaMaximaExtra;
    public float velocidadExtra;
    [Min(0f)] public float bonusDanio;

    public bool TieneAlgunaModificacion()
    {
        return ataqueExtra != 0 ||defensaExtra != 0 ||vidaMaximaExtra != 0 
        ||energiaMaximaExtra != 0 ||!Mathf.Approximately(velocidadExtra, 0f)
        ||!Mathf.Approximately(bonusDanio, 0f);
    }
}
