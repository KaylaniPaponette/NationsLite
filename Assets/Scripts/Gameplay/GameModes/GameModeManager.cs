using System.Collections.Generic;
using UnityEngine;

public class NullGameMode : GameMode { }

public class GameModeManager : Singleton<GameModeManager>, IMouseInputReceiver
{
    public StandardMode standardMode = new StandardMode();
    List<GameMode> gameModes = new List<GameMode>() { new NullGameMode() };
    public GameMode currentMode => gameModes.Count > 0 ? gameModes[^1] : null;
    public GameMode activeMode { get; private set; }
    // public bool isHudVisible => !currentMode.hideHud;
    public bool isPaused;

    public void EnterMode(GameMode mode)
    {
        DeactivateCurrentMode();
        gameModes.Add(mode);
        currentMode.OnEnter();
    }

    public void ReplaceMode(GameMode mode)
    {
        if (mode != currentMode)
        {
            ExitMode(currentMode);
            EnterMode(mode);
        }
    }

    public void ExitMode(GameMode mode)
    {
        if (mode != currentMode)
        {
            Debug.LogWarning($"<<< out of order {mode}");
            if (gameModes.Remove(mode))
                mode.OnExit();
            return;
        }

        DeactivateCurrentMode();
        gameModes.RemoveAt(gameModes.Count - 1);
        mode.OnExit();
    }

    void DeactivateCurrentMode()
    {
        if (activeMode != null)
        {
            activeMode?.OnDeactivate();
            activeMode = null;
        }
    }

    public void OnUpdate()
    {
        ActivateCurrentMode();
        activeMode.OnUpdate();
    }

    public void ActivateCurrentMode()
    {
        if (currentMode != activeMode)
        {
            activeMode = currentMode;
            RefreshHudVisibility();
            activeMode?.OnActivate();
            
        }
    }

    public void RefreshHudVisibility()
    {
        
    }

    public void OnHoverEnter(MouseHoverEventArgs e)
    {
        currentMode.OnHoverEnter(e);
    }

    public void OnHoverExit()
    {
        currentMode.OnHoverExit();
    }

    public void OnHoverUpdate(MouseEventArgs e)
    {
        currentMode.OnHoverUpdate(e);
    }

    public void OnMouseCancel()
    {
        currentMode.OnMouseCancel();
    }

    public void OnMousePress(MouseEventArgs e)
    {
        currentMode.OnMousePress(e);
    }

    public void OnMouseRelease(MouseEventArgs e)
    {
        currentMode.OnMouseRelease(e);
    }

    public void OnMouseUpdate(MouseEventArgs e)
    {
        currentMode.OnMouseUpdate(e);
    }

    public void OnScroll(MouseScrollEventArgs e)
    {
        currentMode.OnScroll(e);
    }
}
