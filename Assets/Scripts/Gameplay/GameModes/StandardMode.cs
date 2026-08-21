using UnityEngine;

public class StandardMode : GameMode, IGameplayInputMapHandler
{

    public override void OnEnter()
    {
        UIManager.instance.CreateHUD();
    }

    public override void OnActivate()
    {
        base.OnActivate();
        InputManager.instance.PushInputHandler(this);
    }

    public void PlaceAttraction(AttractionProfile profile)
    {
        var instance = GameObjectExtensions.InstantiatePrefab(profile.prefab);
        instance.transform.position = profile.placementPosition.WithZ(0);

        var worldAttraction = instance.GetComponent<WorldAttraction>();
        AttractionManager.instance.AddAttraction(worldAttraction, profile);
    }

    public override void OnMousePress(MouseEventArgs e)
    {
        base.OnMousePress(e);
        if (e.button == MouseButton.Left && e.interactable == null)
        {
            if (TryRaycastAttraction(e.screenPosition, out var attraction))
            {
                if (attraction.state != null && attraction.state.phase == BuildPhase.Waiting)
                {
                    // Addd reward based on the current level of the attraction
                    int rewardAmount = attraction.profile.GetRewardForLevel(attraction.state.currentLevel);
                    GameState.instance.AddAttractionPoints(rewardAmount);
                    attraction.HideReadyState(showParticles: true);

                    if (attraction.state.currentLevel < attraction.profile.maxLevel)
                    {
                        attraction.state.currentLevel++;
                    }

                    attraction.state.phase = BuildPhase.Processing;

                    // Set the next cycle time based on the current level of the attraction
                    GameTimeSpan nextCycleTime = attraction.profile.GetCycleTimeForLevel(attraction.state.currentLevel);
                    attraction.state.phaseSwitchTime = GameState.instance.dateTime + nextCycleTime;
                    attraction.ShowProgress(attraction.state.phaseSwitchTime, nextCycleTime);
                }

                /*STANDARD BUILD PHASE - NO UPGRADE LEVELS*/
                //if (attraction.state.phase == BuildPhase.Waiting)
                //{
                //    attraction.HideReadyState(showParticles: true);
                //    attraction.state.phase = BuildPhase.Processing;
                //    attraction.state.phaseSwitchTime = GameState.instance.dateTime + attraction.profile.cycleTime;
                //    attraction.ShowProgress(attraction.state.phaseSwitchTime);
                //    GameState.instance.AddAttractionPoints(attraction.profile.rewardPerCycle.amount);
                //}
            }
        }
    }

    bool TryRaycastAttraction(Vector2 screenPosition, out WorldAttraction worldAttraction)
    {
        worldAttraction = null;

        var worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        var hits = Physics2D.RaycastAll(worldPosition, Vector2.zero);
        if (hits.Length == 0 || hits[0] == default)
            return false;

        worldAttraction = hits[0].collider.GetComponent<WorldAttraction>();
        return true;
    }

    public void OnPan(Vector2 direction)
    {

    }

    public void OnPanCancel()
    {

    }

    public void OnBuild()
    {

    }

    public void OnCancel()
    {

    }
}