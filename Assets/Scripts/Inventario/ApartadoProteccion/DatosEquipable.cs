using UnityEngine;

public enum TipoEquipamiento
{
    Casco,
    Pechera
}

[CreateAssetMenu(fileName = "NuevoEquipable",menuName = "Inventario/Objeto Equipable")]
public class DatosEquipable : DatosObjeto_Base
{
    [Header("Tipo de equipamiento")]
    public TipoEquipamiento tipoEquipamiento;

    [Header("Sprites equipado por direccion")]
    public Sprite spriteFrente;
    public Sprite spriteEspalda;
    public Sprite spriteLaterales;


    public Sprite ObtenerSprite(DireccionJugador direccion)
    {
        switch (direccion)
        {
            case DireccionJugador.Arriba:
                return spriteEspalda;

            case DireccionJugador.Abajo:
                return spriteFrente;

            case DireccionJugador.Izquierda:
            case DireccionJugador.Derecha:
                return spriteLaterales;

            default:
                return null;
        }
    }
}