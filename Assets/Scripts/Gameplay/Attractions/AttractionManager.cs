using System.Collections.Generic;

public class AttractionManager : Singleton<AttractionManager>
{
    List<AttractionProfile> _profiles = new List<AttractionProfile>();
    public List<AttractionProfile> profiles => _profiles;

    List<WorldAttraction> placedAttractions = new List<WorldAttraction>();
    
    public void LoadProfiles()
    {
        _profiles = AssetManager.instance.LoadAssetsOfType<AttractionProfile>();
        _profiles.Sort((a, b) => a.sortId.CompareTo(b.sortId));
    }

    public void AddAttraction(WorldAttraction worldAttraction, AttractionProfile profile)
    {
        worldAttraction.profile = profile;

        // Set state to Processing phase
        worldAttraction.state = new AttractionState
        {
            phase = BuildPhase.Processing,
            phaseSwitchTime = GameState.instance.dateTime + profile.cycleTime
        };

        placedAttractions.Add(worldAttraction);

        // Show the progress bar over the attraction
        worldAttraction.ShowProgress(worldAttraction.state.phaseSwitchTime);
    }

    public bool IsAttractionPlaced(AttractionProfile profile)
    {
        // Check if any placed attraction matches this profile
        return placedAttractions.Exists(a => a.profile == profile);
    }

    public void OnUpdate()
    {
        foreach (var attraction in placedAttractions)
        {
            if (attraction == null || attraction.state == null)
                continue;

            // Transition from Processing -> Waiting when time elapses
            if (attraction.state.phase == BuildPhase.Processing &&
                GameState.instance.dateTime >= attraction.state.phaseSwitchTime)
            {
                attraction.state.phase = BuildPhase.Waiting;
                attraction.HideProgress();
                attraction.ShowReadyState();
            }
        }
    }
}
