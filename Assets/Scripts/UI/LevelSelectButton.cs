using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Used on the main menu's level-select buttons. Instead of loading a separate level
/// scene, it records which level index the player picked and loads the single shared
/// Game scene; LevelManager then starts on that index.
/// </summary>
public class LevelSelectButton : MonoBehaviour
{
    [SerializeField, Tooltip("The name of the shared gameplay scene that holds the LevelManager")]
    private string m_GameSceneName = "Game";

    [SerializeField, Tooltip("The name of the main menu scene")]
    private string m_MainMenuSceneName = "MainMenu";

    /// <summary>
    /// Description:
    /// Stores the chosen level index and loads the shared Game scene.
    /// Hook this up to a button's OnClick with the desired level index as the argument.
    /// Input:
    /// int l_levelIndex (the index of the level to start on)
    /// Return:
    /// void (no return)
    /// </summary>
    /// <param name="a_levelIndex">The index of the level to start on</param>
    public void LoadGameAtLevel(int a_levelIndex)
    {
        LevelManager.SetRequestedStartLevel(a_levelIndex);
        Time.timeScale = 1;
        SceneManager.LoadScene(m_GameSceneName);
    }

    /// <summary>
    /// Description:
    /// Loads the furthest level the player has reached (a "continue" button).
    /// Reads the unlocked level from PlayerPrefs; LevelManager clamps it on start
    /// if the saved value is out of range.
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    public void LoadNextLevel()
    {
        int l_next = PlayerPrefs.GetInt(LevelManager.c_UnlockedLevelKey, 0);
        LoadGameAtLevel(l_next);
    }

    /// <summary>
    /// Description:
    /// Loads the main menu scene. Works from any scene since this component lives on the
    /// persistent managers object (use for back-to-menu / quit-to-menu buttons).
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(m_MainMenuSceneName);
    }

    /// <summary>
    /// Description:
    /// Advances to the next level (victory screen "Next" button). Delegates to the
    /// scene's LevelManager. Lives here because the in-game UI is persistent and cannot
    /// hold a direct cross-scene reference to the LevelManager in the Game scene.
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    public void NextLevel()
    {
        LevelManager.Instance.LoadNextLevel();
    }

    /// <summary>
    /// Description:
    /// Reloads the current level (game over "Retry" button). Delegates to the scene's
    /// LevelManager.
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    public void RestartLevel()
    {
        LevelManager.Instance.ReloadLevel();
    }
}
