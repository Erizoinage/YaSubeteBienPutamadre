using UnityEngine;
using UnityEngine.InputSystem;

// Define aquí las teclas del combate para no depender de tu Input Actions asset.
// Si más adelante prefieres usar tu asset, solo hay que cambiar este script.
public class ControlesCombate : MonoBehaviour
{
    private InputAction accionMover;
    private InputAction accionConfirmar;
    private InputAction accionParry;
    private InputAction accionDash;

    public Vector2 Mover => accionMover.ReadValue<Vector2>();
    public bool ConfirmarPresionado => accionConfirmar.WasPressedThisFrame();
    public bool ParryPresionado => accionParry.WasPressedThisFrame();
    public bool DashPresionado => accionDash.WasPressedThisFrame();

    private void Awake()
    {
        accionMover = new InputAction("Mover", InputActionType.Value);
        accionMover.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        accionMover.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");
        accionMover.AddBinding("<Gamepad>/leftStick");

        accionConfirmar = new InputAction("Confirmar", InputActionType.Button);
        accionConfirmar.AddBinding("<Keyboard>/space");
        accionConfirmar.AddBinding("<Keyboard>/enter");
        accionConfirmar.AddBinding("<Keyboard>/z");
        accionConfirmar.AddBinding("<Gamepad>/buttonSouth");

        accionParry = new InputAction("Parry", InputActionType.Button);
        accionParry.AddBinding("<Keyboard>/space");
        accionParry.AddBinding("<Gamepad>/buttonSouth");

        accionDash = new InputAction("Dash", InputActionType.Button);
        accionDash.AddBinding("<Keyboard>/leftShift");
        accionDash.AddBinding("<Gamepad>/buttonWest");
    }

    private void OnEnable()
    {
        accionMover.Enable();
        accionConfirmar.Enable();
        accionParry.Enable();
        accionDash.Enable();
    }

    private void OnDisable()
    {
        accionMover.Disable();
        accionConfirmar.Disable();
        accionParry.Disable();
        accionDash.Disable();
    }

    private void OnDestroy()
    {
        accionMover.Dispose();
        accionConfirmar.Dispose();
        accionParry.Dispose();
        accionDash.Dispose();
    }
}
