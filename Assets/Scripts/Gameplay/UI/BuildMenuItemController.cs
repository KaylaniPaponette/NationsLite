using UnityEngine;

[UIDocument(uiDocName: "BuildMenuItem", stylesheetName: "BuildMenuStyles")]
public class BuildMenuItemController : UIViewController
{
    public UIButton selectionButton;
    public UIImage thumbnail;
    public UILabel title;
    public UILabel description;
    public UIContainer currencyContainer;
    public UILabel rewardCycle;
    /*public UILabel upgradeInfoLabel; // Label to display upgrade information*/ //REPLACED WITH UPGRADE CURRENCY VIEW AND LABELS

    public UIContainer upgradeCurrencyContainer; // Container for the upgrade currency view
    public UILabel upgradeTimeLabel; // Label to display upgrade time information
    public CurrencyViewController upgradeCurrencyView; // Child controller for the upgrade currency view

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
        /*upgradeInfoLabel = view.Find<UILabel>(nameof(upgradeInfoLabel));// Find the upgrade info label */ //REPLACED WITH UPGRADE CURRENCY VIEW AND LABELS
        upgradeCurrencyContainer = view.Find<UIContainer>(nameof(upgradeCurrencyContainer));
        upgradeTimeLabel = view.Find<UILabel>(nameof(upgradeTimeLabel));

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

        if (upgradeCurrencyContainer != null)
        {
            if (upgradeCurrencyView == null)
            {
                upgradeCurrencyView = CreateChild<CurrencyViewController>(upgradeCurrencyContainer);
            }

            upgradeCurrencyView.Setup(profile.rewardPerCycle.type, profile.rewardPerCycle.amount);
            upgradeCurrencyView.amountText.text = $"+ {profile.rewardPerCycle.amount}"; // Display the amount with a "+" sign to indicate it's an upgrade
        }

        if (upgradeTimeLabel != null)
        {
            string timePerLevel = profile.cycleTime.ToDisplayString();
            upgradeTimeLabel.text = $" | + {timePerLevel} per level";
        }

        /*//REPLACED WITH UPGRADE CURRENCY VIEW AND LABELS
        // Display upgrade information if the label exists
        if (upgradeInfoLabel != null)
        {
            int currencyPerLevel = profile.rewardPerCycle.amount;
            string timePerLevel = profile.cycleTime.ToDisplayString();

            upgradeInfoLabel.text = $"(+{currencyPerLevel} currency / +{timePerLevel} per level)";
        }*/
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