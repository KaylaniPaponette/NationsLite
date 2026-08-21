using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class UIImage : UIComponent
{
    static CustomStyleProperty<Color> _tintStyle = new CustomStyleProperty<Color>("--ui-button__tint");

    Image _image;
    Sprite _sprite;
    ScaleMode _imageScaleMode;

    // New property for tint color
    [UxmlAttribute]
    public Color tint
    {
        get => _image.tintColor;
        set => _image.tintColor = value;
    }

    [UxmlAttribute]
    public Sprite sprite
    {
        get => _sprite;
        set
        {
            _sprite = value;
            if (_sprite != null)
            {
                _image.image = _sprite.texture;
                _image.sourceRect = _sprite.textureRect;
                _image.uv = SetUV();
            }
        }
    }

    [UxmlAttribute]
    public ScaleMode scaleMode
    {
        get => _imageScaleMode;
        set
        {
            _imageScaleMode = value;
            _image.scaleMode = _imageScaleMode;
        }
    }

    public UIImage()
    {
        _image = new Image();
        _image.pickingMode = PickingMode.Ignore;
        Add(_image);

        RegisterCallback<CustomStyleResolvedEvent>(e => CustomStylesResolved(e));
        pickingMode = PickingMode.Ignore;
    }

    public UIImage(Sprite sprite)
    {
        _image = new Image();
        _image.sprite = sprite;
        _image.pickingMode = PickingMode.Ignore;
        Add(_image);

        RegisterCallback<CustomStyleResolvedEvent>(e => CustomStylesResolved(e));
        pickingMode = PickingMode.Ignore;
    }

    static void CustomStylesResolved(CustomStyleResolvedEvent e)
    {
        UIImage element = (UIImage)e.currentTarget;
        element.UpdateStyles();
    }

    void UpdateStyles()
    {
        if (customStyle.TryGetValue(_tintStyle, out var tint))
            _image.tintColor = tint;
    }
    
    Rect SetUV()
    {
        Rect textureRect = _sprite.textureRect;
        Vector2 textureSize = new Vector2(_sprite.texture.width, _sprite.texture.height);
        Vector2 minUV = textureRect.position / textureSize;
        Vector2 maxUV = (textureRect.position + textureRect.size) / textureSize;

        return new Rect(minUV.x, minUV.y, maxUV.x - minUV.x, maxUV.y - minUV.y);
    }
}