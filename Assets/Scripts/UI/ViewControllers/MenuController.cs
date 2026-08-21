using System.Collections.Generic;
using UnityEngine;

public abstract class MenuController : UIViewController
{
    public virtual void OnOpen() { }

    public virtual void OnClose(){ }

    public virtual void OnBackgroundClick()
    {
        UIMenuManager.instance.CloseMenu(this);
    }

    public virtual void OnMenuOccluded()
    {
        view.isRaycastBlocked = true;
    }

    public virtual void OnUpdate() { }

}
