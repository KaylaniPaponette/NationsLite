using UnityEngine;

[UIDocument(uiDocName:"LevelBadge", stylesheetName:"LevelBadgeStyles")]
public class LevelBadgeViewController : UIViewController
{
    public UIContainer levelBadgeContainer;
    public UILabel levelText;
    private int _maxLevel = 3; // Default max level, can be set via Setup method

    public override void Init()
    {
        // Initialize the UI components, cache references to the UI elements for later use
        levelBadgeContainer = view.Find<UIContainer>(nameof(levelBadgeContainer));
        levelText = view.Find<UILabel>("levelText");
    }
    public void Setup(int maxLevel)
    {
        _maxLevel = maxLevel;
    }

    public void UpdateLevelDisplay(int newLevel)
    {
        if (levelText == null)
        {
            Debug.LogWarning("LevelBadgeViewController: levelText is not assigned.");
            return;
        }
        // Check if at max level and update the text accordingly
        if (newLevel >= _maxLevel)
        {
            levelText.text = "Max Level";
        }
        else
        {
            levelText.text = $"Lv. {newLevel}";
        }
        Debug.Log($"LevelBadgeViewController: Updated level display to {newLevel}");
    }
}
