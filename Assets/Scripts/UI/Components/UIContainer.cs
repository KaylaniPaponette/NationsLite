using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class UIContainer : UIComponent
{
    UIScrollBar scrollBar;
    ScrollDirection _scrollDirection;
    bool _isScrollTarget;
    
    Vector2 _scrollOffset;
    public Vector2 scrollOffset
    { 
        get => _scrollOffset;
        set
        {
            _scrollOffset = value;
            OnScrollOffsetChanged();
        }
    }

    public void OnScrollOffsetChanged()
    {
        foreach (var child in Children())
        {
            if (child is UIComponent component)
            {
                component.style.translate = new Translate(
                    new Length(-scrollOffset.x), 
                    new Length(-scrollOffset.y));
            }
        }
    }

    [UxmlAttribute]
    public bool isScrollTarget
    {
        get => _isScrollTarget;
        set => SetScrollTarget(value);
    }


    [UxmlAttribute]
    public ScrollDirection scrollDirection
    {
        get => _scrollDirection;
        set
        {
            if (scrollBar == null)
            {
                _scrollDirection = ScrollDirection.None;
                return;
            }
            scrollBar.scrollDirection = value;
            _scrollDirection = value;
        }
    }

    public UIContainer()
    {
        pickingMode = PickingMode.Ignore;
    }

    public override IMouseInputReceiver GetScrollHandler() => scrollBar;

    public void SetScrollTarget(bool scrollTarget)
    {
        _isScrollTarget = scrollTarget;
        if (_isScrollTarget)
        {
            if (scrollBar != null)
                return;

            scrollBar = new UIScrollBar();
            scrollBar.container = this;
            Add(scrollBar);

            pickingMode = PickingMode.Position;
            _flags |= UIComponentFlags.ScrollTarget;
            style.overflow = Overflow.Hidden;
            style.flexShrink = 0;
            style.flexGrow = 0;
            style.flexWrap = Wrap.NoWrap;

            foreach (var child in Children())
            {
                child.style.flexShrink = 0;
                child.style.flexGrow = 0;
            }
        }
        else
        {
            if (scrollBar == null)
                return;
            
            Remove(scrollBar);
            scrollBar.container = null;
            scrollBar = null;

            pickingMode = PickingMode.Ignore;
            _flags &= ~UIComponentFlags.ScrollTarget;
            style.overflow = Overflow.Visible;
            style.flexShrink = 1;
            style.flexWrap = Wrap.Wrap;
        }

        MarkDirtyRepaint();
    }

    public Vector2 GetContentSize()
    {
        float width = 0;
        float height = 0;

        foreach (var child in Children())
        {
            var childRight = child.layout.x + child.layout.width;
            var childBottom = child.layout.y + child.layout.height;

            width = Mathf.Max(width, childRight);
            height = Mathf.Max(height, childBottom);
        }

        return new Vector2(width, height);
    }
}
