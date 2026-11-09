using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Entradas del combate. Si arrastras acciones de tu Input Actions asset
// a los campos de abajo, usa esas. Los campos que dejes vacíos usan teclas
// por defecto creadas por código, así que funciona aunque no configures nada.
public class ControlesCombate : MonoBehaviour
{
    [Header("Acciones de tu Input Actions (opcional)")]
    [SerializeField] private InputActionReference moverReferencia;
    [SerializeField] private InputActionReference confirmarReferencia;
    [SerializeField] private InputActionReference parryReferencia;
    [SerializeField] private InputActionReference dashReferencia;

    private InputAction accionMover;
    private InputAction accionConfirmar;
    private InputAction accionParry;
    private InputAction accionDash;

    // Solo las acciones creadas aquí se desactivan y se liberan al final.
    // Las de tu asset pertenecen al Player Input y no se tocan.
    private readonly List<InputAction> accionesPropias = new List<InputAction>();

    public Vector2 Mover => accionMover.ReadValue<Vector2>();
    public bool ConfirmarPresionado => accionConfirmar.WasPressedThisFrame();
    public bool ParryPresionado => accionParry.WasPressedThisFrame();
    public bool DashPresionado => accionDash.WasPressedThisFrame();

    private void Awake()
    {
        accionMover = Obtener(moverReferencia) ?? CrearMover();
        accionConfirmar = Obtener(confirmarReferencia) ?? CrearConfirmar();
        accionParry = Obtener(parryReferencia) ?? CrearParry();
        accionDash = Obtener(dashReferencia) ?? CrearDash();
    }

    private void OnEnable()
    {
        // Enable() no hace nada si la acción ya estaba activa.
        accionMover.Enable();
        accionConfirmar.Enable();
        accionParry.Enable();
        accionDash.Enable();
    }

    private void OnDisable()
    {
        foreach (InputAction accion in accionesPropias)
        {
            accion.Disable();
        }
    }

    private void OnDestroy()
    {
        foreach (InputAction accion in accionesPropias)
        {
            accion.Dispose();
        }
    }

    private static InputAction Obtener(InputActionReference referencia)
    {
        return referencia != null ? referencia.action : null;
    }

    private InputAction CrearMover()
    {
        InputAction accion = new InputAction("Mover", InputActionType.Value);
        accion.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        accion.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");
        accion.AddBinding("<Gamepad>/leftStick");

        accionesPropias.Add(accion);
        return accion;
    }

    private InputAction CrearConfirmar()
    {
        InputAction accion = new InputAction("Confirmar", InputActionType.Button);
        accion.AddBinding("<Keyboard>/space");
        accion.AddBinding("<Keyboard>/enter");
        accion.AddBinding("<Keyboard>/z");
        accion.AddBinding("<Gamepad>/buttonSouth");

        accionesPropias.Add(accion);
        return accion;
    }

    private InputAction CrearParry()
    {
        InputAction accion = new InputAction("Parry", InputActionType.Button);
        accion.AddBinding("<Keyboard>/space");
        accion.AddBinding("<Gamepad>/buttonSouth");

        accionesPropias.Add(accion);
        return accion;
    }

    private InputAction CrearDash()
    {
        InputAction accion = new InputAction("Dash", InputActionType.Button);
        accion.AddBinding("<Keyboard>/leftShift");
        accion.AddBinding("<Gamepad>/buttonWest");

        accionesPropias.Add(accion);
        return accion;
    }
}