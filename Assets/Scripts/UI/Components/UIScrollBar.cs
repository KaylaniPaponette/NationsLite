using UnityEngine;
using UnityEngine.UIElements;

public enum ScrollDirection
{
    None,
    Horizontal,
    Vertical
}

[UxmlElement]
public partial class UIScrollBar : UIInteractable
{
    const string kHorizontalStyle = "scrollbar-horizontal";
    const string kVerticalStyle = "scrollbar-vertical";
    ScrollDirection _scrollDirection;

    [UxmlAttribute]
    public ScrollDirection scrollDirection
    {
        get => _scrollDirection;
        set
        {
            _scrollDirection = value;
            if (_scrollDirection == ScrollDirection.Horizontal)
            {
                AddToClassList(kHorizontalStyle);
                RemoveFromClassList(kVerticalStyle);
            }
            else if (_scrollDirection == ScrollDirection.Vertical)
            {
                AddToClassList(kVerticalStyle);
                RemoveFromClassList(kHorizontalStyle);
            }
            else
            {
                // Should this be an option?
                RemoveFromClassList(kHorizontalStyle);
                RemoveFromClassList(kVerticalStyle);
            }
        }
    }

    public UIContainer container { get; set; }

    public const float kScrollSensitivity = 20f; // Standard scroll speed

    public override void OnScroll(MouseScrollEventArgs e)
    {
        if (container == null)
            return;

        // Normalize scroll input to standardize across different input devices
        var normalizedDelta = NormalizeScrollDelta(e.scrollDelta);
        var delta = normalizedDelta * new Vector2(-1, 1);

        if (_scrollDirection == ScrollDirection.Horizontal)
        {
            if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x))
                delta.x = delta.y;
            delta.y = 0;
        }
        else if (_scrollDirection == ScrollDirection.Vertical)
        {
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                delta.y = delta.x;
            delta.x = 0;
        }
        else
        {
            return;
        }

        Vector2 scrollOffset = container.scrollOffset + delta;
        container.scrollOffset = ClampScrollOffset(scrollOffset);
    }

    Vector2 NormalizeScrollDelta(Vector2 delta)
    {
        if (delta.magnitude > 10f)
        {
            Vector2 normalized = delta.normalized * kScrollSensitivity;
            return normalized;
        }
        else
        {
            float trackpadMultiplier = kScrollSensitivity / 2f;
            return delta * trackpadMultiplier;
        }
    }

    Vector2 ClampScrollOffset(Vector2 newScrollOffset)
    {
        var contentSize = container.GetContentSize();
        var containerSize = new Vector2(container.layout.width, container.layout.height);
        contentSize = Vector2.Max(contentSize, containerSize);

        float maxX = Mathf.Max(0, contentSize.x - containerSize.x);
        float maxY = Mathf.Max(0, contentSize.y - containerSize.y);

        return new Vector2
        (
            Mathf.Clamp(newScrollOffset.x, 0, maxX),
            Mathf.Clamp(newScrollOffset.y, 0, maxY)
        );
    }
}
