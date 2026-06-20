/// <summary>
/// Central home for string keys and tags used across the game. Keeping them here avoids
/// scattered magic strings that are easy to mistype and hard to refactor.
/// </summary>
public static class GameConstants
{
    // PlayerPrefs keys
    public const string c_HighScoreKey = "highscore";
    public const string c_ScoreKey = "score";

    // Tags
    public const string c_PlayerTag = "Player";
}
