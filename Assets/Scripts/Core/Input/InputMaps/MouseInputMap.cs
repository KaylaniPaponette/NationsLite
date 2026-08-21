using System;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IMouseInputMapHandler : IInputMapHandler
{
    public void OnMouseButtonLeft(bool pressed);
    public void OnMouseButtonRight(bool pressed);
    public void OnMouseScroll(Vector2 scroll);
}

public class MouseInputMap : InputMap<IMouseInputMapHandler>, IDisposable
{
    readonly InputAction mouseLeftAction;
    readonly InputAction mouseRightAction;
    readonly InputAction mousePositionAction;
    readonly InputAction scrollAction;

    public Vector2 mousePosition => mousePositionAction.ReadValue<Vector2>();

    public MouseInputMap()
        : base("Mouse")
    {
        mouseLeftAction = actionMap.FindAction("MouseLeft");
        mouseRightAction = actionMap.FindAction("MouseRight");
        mousePositionAction = actionMap.FindAction("MousePosition");
        scrollAction = actionMap.FindAction("Scroll");

        mouseLeftAction.performed += OnMouseLeftAction;
        mouseRightAction.performed += OnMouseRightAction;
        scrollAction.performed += OnScrollAction;
    }

    void OnMouseLeftAction(InputAction.CallbackContext context)
    {
        OnMouseLeft(context.ReadValueAsButton());
    }
    void OnMouseLeft(bool pressed) => handler?.OnMouseButtonLeft(pressed);

    void OnMouseRightAction(InputAction.CallbackContext context)
    {
        OnMouseRight(context.ReadValueAsButton());
    }
    void OnMouseRight(bool pressed) => handler?.OnMouseButtonRight(pressed);

    void OnScrollAction(InputAction.CallbackContext context)
    {
        OnScroll(context.ReadValue<Vector2>());
    }
    void OnScroll(Vector2 scroll) => handler?.OnMouseScroll(scroll);

    public void Dispose()
    {
        mouseLeftAction.performed -= OnMouseLeftAction;
        mouseRightAction.performed -= OnMouseRightAction;
        scrollAction.performed -= OnScrollAction;
    }
}
