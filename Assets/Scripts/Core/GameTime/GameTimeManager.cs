using UnityEngine;

public enum GameTimeSpeed
{
    Debug_Pause,
    Slow,
    Relaxed,
    Normal,
    Fast,
    Challenging,
    Debug_Fast,
}

public class GameTimeManager : Singleton<GameTimeManager>
{
    // temporary. Move to GameState when persistence is set up.
    public GameTimeSpeed gameSpeed = GameTimeSpeed.Normal;
    
    public GameDateTime dateTime
    {
        get => GameState.instance.dateTime;
        set => GameState.instance.dateTime = value;
    }

    public bool timePaused;

    int pauseMultiplier => 0;
    int slowMultiplier => CalculateSpeedMultiplier(realTimeSecondsPerGameDay: 345600);
    int relaxedMultiplier => CalculateSpeedMultiplier(realTimeSecondsPerGameDay: 172800);
    int normalMultiplier => CalculateSpeedMultiplier(realTimeSecondsPerGameDay: 86400);
    int fastMultiplier => CalculateSpeedMultiplier(realTimeSecondsPerGameDay: 43200);
    int challengingMultiplier => CalculateSpeedMultiplier(realTimeSecondsPerGameDay: 21600);
    int debugFastMultiplier => CalculateSpeedMultiplier(realTimeSecondsPerGameDay: 10800);

    int timeStepMultiplier
    {
        get
        {
            switch (gameSpeed)
            {
                case GameTimeSpeed.Debug_Pause:
                    return pauseMultiplier;
                case GameTimeSpeed.Slow:
                    return slowMultiplier;
                case GameTimeSpeed.Relaxed:
                    return relaxedMultiplier;
                case GameTimeSpeed.Normal:
                    return normalMultiplier;
                case GameTimeSpeed.Fast:
                    return fastMultiplier;
                case GameTimeSpeed.Challenging:
                    return challengingMultiplier;
                case GameTimeSpeed.Debug_Fast:
                    return debugFastMultiplier;
                default:
                    Debug.LogWarning("GameSpeed not set properly. Debug pausing game.");
                    return pauseMultiplier;
            }
        }
    }

    int CalculateSpeedMultiplier(int realTimeSecondsPerGameDay)
    {
        // Avoid dividing by zero
        if (realTimeSecondsPerGameDay == 0)
            return 0;

        return GameTime.kMillisecondsInDay / (realTimeSecondsPerGameDay * 1000);
    }

    public void OnUpdate()
    {
        if (timePaused)
            return;
            
        dateTime += new GameTimeSpan((int)(Time.deltaTime * 1000 * timeStepMultiplier));
    }    
}
