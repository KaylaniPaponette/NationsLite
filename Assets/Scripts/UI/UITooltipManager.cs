using UnityEngine;

public class UITooltipManager : Singleton<UITooltipManager>
{
    public UIContainer container;
    UITooltipController tooltip;
    UIInteractable anchor;
    UIViewController owner;

    public T CreateTooltip<T>()
        where T : UITooltipController, new()
    {
        return UIManager.instance.CreateView<T>(container);
    }

    public void OnHoverExit()
    {
        HideTooltip();
    }

    void HideTooltip()
    {
        if (tooltip != null)
            UIManager.instance.ReleaseView(tooltip);

        tooltip = null;
        anchor = null;
        owner = null;
    }

    public void OnHoverEnter(UIInteractable anchor)
    {
        ShowTooltip(anchor);
    }

    void ShowTooltip(UIInteractable anchor)
    {
        HideTooltip();
        this.anchor = anchor;
        this.owner = FindViewControllerInParent();
        if (owner != null)
        {
            tooltip = owner.CreateTooltip();
            if (tooltip != null)
                tooltip.Arrange(anchor);
        }
    }

    UIViewController FindViewControllerInParent()
    {
        if (anchor.view != null && anchor.view.viewController != null)
            return anchor.view.viewController;
            
        var parent = anchor.GetParent();
        while (parent != null)
        {
            if (parent.view != null && parent.view.viewController != null)
                return parent.view.viewController;
            
            parent = parent.GetParent();
        }

        return null;
    }
}