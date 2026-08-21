using UnityEngine;

public class LevelBadgeViewController : UIViewController
{
    public UILabel levelText;

    public override void Init()
    {
        levelText = view.Find<UILabel>("levelText");
    }

    public void UpdateLevelDisplay(int newLevel)
    {
        if (levelText == null)
        {
            Debug.LogWarning("LevelBadgeViewController: levelText is not assigned.");
            return;
        }

        levelText.text = $"Lv. {newLevel}";
        Debug.Log($"LevelBadgeViewController: Updated level display to {newLevel}");
    }

}
