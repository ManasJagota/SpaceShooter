using UnityEngine;
using UnityEngine.UI;
using MjCreates.Events;

/// <summary>
/// Shows the player's score. Listens for ScoreChangedEvent instead of being polled/scanned, so it
/// stays decoupled from the GameManager.
/// </summary>
public class ScoreDisplay : UIelement
{
    [Tooltip("The text UI to use for display")]
    public Text displayText = null;

    // The last score written to the label; lets us skip rebuilding the string when unchanged.
    private int m_LastDisplayedScore = int.MinValue;

    /// <summary>
    /// Description:
    /// Subscribes to score changes while this display is enabled.
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    private void OnEnable()
    {
        EventManager.Subscribe<ScoreChangedEvent>(OnScoreChanged);
    }

    /// <summary>
    /// Description:
    /// Stops listening when disabled.
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    private void OnDisable()
    {
        EventManager.Unsubscribe<ScoreChangedEvent>(OnScoreChanged);
    }

    /// <summary>
    /// Description:
    /// Handles a score change by updating the label.
    /// Inputs:
    /// ScoreChangedEvent a_event (carries the new score)
    /// Returns:
    /// void (no return)
    /// </summary>
    private void OnScoreChanged(ScoreChangedEvent a_event)
    {
        if (displayText != null && a_event.Score != m_LastDisplayedScore)
        {
            m_LastDisplayedScore = a_event.Score;
            displayText.text = "Score: " + a_event.Score.ToString();
        }
    }
}
