using MjCreates.Events;

/// <summary>
/// Game-specific event payloads carried by the reusable MjCreates EventManager. They live in the
/// game (not the package) so the package stays project-agnostic. All are small structs so
/// publishing them does not allocate.
/// </summary>

/// <summary>Raised whenever the player's score changes. Carries the new total.</summary>
public struct ScoreChangedEvent : IEvent
{
    public int Score;
    public ScoreChangedEvent(int a_score)
    {
        Score = a_score;
    }
}

/// <summary>Raised whenever the high score changes. Carries the new high score.</summary>
public struct HighScoreChangedEvent : IEvent
{
    public int HighScore;
    public HighScoreChangedEvent(int a_highScore)
    {
        HighScore = a_highScore;
    }
}

/// <summary>Raised by an enemy as it is defeated. Carries the score it is worth.</summary>
public struct EnemyDefeatedEvent : IEvent
{
    public int ScoreValue;
    public EnemyDefeatedEvent(int a_scoreValue)
    {
        ScoreValue = a_scoreValue;
    }
}

/// <summary>Raised when the player dies. The GameManager reacts by ending the game.</summary>
public struct PlayerDiedEvent : IEvent
{
}

/// <summary>Raised when the player takes damage. Carries the player's current and max health.</summary>
public struct PlayerDamagedEvent : IEvent
{
    public int CurrentHealth;
    public int MaxHealth;
    public PlayerDamagedEvent(int a_currentHealth, int a_maxHealth)
    {
        CurrentHealth = a_currentHealth;
        MaxHealth = a_maxHealth;
    }
}

/// <summary>Raised when the game is lost (after the game-over flow starts).</summary>
public struct GameOverEvent : IEvent
{
}

/// <summary>Raised when the current level is cleared (victory condition met).</summary>
public struct LevelClearedEvent : IEvent
{
}

/// <summary>Raised when a level finishes loading and the GameManager has configured for it.</summary>
public struct LevelLoadedEvent : IEvent
{
}
