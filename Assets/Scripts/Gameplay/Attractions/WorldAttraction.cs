using System;
using UnityEngine;

public class WorldAttraction : MonoBehaviour
{
    public Transform uiAnchor;
    public Transform progressAnchor;
    public ParticleSystem particles;

    public AttractionProfile profile { get; set; }
    public AttractionState state { get; set; }

    AttractionStateProgress progress;
    AttractionReadyIcon readyIcon;

    public UIContainer worldUIContainer;
    private LevelBadgeViewController levelBadge;

    public void Initialize()
    {
        // 1. Instantiate the UI controller for the badge
        if (levelBadge == null)
        {
            levelBadge = WorldUIManager.instance.CreateWorldView<LevelBadgeViewController>("levelBadgeContainer", progressAnchor);
        }

        levelBadge.UpdateLevelDisplay(state.currentLevel);
        state.OnLevelChanged += levelBadge.UpdateLevelDisplay;
    }
    private void OnDestroy()
    {
        if (state != null)
        {
            state.OnLevelChanged -= levelBadge.UpdateLevelDisplay;
        }
    }

    public void ShowProgress(GameDateTime targetTime)
    {
        if (progress == null)
            progress = WorldUIManager.instance.CreateWorldView<AttractionStateProgress>("progressBarContainer", progressAnchor);
        
        progress.progressBarContainer.displayed = true;
        progress.Setup(targetTime, profile.cycleTime);
    }

    public void HideProgress()
    {
        progress.progressBarContainer.displayed = false;
    }

    public void ShowReadyState()
    {
        if (readyIcon == null)
            readyIcon = WorldUIManager.instance.CreateWorldView<AttractionReadyIcon>("readyIcon", uiAnchor);
        
        readyIcon.readyIcon.displayed = true;
    }

    public void HideReadyState(bool showParticles = true)
    {
        readyIcon.readyIcon.displayed = false;
        if (showParticles)
            particles.Play();
    }


}
