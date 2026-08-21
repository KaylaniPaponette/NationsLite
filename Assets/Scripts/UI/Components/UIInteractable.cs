using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class UIInteractable : UIComponent, IMouseInputReceiver
{

    [UxmlAttribute("tooltipText")]
    public string tooltipText { get; set; }

    public bool isHovering => this.HasPseudoFlag(PseudoState.Hover);
    public bool isActive => this.HasPseudoFlag(PseudoState.Active);

    bool _dimmed;
    public bool dimmed
    {
        get => _dimmed;
        set
        {
            _dimmed = value;
            SetEnabled(_dimmed);
        }
    }

    bool _disabled;
    public bool disabled
    {
        get => _disabled;
        set
        {
            _disabled = value;
            SetEnabled(!_disabled);
        }
    }

    
    public virtual void OnHoverEnter(MouseHoverEventArgs e)
    {
        if (!isRaycastBlocked)
            this.AddPsuedoState(PseudoState.Hover);
            
        UITooltipManager.instance.OnHoverEnter(this);
    }

    public virtual void OnHoverExit()
    {
        this.RemovePsuedoState(PseudoState.Hover);
        UITooltipManager.instance.OnHoverExit();
    }

    public virtual void OnHoverUpdate(MouseEventArgs e) { }

    public virtual void OnMouseCancel() { }

    public virtual void OnMousePress(MouseEventArgs e)
    {
        if (!isRaycastBlocked)
            this.AddPsuedoState(PseudoState.Active);
    }

    public virtual void OnMouseRelease(MouseEventArgs e)
    {
        this.RemovePsuedoState(PseudoState.Active);
    }

    public virtual void OnMouseUpdate(MouseEventArgs e) { }

    public virtual void OnScroll(MouseScrollEventArgs e) { }

}
