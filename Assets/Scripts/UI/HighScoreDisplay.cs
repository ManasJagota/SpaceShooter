using UnityEngine;
using UnityEngine.UI;
using MjCreates.Events;

/// <summary>
/// Shows the player's high score. Listens for HighScoreChangedEvent instead of being polled/scanned,
/// so it stays decoupled from the GameManager.
/// </summary>
public class HighScoreDisplay : UIelement
{
    [Tooltip("The text UI to use for display")]
    public Text displayText = null;

    // The last high score written to the label; lets us skip rebuilding the string when unchanged.
    private int m_LastDisplayedHighScore = int.MinValue;

    /// <summary>
    /// Description:
    /// Subscribes to high score changes while this display is enabled.
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    private void OnEnable()
    {
        EventManager.Subscribe<HighScoreChangedEvent>(OnHighScoreChanged);
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
        EventManager.Unsubscribe<HighScoreChangedEvent>(OnHighScoreChanged);
    }

    /// <summary>
    /// Description:
    /// Handles a high score change by updating the label.
    /// Inputs:
    /// HighScoreChangedEvent a_event (carries the new high score)
    /// Returns:
    /// void (no return)
    /// </summary>
    private void OnHighScoreChanged(HighScoreChangedEvent a_event)
    {
        if (displayText != null && a_event.HighScore != m_LastDisplayedHighScore)
        {
            m_LastDisplayedHighScore = a_event.HighScore;
            displayText.text = "High: " + a_event.HighScore.ToString();
        }
    }
}
