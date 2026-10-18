using UnityEngine;

public class NPC1 : MonoBehaviour
{
    [Header("Sistema de diálogo")]
    [SerializeField] private SistemaDialogo sistemaDialogo;

    [Header("Datos del NPC")]
    [SerializeField] private string nombreNPC = "Slime";
    [SerializeField] private Sprite retratoNPC;

    [Header("Conversación")]
    [TextArea(2, 6)]
    [SerializeField] private string[] lineasDialogo;

    public void InteractuarNPC()
    {
        if (sistemaDialogo == null)
        {
            Debug.LogWarning(
                name + " no tiene asignado SistemaDialogo."
            );

            return;
        }

        if (sistemaDialogo.DialogoActivo)
        {
            sistemaDialogo.AvanzarDialogo();
            return;
        }

        sistemaDialogo.IniciarDialogo(
            nombreNPC,
            lineasDialogo,
            retratoNPC
        );
    }
}