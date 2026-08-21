using UnityEngine;
public class GameState : Singleton<GameState>
{
    public int attractionPoints { get; private set; }

    [SerializeField] GameDateTime _dateTime;
    public GameDateTime dateTime
    {
        get => _dateTime;
        set
        {
            var oldTime = _dateTime;
            _dateTime = value;
            if (oldTime.day != value.day)
                OnDayChanged();
        }
    }

    public void OnDayChanged()
    {
    }


    public void AddAttractionPoints(int points)
    {
        attractionPoints += points;
        UIManager.instance.rootViewController.RefreshHud();
    }
}