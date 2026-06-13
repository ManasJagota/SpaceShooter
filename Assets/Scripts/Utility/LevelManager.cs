using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Lives in the single shared gameplay scene (Game.unity). Owns the list of level
/// prefabs and swaps the active level by destroying the current prefab instance and
/// instantiating the next one - no scene reload. Replaces the old approach where
/// every level was its own scene loaded through SceneManager.LoadScene.
/// </summary>
public class LevelManager : MonoBehaviour
{
    // Global reference so UI buttons / GameManager can drive level flow
    private static LevelManager s_Instance = null;
    public static LevelManager Instance => s_Instance;

    // Set by the level-select menu before loading the Game scene. -1 means "use the
    // serialized start index". Static so it survives the scene load into Game.unity.
    private static int s_RequestedStartLevel = -1;

    // PlayerPrefs key storing the highest level index the player has reached.
    public const string c_UnlockedLevelKey = "unlockedLevel";

    [SerializeField, Tooltip("The level prefabs in play order. Each prefab root must have a Level component.")]
    private List<GameObject> m_LevelPrefabs = new List<GameObject>();

    [SerializeField, Tooltip("The index of the level to load first when the scene starts (used when no level was requested from a menu)")]
    private int m_StartLevelIndex = 0;

    [SerializeField, Tooltip("Scene to return to once all levels are cleared (leave blank to just reload the last level)")]
    private string m_SceneAfterLastLevel = "MainMenu";

    // The currently instantiated level prefab instance
    private GameObject m_CurrentInstance = null;

    // The index of the level currently loaded
    private int m_CurrentLevelIndex = 0;

    public int CurrentLevelIndex => m_CurrentLevelIndex;
    public int LevelCount => m_LevelPrefabs.Count;

    /// <summary>
    /// Description:
    /// Records which level a menu wants to start at, before the Game scene is loaded.
    /// Input:
    /// int l_index (the level index to start on)
    /// Return:
    /// void (no return)
    /// </summary>
    /// <param name="a_index">The level index the menu picked</param>
    public static void SetRequestedStartLevel(int a_index)
    {
        s_RequestedStartLevel = a_index;
    }

    /// <summary>
    /// Standard Unity function called when the script is loaded, before Start.
    /// Sets up the global reference.
    /// </summary>
    private void Awake()
    {
        if (s_Instance == null)
        {
            s_Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Standard Unity function called once before the first Update.
    /// Loads the requested start level (from a menu) or the serialized default.
    /// </summary>
    private void Start()
    {
        // The managers are persistent (created in MainMenu); the player lives in this
        // scene, so wire it into the persistent GameManager before loading a level.
        if (GameManager.instance != null && GameManager.instance.player == null)
        {
            GameObject l_player = GameObject.FindWithTag("Player");
            if (l_player != null)
            {
                GameManager.instance.player = l_player;
            }
        }

        int l_startIndex = (s_RequestedStartLevel >= 0) ? s_RequestedStartLevel : m_StartLevelIndex;
        s_RequestedStartLevel = -1;
        l_startIndex = Mathf.Clamp(l_startIndex, 0, m_LevelPrefabs.Count - 1);
        LoadLevel(l_startIndex);
    }

    /// <summary>
    /// Description:
    /// Destroys the current level instance (if any) and instantiates the level prefab
    /// at the given index, then configures the shared GameManager for it.
    /// Input:
    /// int l_index (index into the level prefab list to load)
    /// Return:
    /// void (no return)
    /// </summary>
    /// <param name="a_index">Index into the level prefab list to load</param>
    public void LoadLevel(int a_index)
    {
        if (a_index < 0 || a_index >= m_LevelPrefabs.Count || m_LevelPrefabs[a_index] == null)
        {
            Debug.LogWarning("LevelManager: no valid level prefab at index " + a_index);
            return;
        }

        if (m_CurrentInstance != null)
        {
            Destroy(m_CurrentInstance);
        }

        m_CurrentLevelIndex = a_index;

        // Record progress: remember the furthest level the player has reached.
        int l_unlocked = Mathf.Max(PlayerPrefs.GetInt(c_UnlockedLevelKey, 0), a_index);
        PlayerPrefs.SetInt(c_UnlockedLevelKey, l_unlocked);

        m_CurrentInstance = Instantiate(m_LevelPrefabs[a_index]);

        Level l_level = m_CurrentInstance.GetComponent<Level>();
        if (GameManager.instance != null)
        {
            GameManager.instance.ConfigureForLevel(l_level);
        }

        Time.timeScale = 1;
    }

    /// <summary>
    /// Description:
    /// Advances to the next level. If there are no more levels, goes to the
    /// configured scene (e.g. the main menu).
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    public void LoadNextLevel()
    {
        int l_next = m_CurrentLevelIndex + 1;
        if (l_next < m_LevelPrefabs.Count)
        {
            LoadLevel(l_next);
        }
        else
        {
            Time.timeScale = 1;
            if (!string.IsNullOrEmpty(m_SceneAfterLastLevel))
            {
                SceneManager.LoadScene(m_SceneAfterLastLevel);
            }
        }
    }

    /// <summary>
    /// Description:
    /// Reloads the current level (used by the game over / retry button).
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    public void ReloadLevel()
    {
        LoadLevel(m_CurrentLevelIndex);
    }

    /// <summary>
    /// Description:
    /// Loads the main menu scene (used by "back to menu" / quit-to-menu buttons,
    /// replacing the old LevelLoadButton).
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        if (!string.IsNullOrEmpty(m_SceneAfterLastLevel))
        {
            SceneManager.LoadScene(m_SceneAfterLastLevel);
        }
    }
}
