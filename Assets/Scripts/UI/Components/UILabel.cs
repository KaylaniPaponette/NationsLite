using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class UILabel : UIComponent
{
    TextElement _label;
    Font _font;
    int _fontSize;

    [UxmlAttribute]
    public string text
    {
        get => _label.text ?? string.Empty;
        set
        {
            if (_label != null)
                _label.text = value;
        }
    }

    [UxmlAttribute]
    public Font font
    {
        get => _font;
        set
        {
            _font = value;
            if (_label != null && value != null)
            {
                _label.style.unityFont = value;
            }
        }
    }

    [UxmlAttribute]
    public int fontSize
    {
        get => _fontSize;
        set
        {
            _fontSize = value;
            _label.style.fontSize = _fontSize;
        }
    }

    public UILabel()
    {
        pickingMode = PickingMode.Ignore;
        _label = new TextElement();
        _label.pickingMode = PickingMode.Ignore;

        Add(_label);
    }

    public UILabel(string text)
    {
        pickingMode = PickingMode.Ignore;
        _label = new TextElement();
        _label.text = text;
        _label.pickingMode = PickingMode.Ignore;

        Add(_label);
    }
}
