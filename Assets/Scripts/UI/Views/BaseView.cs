using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseView
{
    public UIViewController viewController;

    List<UIComponent> _components;
    public List<UIComponent> components => _components;

    UIComponent _parent;
    public UIComponent parent => _parent;

    bool raycastBlocked;
    public bool isRaycastBlocked
    {
        get => raycastBlocked;
        set
        {
            raycastBlocked = value;
            OnRaycastBlockedChanged();
        }
    }

    public BaseView(TemplateContainer template, UIContainer parent, UIViewController viewController)
    {
        _parent = parent;
        _components = new List<UIComponent>();
        this.viewController = viewController;

        //Collect all top-level UIComponents, removing the TemplateContainer from hierarchy
        var topLevelComponents = new List<UIComponent>();
        while (template.childCount > 0)
        {
            VisualElement child = template[0];
            template.Remove(child);

            if (child is UIComponent component)
            {
                component.view = this;
                topLevelComponents.Add(component);
                _components.Add(component);
            }
        }
        // Add non-TemplateContainer children into parent's hierarchy
        foreach (var component in topLevelComponents)
            parent.Add(component);
    }

    public TElement Find<TElement>(string elementName = null) where TElement : UIComponent
    {
        foreach (var view in _components)
        {
            var element = string.IsNullOrEmpty(elementName) 
                ? view.Q<TElement>() 
                : view.Q<TElement>(elementName);
            
            if (element != null)
                return element;
        }
        return null;
    }
    
    public void Bind<T>(Action<T> setter, BindableProperty<T> property)
    {
        setter(property.Value);
        property.OnValueChanged += setter;
    }
    
    void OnRaycastBlockedChanged()
    {
        foreach (UIComponent component in components.Cast<UIComponent>())
            component.isRaycastBlocked = raycastBlocked;
    }
}
