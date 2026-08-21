using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public class WorldUIManager : Singleton<WorldUIManager>, IDisposable
{
    List<UIViewController> worldViews = new List<UIViewController>();    
    public UIContainer worldViewContainer { get; set; }
    public float referenceOrthographicSize { get; set; } = 3f;

    public T CreateWorldView<T>(string anchorComponentName, Transform anchor, Vector3 anchorOffset = new Vector3())
        where T : UIViewController, new()
    {
        T viewController = UIManager.instance.CreateView<T>(worldViewContainer);
        viewController.anchor = anchor;
        viewController.anchorOffset = anchorOffset;
        viewController.anchorComponent = viewController.view.Find<UIComponent>(anchorComponentName);

        worldViews.Add(viewController);
        Debug.Log($"[WORLD VIEW]: Open {viewController}");
        
        return viewController;
    }

    public void ReleaseWorldView(UIViewController viewController)
    {
        UIManager.instance.ReleaseView(viewController);
        worldViews.Remove(viewController);
    }

    public void ClearAllWorldViews()
    {
        foreach (var worldView in worldViews)
        {
            if (worldView.anchor)
            {
                worldView.anchor.gameObject.SafeDestroyImmediate();
                UIManager.instance.ReleaseView(worldView);
            }
        }   
        worldViews.Clear();
    }

    public void OnLateUpdate(Camera camera)
    {
        using (ListPool<UIViewController>.Get(out var viewsToDelete))
        {
            foreach (var worldView in worldViews)
            {
                Vector3 screenPosition = camera.WorldToScreenPoint(worldView.anchor.TransformPoint(worldView.anchorOffset));
                screenPosition.y = Screen.height - screenPosition.y;
                
                Vector2 panelPosition = RuntimePanelUtils.ScreenToPanel(worldViewContainer.panel, screenPosition);
                Vector2 uiPosition = worldViewContainer.WorldToLocal(panelPosition);

                worldView.OnAnimate(uiPosition, camera.orthographicSize / referenceOrthographicSize);

                if (worldView.isReadyForDeletion)
                    viewsToDelete.Add(worldView);
            }

            foreach (var view in viewsToDelete)
                ReleaseWorldView(view);
            
        }
    }

    public void Dispose()
    {
        Debug.Log("WorldUIManager Dispose");
        worldViews.Clear();
    }
}
