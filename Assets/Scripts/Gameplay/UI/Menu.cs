public class Menu : MenuController
{
    GameMode gameMode;
    
    public override void Init()
    {
        foreach (var child in view.components)
            child.pickingMode = UnityEngine.UIElements.PickingMode.Position;
    }

    public override void OnOpen()
    {
        base.OnOpen();
        BeginGameMode();
        UIManager.instance.rootViewController.RefreshHud();
    }

    protected virtual void BeginGameMode()
    {
        if (GameModeManager.instance.currentMode is not MenuMode)
        {
            gameMode = new MenuMode(this);
            GameModeManager.instance.EnterMode(gameMode);
        }
    }

    public virtual void Close()
    {
        UIMenuManager.instance.CloseMenu(this);
    }

    public override void OnClose()
    {
        base.OnClose();
        EndGameMode();
        UIManager.instance.rootViewController.RefreshHud();
    }

    protected virtual void EndGameMode()
    {
        if (gameMode != null)
            GameModeManager.instance.ExitMode(gameMode);
        gameMode = null;
    }
}
