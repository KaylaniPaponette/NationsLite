using UnityEngine;

[UIDocument("AttractionStateProgress")]
public class AttractionStateProgress : UIViewController
{
    public UIContainer progressBarContainer;
    public UIProgress progressBar;

    public bool isDisplayed => progressBarContainer.displayed;

    GameDateTime targetTime;
    GameTimeSpan totalTime;

    public override void Init()
    {
        base.Init();
        progressBarContainer = view.Find<UIContainer>();
        progressBar = view.Find<UIProgress>();
    }

    public void Setup(GameDateTime targetTime, GameTimeSpan totalTime)
    {
        this.targetTime = targetTime;
        this.totalTime = totalTime;
    }

    public override void OnAnimate(Vector2 uiPosition, float zoomRatio)
    {
        base.OnAnimate(uiPosition, zoomRatio);
        float progress = 1f - (targetTime - GameState.instance.dateTime).totalMilliseconds / (float)totalTime.totalMilliseconds;
        progressBar.progress = progress;
    }
}