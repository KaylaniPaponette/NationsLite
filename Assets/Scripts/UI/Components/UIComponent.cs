using UnityEngine;
using UnityEngine.UIElements;


public enum UIComponentFlags
{
    ScrollTarget = 1 << 1,
}

// A UIComponent is UI Item. It shares the same hierarchy, layout, and rendering as a VisualElement
// but input is handled separately.
public abstract class UIComponent : VisualElement
{
    protected UIComponentFlags _flags;
    public UIComponentFlags flags => _flags;
    public BaseView view;

    bool _raycastBlocked;
    public bool isRaycastBlocked
    {
        get => _raycastBlocked;
        set
        {
            _raycastBlocked = value;
            OnRaycastBlockChanged();
        }
    }

    bool _displayed;
    public bool displayed
    {
        get => _displayed;
        set
        {
            _displayed = value;
            style.display = _displayed ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    public void OnRaycastBlockChanged()
    {
        pickingMode = _raycastBlocked ? PickingMode.Ignore : PickingMode.Position;
        foreach (var child in Children())
        {
            if (child is UIComponent component)
                component.isRaycastBlocked = _raycastBlocked;
        }
    }

    public virtual IMouseInputReceiver GetScrollHandler() => this as IMouseInputReceiver;

    public UIInteractable GetInteractableParent()
    {
        for (VisualElement view = this; view != null; view = view.parent)
        {
            if (view is UIInteractable interactable)
                return interactable;
        }
        return null;
    }

    public UIComponent GetParent()
    {
        if (parent is UIComponent component)
            return component;
        else
            return null;
    }
}
