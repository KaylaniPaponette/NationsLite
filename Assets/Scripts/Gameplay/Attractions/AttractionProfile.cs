using System;
using UnityEngine;

[Serializable]
public struct CurrencyAmount
{
    public Currency type;
    public int amount;
} 

[CreateAssetMenu(fileName = "AttractionProfile", menuName = "SP3D/AttractionProfile")]
public class AttractionProfile : ScriptableObject
{
    public int sortId;

    public string title;
    public string description;
    public GameObject prefab;
    public Sprite icon;
    public Vector2Int placementPosition;
    public CurrencyAmount rewardPerCycle;
    public GameTimeSpan cycleTime;
    public int maxLevel = 3;

    // Returns the reward amount for a given level
    public int GetRewardForLevel(int level)
    {
        // Increase reward amount per level, level 1 = base, level 2 = base * 2, level 3 = base * 3
        return rewardPerCycle.amount * level;
    }

    public GameTimeSpan GetCycleTimeForLevel(int level)
    {
        // Increase cycle time per level, level 1 = base, level 2 = base * 2, level 3 = base * 3
        return cycleTime * level; 
    }
}
