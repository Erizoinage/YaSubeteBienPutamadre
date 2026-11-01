using UnityEngine;

[CreateAssetMenu(fileName = "NuevaArmadura",menuName = "Inventario/Armadura")]
public class DatosArmadura : DatosEquipable
{
    [Header("Estadisticas de defensa")]
    [Min(0)] public int defensaExtra;

    [Min(0)] public float velocidadExtra;

    public DatosBonus CrearBonusArmadura()
    {
        DatosBonus bonus = ScriptableObject.CreateInstance<DatosBonus>();

        bonus.name = "Bonus" + nombreObjeto;
        bonus.nombreBonus = nombreObjeto;
        bonus.defensaExtra = defensaExtra;
        bonus.velocidadExtra = velocidadExtra;
        
        return bonus;
    }
}
