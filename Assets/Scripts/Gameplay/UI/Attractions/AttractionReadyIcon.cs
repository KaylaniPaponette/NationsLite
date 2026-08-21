using UnityEngine;

[UIDocument("AttractionReadyIcon")]
public class AttractionReadyIcon : UIViewController
{
    public UIImage readyIcon;
    public bool isDisplayed => readyIcon.displayed;

    public override void Init()
    {
        base.Init();
        readyIcon = view.Find<UIImage>();
    }

    public override void OnAnimate(Vector2 uiPosition, float zoomRatio)
    {
        if (!readyIcon.displayed)
            return;

        base.OnAnimate(uiPosition, zoomRatio);
        float bounceOffset = Mathf.Sin(Time.time * 3f) * 12f;

        readyIcon.style.top = uiPosition.y + bounceOffset;
    }
}