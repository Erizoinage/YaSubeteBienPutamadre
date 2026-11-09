using UnityEngine;

// Va en el mismo objeto que JugadorEsquive y VisualEquipamiento.
// Cada vez que el panel de combate se activa, copia el casco y la pechera
// que tienes equipados en el inventario al personaje del marco.
[RequireComponent(typeof(VisualEquipamiento))]
public class EquipamientoVisualCombate : MonoBehaviour
{
    [SerializeField] private InventoryManager inventario;

    private VisualEquipamiento visual;

    private void OnEnable()
    {
        Sincronizar();
    }

    public void Sincronizar()
    {
        if (inventario == null)
        {
            Debug.LogWarning("Asigna el InventoryManager para copiar el equipamiento.", this);
            return;
        }

        if (visual == null)
        {
            visual = GetComponent<VisualEquipamiento>();
        }

        DatosEquipable casco = null;
        DatosEquipable armadura = null;

        foreach (SlotEspecial slot in inventario.SlotsEspeciales)
        {
            if (slot == null || slot.EstaVacio() || !(slot.Objeto is DatosEquipable equipamiento))
            {
                continue;
            }

            switch (equipamiento.tipoEquipamiento)
            {
                case TipoEquipamiento.Casco:
                    casco = equipamiento;
                    break;

                case TipoEquipamiento.Pechera:
                    armadura = equipamiento;
                    break;
            }
        }

        visual.EstablecerEquipamiento(casco, armadura);
    }
}
