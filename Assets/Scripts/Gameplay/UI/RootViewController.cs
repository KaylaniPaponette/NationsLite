using UnityEngine;

[UIDocument(uiDocName: "Root", stylesheetName: "_Main")]
public class RootViewController : UIViewController
{
    public UIContainer menuContainer;
    public UIContainer worldViewContainer;
    public UIContainer tooltipContainer;
    public HUDViewController hud;

    public override void Init()
    {
        menuContainer = view.Find<UIContainer>("MenuContainer");
        worldViewContainer = view.Find<UIContainer>("WorldUIContainer");
        tooltipContainer = view.Find<UIContainer>("TooltipContainer");
    }

    public void CreateHUD()
    {
        if (hud != null)
            ClearHUD();
        
        var parent = view.Find<UIContainer>("Hud");
        hud = UIManager.instance.CreateView<HUDViewController>(parent);
        hud.Setup();
    }

    public void ClearHUD()
    {
        UIManager.instance.ReleaseView(hud);
        hud = null;
    }

    public void RefreshHud()
    {
        hud.Refresh();
    }
}
