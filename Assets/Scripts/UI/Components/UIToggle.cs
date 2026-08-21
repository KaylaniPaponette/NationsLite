using System;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class UIToggle : UIButton
{
    const string kToggleClassName = "ui-toggle";

    public Action<bool> valueChanged;

    bool _value;


    [UxmlAttribute]
    public bool toggleValue
    {
        get => _value;
        set
        {
            _value = value;
            valueChanged?.Invoke(_value);
            OnToggleValueChanged();
        }
    }

    public UIToggle()
    {
        AddToClassList(kToggleClassName);
        RemoveFromClassList(kButtonClassName);
    }

    protected override void OnClick()
    {
        _value = !_value;
        base.OnClick();
        valueChanged?.Invoke(_value);
    }

    void OnToggleValueChanged()
    {
        valueChanged?.Invoke(_value);
        MarkDirtyRepaint();
    }
}
