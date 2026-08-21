using UnityEngine;
using UnityEngine.UIElements;

public class UITooltipController : UIViewController
{
    protected UIContainer menuContainer;
    const float kMargin = 12f;

    public override void Init()
    {
        menuContainer = view.Find<UIContainer>("TooltipMenu");
    }

    public virtual void Arrange(UIComponent anchor) { }
}