using UnityEngine;
[CreateAssetMenu(fileName = "NuevaPocion",menuName = "Inventario/Objeto Pocion")]
public class DatosPocion : DatosObjeto_Base
{
    [Header("Efectos Inmediatos")]

    [Min(0)] public int vidaRecuperada;

    [Min(0)] public int energiaRecuperada;
}
