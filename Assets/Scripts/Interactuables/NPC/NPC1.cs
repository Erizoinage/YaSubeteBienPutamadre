using UnityEngine;

public class NPC1 : MonoBehaviour
{
    [Header("Sistema de diálogo")]
    [SerializeField] private SistemaDialogo sistemaDialogo;

    [Header("Datos del NPC")]
    [SerializeField] private string nombreNPC = "Slime";
    [SerializeField] private Sprite retratoNPC;

    private SpriteRenderer spriteRenderer;

    [Header("Orientación")]
    [SerializeField] private bool invertirGiro;

    [Header("Conversación")]
    [TextArea(2, 6)]
    [SerializeField] private string[] lineasDialogo;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void InteractuarNPC()
    {
        MirarAlJugador();

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

    private void MirarAlJugador()
    {
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");

        if (jugador == null)
        {
            Debug.LogWarning(
                name + " no encontró al jugador para mirarlo.",
                this
            );

            return;
        }

        float diferenciaX = jugador.transform.position.x - transform.position.x;

        if (Mathf.Approximately(diferenciaX, 0f))
            return;

        if (spriteRenderer == null)
        {
            Debug.LogWarning(
                name + " no tiene un SpriteRenderer para cambiar su dirección.",
                this
            );

            return;
        }

        spriteRenderer.flipX = (diferenciaX < 0f) != invertirGiro;
    }
}