using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemID;
    public string Nombre;
    public Sprite icon;
    public int maxStackSize=1;

    public string descripcion;    
}
