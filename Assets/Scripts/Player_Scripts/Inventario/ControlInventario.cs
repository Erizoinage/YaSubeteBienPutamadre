using UnityEngine;
using UnityEngine.InputSystem;

public class ControlInventario : MonoBehaviour
{
    [Header("interfaz")]
    [SerializeField] private CanvasGroup canvasGroupInventario;

    [Header ("Sistema Jugador")]
    [SerializeField] private MovimientoJugador MVjugador;
    [SerializeField] private InteraccionJugador IntJugador;

    [Header("comportamiento")]
    [SerializeField] private bool cerrarInventarioTecla = true;
    
    private bool inventarioAbierto;
    public bool InventarioAbierto => inventarioAbierto;

    private void Start()
    {
        CerrarInventario();
    }

    public void OnInventario(InputAction.CallbackContext contexto)
    {
        if (!contexto.performed)
        return;

        AlternarInventario();
        
    }
    public void AlternarInventario()
    {
        if (inventarioAbierto)
        {
            if (cerrarInventarioTecla)CerrarInventario();
        }
        else
        {
            AbrirInventario();
        }
    }

    public void AbrirInventario()
    {
       if (inventarioAbierto)
       return;

       inventarioAbierto = true;

       MostrarInventario(true);
       CambiarEstJugador(false); 
    }

    public void CerrarInventario()
    {
        inventarioAbierto = false;

        MostrarInventario(false);
        CambiarEstJugador(true);
    }

    public void MostrarInventario(bool mostrar)
    {
        if (canvasGroupInventario == null)
        {
            Debug.LogWarning("no tiene canvas group asignado pendeju");

            return;
        }

        
        canvasGroupInventario.alpha = mostrar ? 1f : 0f;
        canvasGroupInventario.interactable = mostrar;
        canvasGroupInventario.blocksRaycasts = mostrar;
    }

    private void CambiarEstJugador(bool habilitar)
    {
        if (MVjugador != null)
        {
            MVjugador.EstablecerMovimientoActivo(habilitar);
        }

        if (IntJugador != null)
        {
            IntJugador.EstablecerInteraccionActiva(habilitar);
        
        }
    }

    private void OnDisable()
    {
        if (inventarioAbierto)
        {
            inventarioAbierto = false;
            CambiarEstJugador(true);
        }
    }


    }

