using UnityEngine;

public class MenuMode : GameMode, IGameplayInputMapHandler
{
    Menu menu;

    public MenuMode(Menu menu)
    {
        this.menu = menu;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        InputManager.instance.PushInputHandler(this);
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        InputManager.instance.PopInputHandler(this);
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public void OnCancel()
    {
        UIMenuManager.instance.CloseAllMenus();
    }

    public void OnPan(Vector2 direction) { }

    public void OnPanCancel() { }


    public void OnBuild() { }

}
