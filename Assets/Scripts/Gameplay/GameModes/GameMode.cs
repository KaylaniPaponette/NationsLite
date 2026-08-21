using UnityEngine;

public abstract class GameMode : IMouseInputReceiver
{
    public virtual void OnEnter() { }
    public virtual void OnActivate() { }
    public virtual void OnDeactivate() {}
    public virtual void OnUpdate() {}
    public virtual void OnExit() {}

    public virtual void OnMousePress(MouseEventArgs e)
    { 
        // if (GameUI.instance.worldUIMenu != null)
        //     GameUI.instance.CloseWorldUIMenu();

        if (UIMenuManager.instance.topMenu != null && e.raycastTarget == null)
            UIMenuManager.instance.topMenu.OnBackgroundClick();
    }

    public virtual void OnMouseRelease(MouseEventArgs e) {}

    public virtual void OnMouseUpdate(MouseEventArgs e) {}

    public virtual void OnMouseCancel() {}

    public virtual void OnHoverEnter(MouseHoverEventArgs e) {}

    public virtual void OnHoverExit() {}

    public virtual void OnHoverUpdate(MouseEventArgs e) {}

    public virtual void OnScroll(MouseScrollEventArgs e) {}

    public virtual void Pause()
    {
        GameModeManager.instance.isPaused = true;
        Time.timeScale = 0;
        
        // UIMenuManager.instance.OpenMenu<PauseMenu>();
    }
    
    public virtual void Unpause()
    {
        GameModeManager.instance.isPaused = false;
        Time.timeScale = 1;
    }
}