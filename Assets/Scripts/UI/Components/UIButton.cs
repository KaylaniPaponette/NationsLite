using System;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class UIButton : UIInteractable
{
    protected const string kButtonClassName = "ui-button";
    public Action clicked;

    public string text
    {
        get
        {
            if (label == null)
                return null;
            return label.text;
        }
        set
        {
            if (label == null)
            {
                label = new UILabel(text);
                Add(label);
            }
            else
                label.text = value;
        }
    }

    UILabel label;
    UIImage image;
    
    public UIButton()
    {
        AddToClassList(kButtonClassName);
        pickingMode = PickingMode.Position;
    }

    public UIButton(Sprite sprite)
    {
        this.image = new UIImage(sprite);
        pickingMode = PickingMode.Position;
        Add(image);
    }

    public UIButton(string labelText)
    {
        this.label = new UILabel(labelText);
        pickingMode = PickingMode.Position;
        Add(label);
    }

    public UIButton(UILabel label)
    {
        this.label = label;
        pickingMode = PickingMode.Position;
        Add(label);
    }

    public override void OnMousePress(MouseEventArgs e)
    {
        if (disabled)
            return;
            
        OnClick();
        base.OnMousePress(e);
    }

    protected virtual void OnClick()
    {
        clicked?.Invoke();
    }
}
