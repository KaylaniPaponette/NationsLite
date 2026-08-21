using UnityEngine;

[UIDocument(uiDocName:"BuildMenuItem", stylesheetName:"BuildMenuStyles")]
public class BuildMenuItemController : UIViewController
{
    public UIButton selectionButton;
    public UIImage thumbnail;
    public UILabel title;
    public UILabel description;
    public UIContainer currencyContainer;
    public UILabel rewardCycle;
    public UILabel upgradeInfoLabel; // Label to display upgrade information

    public CurrencyViewController currencyView;
    public AttractionProfile profile;

    public override void Init()
    {
        selectionButton = view.Find<UIButton>(nameof(selectionButton));
        thumbnail = view.Find<UIImage>(nameof(thumbnail));
        title = view.Find<UILabel>(nameof(title));
        description = view.Find<UILabel>(nameof(description));
        currencyContainer = view.Find<UIContainer>(nameof(currencyContainer));
        rewardCycle = view.Find<UILabel>(nameof(rewardCycle));
        upgradeInfoLabel = view.Find<UILabel>(nameof(upgradeInfoLabel));// Find the upgrade info label in the UI

        selectionButton.clicked += OnItemClicked;
    }

    public void Setup(AttractionProfile profile)
    {
        this.profile = profile;

        if (profile.icon != null)
            thumbnail.sprite = profile.icon;

        title.text = profile.title;
        description.text = profile.description;

        // Display cycle time
        rewardCycle.text = profile.cycleTime.ToDisplayString();

        // Spawn child currency controller inside the reward container
        if (currencyView == null)
            currencyView = CreateChild<CurrencyViewController>(currencyContainer);

        currencyView.Setup(profile.rewardPerCycle.type, profile.rewardPerCycle.amount);

        // Display upgrade information if the label exists
        if (upgradeInfoLabel != null)
        {
            int currencyPerLevel = profile.rewardPerCycle.amount;
            string timePerLevel = profile.cycleTime.ToDisplayString();

            upgradeInfoLabel.text = $"(+{currencyPerLevel} currency / +{timePerLevel} per level)";
        }
    }

    void OnItemClicked()
    {
        if (profile == null)
            return;

        // Places attraction per StandardMode
        if (GameModeManager.instance.activeMode is StandardMode standardMode)
        {
            standardMode.PlaceAttraction(profile);
        }

        // Close build menu
        UIMenuManager.instance.CloseAllMenus();
    }
}