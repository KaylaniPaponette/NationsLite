using UnityEngine;

public class MouseInputManager : Singleton<MouseInputManager>, IMouseInputMapHandler
{
    struct MouseButtonState
    {
        public bool pressedThisFrame;
        public bool releasedThisFrame;
        public bool pressed;
    }

    public MouseInputMap mouseInputMap { get; set; }
    public IMouseInputReceiver defaultInputHandler;

    
    IMouseInputReceiver mouseCapture;
    IMouseInputReceiver mouseHover;
    MouseButton mouseCaptureButton;
    Vector2 mousePosition;
    Vector2 mouseScrollDelta;
    MouseButtonState leftMouseButtonState;
    MouseButtonState middleMouseButtonState;
    MouseButtonState rightMouseButtonState;
    CursorVisibility _cursorVisibility;

    public bool isOverUI => mouseHover != null && mouseHover is UIComponent;
    
    public CursorVisibility cursorVisibility
    {
        get => _cursorVisibility;
        set
        {
            _cursorVisibility = value;
            if (!enableCursorVisibility)
                value = CursorVisibility.Visible;
            
            mouseActivityTime = Time.timeAsDouble;
        }
    }

    // Will change when porting to mobile devices
    public bool enableCursorVisibility => true;

    double mouseActivityTime;
    bool isBlockingInput;

    bool isMouseInScreenBounds
    {
        get
        {
#if UNITY_EDITOR
            return mousePosition.x >= 0 && mousePosition.y <= Screen.width &&
                mousePosition.y >= 0 && mousePosition.y < Screen.height;
#else
            return true;
#endif
        }
    }

    public void OnUpdate()
    {
        bool didMove = false;
        Vector2 screenPosition = mouseInputMap.mousePosition;
        if (screenPosition != mousePosition)
        {
            didMove = true;
            OnMouseActivity(screenPosition);
        }

        UIComponent raycastTarget = null;
        if (!isBlockingInput)
            UIManager.instance.Pick(screenPosition, out raycastTarget);
        var interactable = raycastTarget?.GetInteractableParent();

        if (mouseInputMap.handler != this)
        {
            OnMouseButtonCancel(ref leftMouseButtonState);
            OnMouseButtonCancel(ref middleMouseButtonState);
            OnMouseButtonCancel(ref rightMouseButtonState);
        }

        bool anyButtonPressed =
            leftMouseButtonState.pressed || leftMouseButtonState.pressedThisFrame ||
            middleMouseButtonState.pressed || middleMouseButtonState.pressedThisFrame ||
            rightMouseButtonState.pressed || rightMouseButtonState.pressedThisFrame;
        
        bool anyButtonReleasedThisFrame =
            leftMouseButtonState.releasedThisFrame ||
            middleMouseButtonState.releasedThisFrame ||
            rightMouseButtonState.releasedThisFrame;
        
        if (mouseCapture != null && !anyButtonPressed && !anyButtonReleasedThisFrame)
            CancelMouse();

        var mouseHover = interactable ?? defaultInputHandler;
        if (mouseCapture != null && mouseHover != mouseCapture)
            mouseHover = null;
        if (!ReferenceEquals(mouseHover, this.mouseHover))
        {
            this.mouseHover?.OnHoverExit();
            this.mouseHover = mouseHover;
            if (this.mouseHover != null)
            {
                MouseHoverEventArgs hoverEvent = new MouseHoverEventArgs()
                {
                    didMove = didMove,
                    anyButtonPressed = anyButtonPressed
                };
                this.mouseHover.OnHoverEnter(hoverEvent);
            }
        }
    
        MouseEventArgs e = default;
        e.screenPosition = screenPosition;
        e.raycastTarget = raycastTarget;
        e.interactable = interactable;

        if (didMove)
            this.mouseHover?.OnHoverUpdate(e);

        if (anyButtonPressed)
            OnMouseActivity(screenPosition);

        if (mouseCapture == null && anyButtonPressed && isMouseInScreenBounds)
        {
            mouseCaptureButton = MouseButton.None;
            if (leftMouseButtonState.pressed)
                mouseCaptureButton = MouseButton.Left;
            else if (middleMouseButtonState.pressed)
                mouseCaptureButton = MouseButton.Middle;
            else if (rightMouseButtonState.pressed)
                mouseCaptureButton = MouseButton.Right;
            mouseCapture = e.interactable ?? defaultInputHandler;
        }

        if (mouseCapture != null)
        {
            DispatchMouseButtonEvents(e, MouseButton.Left, ref leftMouseButtonState);
            DispatchMouseButtonEvents(e, MouseButton.Middle, ref middleMouseButtonState);
            DispatchMouseButtonEvents(e, MouseButton.Right, ref rightMouseButtonState);
        }

        if (mouseScrollDelta != default && isMouseInScreenBounds)
        {
            OnMouseActivity(screenPosition);
            MouseScrollEventArgs scrollEventArgs;
            scrollEventArgs.scrollDelta = new Vector2(mouseScrollDelta.x, -mouseScrollDelta.y);
            scrollEventArgs.screenPosition = screenPosition;

            var scrollView = GetParentScrollHandler(raycastTarget);
            if (scrollView == null)
                defaultInputHandler.OnScroll(scrollEventArgs);
            else
                scrollView.OnScroll(scrollEventArgs);
        }

        if (mouseCapture != null)
        {
            e.button = mouseCaptureButton;
            mouseCapture.OnMouseUpdate(e);
        }

        if (mouseCapture != null && !anyButtonPressed)
            mouseCapture = null;
        
        if (Time.timeAsDouble > mouseActivityTime + 10f && 
            cursorVisibility == CursorVisibility.AutoHide && enableCursorVisibility)
        {
            UnityEngine.Cursor.visible = false;
        }

        EndMouseButtonState(ref leftMouseButtonState);
        EndMouseButtonState(ref middleMouseButtonState);
        EndMouseButtonState(ref rightMouseButtonState);
        mouseScrollDelta = default;
    }

    void OnMouseActivity(Vector2 screenPosition)
    {
        mouseActivityTime = Time.timeAsDouble;
        mousePosition = screenPosition;
        if (_cursorVisibility == CursorVisibility.AutoHide)
            UnityEngine.Cursor.visible = true;
    }

    IMouseInputReceiver GetParentScrollHandler(UIComponent component)
    {
        if (component == null)
            return null;
        if (component.flags.HasFlag(UIComponentFlags.ScrollTarget))
            return component.GetScrollHandler();

       return GetParentWithFlag(UIComponentFlags.ScrollTarget, component);
    }

    IMouseInputReceiver GetParentWithFlag(UIComponentFlags flags, UIComponent component)
    {
        while (component != null && component.GetParent() != null && 
            !component.GetParent().flags.HasFlag(UIComponentFlags.ScrollTarget))
        {
            component = component.GetParent();
        }

        return component?.GetParent()?.GetScrollHandler();
    }

    void OnMouseButtonCancel(ref MouseButtonState buttonState)
    {
        buttonState.pressed = false;
        buttonState.pressedThisFrame = false;
        buttonState.releasedThisFrame = false;
    }

    void CancelMouse()
    {
        mouseCapture?.OnMouseCancel();
        mouseCapture = null;
    }

    void DispatchMouseButtonEvents(MouseEventArgs e, MouseButton mouseButton, ref MouseButtonState state)
    {
        e.button = mouseButton;
        if (state.pressedThisFrame)
            mouseCapture.OnMousePress(e);
        if (state.releasedThisFrame)
            mouseCapture.OnMouseRelease(e);
    }

    void EndMouseButtonState(ref MouseButtonState state)
    {
        state.pressedThisFrame = false;
        state.releasedThisFrame = false;
    }

    public void OnMouseButtonLeft(bool pressed)
    {
        OnMouseButton(pressed, ref leftMouseButtonState);
    }

    public void OnMouseButtonMiddle(bool pressed)
    {
        OnMouseButton(pressed, ref middleMouseButtonState);
    }

    public void OnMouseButtonRight(bool pressed)
    {
        OnMouseButton(pressed, ref rightMouseButtonState);
    }

    void OnMouseButton(bool pressed, ref MouseButtonState buttonState)
    {
        if (pressed)
        {
            buttonState.pressedThisFrame = true;
            buttonState.pressed = true;
        }
        else
        {
            buttonState.releasedThisFrame = true;
            buttonState.pressed = false;
        }
    }

    public void OnMouseScroll(Vector2 scroll)
    {
        mouseScrollDelta += scroll;
    }
}
