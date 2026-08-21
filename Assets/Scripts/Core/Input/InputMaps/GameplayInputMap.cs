using System;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IGameplayInputMapHandler : IInputMapHandler
{
    public void OnPan(Vector2 direction);
    public void OnPanCancel();

    public void OnBuild();
    public void OnCancel();
}

public class GameplayInputMap : InputMap<IGameplayInputMapHandler>, IDisposable
{
    readonly InputAction panAction;
    readonly InputAction buildAction;
    readonly InputAction cancelAction;

    public GameplayInputMap()
        : base("Gameplay")
    {
        panAction = actionMap.FindAction("CameraPan");
        buildAction = actionMap.FindAction("Build");
        cancelAction = actionMap.FindAction("Cancel");


        panAction.performed += OnPanAction;
        panAction.canceled += OnPanActionCanceled;
        buildAction.performed += OnBuildAction;
        cancelAction.performed += OnCancelAction;
    }

    void OnPanAction(InputAction.CallbackContext context)
    {
        OnPanAction(context.ReadValue<Vector2>());
    }
    void OnPanAction(Vector2 direction) => handler?.OnPan(direction);

    void OnPanActionCanceled(InputAction.CallbackContext context)
    {
        OnPanActionCanceled();
    }
    void OnPanActionCanceled() => handler?.OnPanCancel();

    void OnBuildAction(InputAction.CallbackContext context)
    {
        OnBuildAction();
    }
    void OnBuildAction() => handler?.OnBuild();

    void OnCancelAction(InputAction.CallbackContext context)
    {
        OnCancelAction();
    }
    void OnCancelAction() => handler?.OnCancel();


    public void Dispose()
    {
        panAction.performed -= OnPanAction;
        panAction.canceled -= OnPanActionCanceled;
        buildAction.performed -= OnBuildAction;
        cancelAction.performed -= OnCancelAction;
    }
}
