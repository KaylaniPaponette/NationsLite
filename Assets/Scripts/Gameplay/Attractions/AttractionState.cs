using System;

public enum BuildPhase
{
    None,
    Processing,
    Waiting,
}

public class AttractionState
{
    public BuildPhase phase;
    public GameDateTime phaseSwitchTime;

    // Track the current level of the attraction
    public int _currentLevel = 1;

    public event Action<int> OnLevelChanged;

    public int currentLevel
    {
        get => _currentLevel;
        set
        {
            if (_currentLevel != value) // Only trigger the event if the level has actually changed
            {
                _currentLevel = value;
                OnLevelChanged?.Invoke(_currentLevel);
            }
        }
    }

    public AttractionState()
    {
        phase = BuildPhase.None;
    }
}