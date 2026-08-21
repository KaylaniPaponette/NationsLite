[UIDocument(uiDocName: "HUD", stylesheetName: "HUDStyles")]
public class HUDViewController : UIViewController
{
    public UIContainer hudContainer;
    public UIContainer currencyContainer;
    public UIButton buildMenuButton;

    public CurrencyViewController attractionPoints;

    bool isAnyMenuOpen => UIMenuManager.instance.topMenu != null;

    public override void Init()
    {
        hudContainer = view.Find<UIContainer>(nameof(hudContainer));
        currencyContainer = view.Find<UIContainer>(nameof(currencyContainer));
        buildMenuButton = view.Find<UIButton>(nameof(buildMenuButton));

        // Open the build menu on click
        buildMenuButton.clicked += () => UIMenuManager.instance.OpenMenu<BuildMenuController>();

        // Refresh button state on open/close
        UIMenuManager.instance.menuOpened += Refresh;
        UIMenuManager.instance.menuClosed += Refresh;
    }

    public void Setup()
    {
        ClearChildren();

        attractionPoints = CreateChild<CurrencyViewController>(currencyContainer);

        var currency = BuildData.instance.attractionPoints;
        var currentCurrency = GameState.instance.attractionPoints;
        attractionPoints.Setup(currency, currentCurrency);
    }

    public void Refresh()
    {
        attractionPoints.RefreshAmount(GameState.instance.attractionPoints);
        buildMenuButton.disabled = isAnyMenuOpen;
    }
}