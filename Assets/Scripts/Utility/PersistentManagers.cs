using UnityEngine;

/// <summary>
/// Placed on the single root object in MainMenu that holds the shared managers
/// (GameManager, UIManager, InputManager, EventSystem, the UI canvases and
/// LevelSelectButton). Marks that root as DontDestroyOnLoad so the whole manager
/// set survives the scene load into Game. If a second copy appears (e.g. on
/// returning to MainMenu) the duplicate root destroys itself.
/// </summary>
public class PersistentManagers : MonoBehaviour
{
    private static PersistentManagers s_Instance = null;
    public static PersistentManagers Instance => s_Instance;

    /// <summary>
    /// Standard Unity function called when the script is loaded, before Start.
    /// Establishes the singleton and persists the root, or destroys a duplicate.
    /// </summary>
    private void Awake()
    {
        if (s_Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        s_Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
