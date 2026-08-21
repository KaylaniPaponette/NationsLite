using UnityEngine;

public class Currency : ScriptableObject
{
    public string title;
    public Sprite icon;
    public string toolTipText;

    public bool hasTooltipText => !string.IsNullOrEmpty(toolTipText);
}
