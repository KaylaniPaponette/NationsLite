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
            phaseSwitchTime = GameState.instance.dateTime + profile.cycleTime,
            currentLevel = 1 // Ensure the base level is set when placed!
        };

        placedAttractions.Add(worldAttraction);
        worldAttraction.Initialize(); // Initialize the attraction and level badge view controller
        worldAttraction.ShowProgress(worldAttraction.state.phaseSwitchTime, profile.cycleTime); // Show the progress bar over the attraction

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

            // Transition from processing to waiting phase
            if (attraction.state.phase == BuildPhase.Processing &&
                GameState.instance.dateTime >= attraction.state.phaseSwitchTime)
            {
                attraction.state.phase = BuildPhase.Waiting;
                attraction.HideProgress();
                attraction.ShowReadyState();

                //deleted LevelBadgeManager
                //// Track the attraction for level badge updates, create the badge if it doesn't exist yet
                //if (LevelBadgeManager.instance != null)
                //{
                //    LevelBadgeManager.instance.TrackAttraction(attraction);
                //}
            }
        }
    }
}
