using System;
using System.Collections.Generic;
using UnityEngine;

public class UIMenuManager : Singleton<UIMenuManager>
{
    public Action menuOpened;   // GameUI to subscribe to have hud raycasting blocked
    public Action menuClosed;
    public UIContainer menuContainer { get; set; }
    public List<MenuController> menuStack = new List<MenuController>();
    public MenuController topMenu => menuStack.Count >= 1 ? menuStack[^1] : null;

    public T OpenMenu<T>()
        where T : MenuController, new()
    {
        var menu = UIManager.instance.CreateView<T>(menuContainer);
        if (topMenu != null)
            topMenu.OnMenuOccluded();
        menuStack.Add(menu);
        menu.OnOpen();

        menuOpened?.Invoke();
        Debug.Log($"[MENU]: open {menu}");
        return (T)menu;
    }

    public void CloseMenu(MenuController menu)
    {
        if (topMenu != menu)
        {
            Debug.LogError($"Cannot close menu as it is not the top menu. {topMenu} is topMenu");
            return;
        }

        menuStack.Remove(menu);
        menu.OnClose();
        UIManager.instance.ReleaseView(menu);
        menuClosed?.Invoke();

        if (topMenu != null)
            topMenu.view.isRaycastBlocked = false;
    }

    public void CloseAllMenus()
    {
        while (topMenu != null)
            CloseMenu(topMenu);
    }

    public void OnUpdate()
    {
        if (topMenu != null)
            topMenu.OnUpdate();
    }
}
