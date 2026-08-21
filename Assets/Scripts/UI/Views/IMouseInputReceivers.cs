using UnityEngine;
using UnityEngine.UIElements;

public enum CursorVisibility
{
    Visible,
    AutoHide,
    Hidden,
}

public enum MouseButton
{
    None = -1,
    Left = 0,
    Middle = 1,
    Right = 2
}

public struct MouseEventArgs
{
    public Vector2 screenPosition;
    public UIComponent raycastTarget;
    public IMouseInputReceiver interactable;
    public MouseButton button;
}

public struct MouseHoverEventArgs
{
    public bool didMove;
    public bool anyButtonPressed;
}

public struct MouseScrollEventArgs
{
    public Vector2 scrollDelta;
    public Vector2 screenPosition;
}

public interface IMouseInputReceiver
{
    void OnMousePress(MouseEventArgs e);
    void OnMouseRelease(MouseEventArgs e);
    void OnMouseUpdate(MouseEventArgs e);
    void OnMouseCancel();

    void OnHoverEnter(MouseHoverEventArgs e);
    void OnHoverExit();
    void OnHoverUpdate(MouseEventArgs e);

    void OnScroll(MouseScrollEventArgs e);
}
