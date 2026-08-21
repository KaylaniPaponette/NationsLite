using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

using UnityObject = UnityEngine.Object;

public interface IUIManagerDelegate
{
    public bool canInteract { get; }
}

public class UIManager : Singleton<UIManager>, IDisposable
{

    public readonly UIDocument uiDocComponent;
    public readonly UIDocument worldSpaceUiDoc;
    List<VisualTreeAsset> uiDocs = new List<VisualTreeAsset>();
    List<StyleSheet> stylesheets = new List<StyleSheet>();
    public UIContainer root { get; private set; }
    public RootViewController rootViewController;
    public UIContainer worldSpaceRoot { get; private set; }

    public IUIManagerDelegate uiDelegate { get; set; }

    public bool canInteract => uiDelegate.canInteract;


    public UIManager()
    {
        worldSpaceUiDoc = CreateUIDocument("WorldSpace UI", "WorldSpacePanelSettings");
        uiDocComponent = CreateUIDocument("UI", "DefaultPanelSettings");

        worldSpaceRoot = CreateRoot(ref worldSpaceUiDoc, "WorldSpaceRoot");
        SetWorldSpaceSize();

        root = CreateRoot(ref uiDocComponent, "Root");
    }

    void SetWorldSpaceSize()
    {
        worldSpaceRoot.style.position = new StyleEnum<Position>(Position.Absolute);
        worldSpaceRoot.style.width = worldSpaceUiDoc.worldSpaceSize.x;
        worldSpaceRoot.style.height = worldSpaceUiDoc.worldSpaceSize.y;
    }

    UIDocument CreateUIDocument(string gameObjectName, string panelSettingsName)
    {
        var uiDoc = new GameObject(gameObjectName, typeof(UIDocument)).GetComponent<UIDocument>();
        uiDoc.panelSettings = AssetManager.instance.LoadAsset<PanelSettings>(panelSettingsName);
        uiDoc.panelSettings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
        uiDoc.panelSettings.referenceResolution = new Vector2Int(2560, 1440);
        uiDoc.panelSettings.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
        uiDoc.panelSettings.match = 0.5f;
        uiDoc.rootVisualElement.Clear();

        if (Application.isPlaying)
            UnityObject.DontDestroyOnLoad(uiDoc.gameObject);

        return uiDoc;
    }

    UIContainer CreateRoot(ref UIDocument uiDoc, string rootName)
    {
        var root = new UIContainer();
        root.name = rootName;
        root.AddToClassList("root-container");
        uiDoc.rootVisualElement.Add(root);
        return root;
    }

    public void LoadViews()
    {
        uiDocs = AssetManager.instance.LoadAssetsOfType<VisualTreeAsset>();
        foreach (var stylesheet in AssetManager.instance.LoadAssetsOfType<StyleSheet>())
            stylesheets.Add(stylesheet);
    }

    public void Init()
    {
        if (rootViewController == null)
        {
            rootViewController = CreateView<RootViewController>(root);
            UIMenuManager.instance.menuContainer = rootViewController.menuContainer;
            WorldUIManager.instance.worldViewContainer = rootViewController.worldViewContainer;
            UITooltipManager.instance.container = rootViewController.tooltipContainer;
        }
    }

    public void Pick(Vector2 screenPosition, out UIComponent target)
    {
        if (root != null && root.panel != null)
        {
            screenPosition.y = Screen.height - screenPosition.y;
            Vector2 panelMousePosition = RuntimePanelUtils.ScreenToPanel(root.panel, screenPosition);
            VisualElement pickedElement = root.panel.Pick(panelMousePosition);
            if (pickedElement != null && pickedElement is UIComponent component)
            {
                target = component.isRaycastBlocked ? null : component;
                return;
            }
        }
        target = null;
    }

    public T CreateView<T>(UIContainer parent)
        where T : UIViewController, new()
    {
        UIDocumentAttribute uiDocAttribute = (UIDocumentAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(UIDocumentAttribute));
        if (uiDocAttribute == null)
        {
            Debug.LogError($"No UIDocument attribute was found attached to viewController {typeof(T)}. Did you forget to add one?");
            return null;
        }

        var viewController = new T();
        var template = GetViewRoot(uiDocAttribute.uiDocName);
        if (template == null)
            return null;

        var uiView = new BaseView(template, parent, viewController);
        viewController.view = uiView;
        viewController.Init();
        viewController.parent = parent;
        viewController.stylesheet = ApplyStyles(uiDocAttribute.stylesheetName);

        return viewController;
    }

    public TemplateContainer GetViewRoot(string docName)
    {
        foreach (var doc in uiDocs)
        {
            if (doc.name != docName)
                continue;
            return doc.Instantiate();
        }

        Debug.LogError($"Cannot create view controller. No view named {docName}.");
        return null;
    }

    StyleSheet ApplyStyles(string stylesheetName)
    {
        if (TryGetStyleSheet(stylesheetName, out var styleSheet))
            root.styleSheets.Add(styleSheet);

        return styleSheet;
    }

    public bool TryGetStyleSheet(string stylesheetName, out StyleSheet styleSheet)
    {
        foreach (var stylesheet in stylesheets)
        {
            if (stylesheet.name == stylesheetName)
            {
                styleSheet = stylesheet;
                return true;
            }
        }

        styleSheet = null;
        return false;
    }

    public void ReleaseView(UIViewController controller)
    {
        var parent = controller.view.parent;
        foreach (var child in controller.view.components)
        {
            if (child.parent == parent)
            {
                parent.Remove(child);
                child.RemoveFromHierarchy();
            }
            else
            {
                Debug.LogWarning($"CHILD {child} PARENT IS NOT: {parent}. Actual parent is: ({child.parent})");
            }
        }
        RemoveStyles(controller.stylesheet);
        controller.OnRelease();
    }

    void RemoveStyles(StyleSheet stylesheet)
    {
        if (stylesheet)
            root.styleSheets.Remove(stylesheet);
    }

    public void CreateHUD()
    {
        rootViewController.CreateHUD();
    }

    public void Dispose()
    {
        Debug.Log("UIMenuManager Dispose");
        if (uiDocComponent)
            UnityEngine.Object.DestroyImmediate(uiDocComponent.gameObject);
    }
}
