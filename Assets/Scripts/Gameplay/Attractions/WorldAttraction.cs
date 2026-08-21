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

    LevelBadgeViewController levelBadge; // Reference to the level badge view controller

    public void Initialize()
    {
        // Subscribe to level changes
        if (state != null)
        {
            state.OnLevelChanged += OnLevelChanged;
        }
    }

    private void OnDestroy()
    {
        if (state != null)
        {
            state.OnLevelChanged -= OnLevelChanged;
        }

        if (levelBadge != null && WorldUIManager.instance != null && Application.isPlaying)
        {
            WorldUIManager.instance.ReleaseWorldView(levelBadge);
            levelBadge = null;
        }
    }

    private void OnLevelChanged(int newLevel)
    {
        if (levelBadge != null)
        {
            levelBadge.UpdateLevelDisplay(newLevel);
        }
    }

    public void ShowProgress(GameDateTime targetTime, GameTimeSpan totalTime)
    {
        if (progress == null)
            progress = WorldUIManager.instance.CreateWorldView<AttractionStateProgress>("progressBarContainer", progressAnchor);
        
        progress.progressBarContainer.displayed = true;
        progress.Setup(targetTime, totalTime);

        // Ensure the level badge is created and displayed
        if (levelBadge == null)
        {
            // Create the level badge and anchor it to the progress bar
            levelBadge = WorldUIManager.instance.CreateWorldView<LevelBadgeViewController>("levelBadgeContainer", progressAnchor);
            levelBadge.Setup(profile.maxLevel);
            levelBadge.UpdateLevelDisplay(state.currentLevel);
        }
        else
        {
            levelBadge.levelBadgeContainer.displayed = true;
        }

    }

    public void HideProgress()
    {
        if (progress != null)
        progress.progressBarContainer.displayed = false;

        // Keep the level badge visible during the waiting phase
        if (levelBadge != null)
            levelBadge.levelBadgeContainer.displayed = true; // Keep visible during waiting phase
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
