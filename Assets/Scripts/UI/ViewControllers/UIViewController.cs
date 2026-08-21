using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class UIViewController
{
    public BaseView view { get; set; }
    public UIComponent parent;
    public StyleSheet stylesheet;
    public Transform anchor;
    public Vector3 anchorOffset;
    public UIComponent anchorComponent { get; set; }
    public virtual bool adjustToZoom => false;
    public virtual bool isReadyForDeletion => false;
    
    List<UIViewController> _children = new List<UIViewController>();
    public int childrenCount => _children.Count;

    public virtual void Init() { }

    public virtual void OnRelease() { }

    public T CreateChild<T>(UIContainer container)
        where T : UIViewController, new()
    {
        T viewController = UIManager.instance.CreateView<T>(container);
        _children.Add(viewController);
        return viewController;
    }

    public void ReleaseChild(UIComponent child)
    {
        var controller = _children.Find(c => c.view.components.Contains(child));
        if (controller != null)
        {
            if (view != null && controller.view != null)
            {
                foreach (var component in controller.view.components)
                {
                    view.components.Remove(component);
                    component.RemoveFromHierarchy();
                }
            }
            controller.OnRelease();
            _children.Remove(controller);
        }
    }

    public void ClearChildren()
    {
        foreach (var child in _children)
        {
            if (view != null && child.view != null)
            {
                foreach (var component in child.view.components)
                {
                    view.components.Remove(component);
                    component.RemoveFromHierarchy();
                }
            }
            child.OnRelease();
        }
        _children.Clear();
    }

    public virtual void OnAnimate(Vector2 uiPosition, float zoomRatio)
    {
        anchorComponent.style.left = uiPosition.x - anchorComponent.resolvedStyle.width * 0.5f;
        anchorComponent.style.top = uiPosition.y;

        if (adjustToZoom)
        {
            float ratio = Mathf.Clamp(zoomRatio, 1f, 1.5f);
            var componentHeight = anchorComponent.resolvedStyle.height;
            anchorComponent.style.top = uiPosition.y - componentHeight * ratio;
        }
    }

    public virtual UITooltipController CreateTooltip() => null;
}
