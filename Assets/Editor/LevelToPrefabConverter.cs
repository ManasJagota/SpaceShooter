#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// TEMPORARY editor tool. Converts each per-level scene (Level1/2/3) into a single
/// level prefab. Everything in the scene EXCEPT the shared managers (GameManager,
/// UIManager, InputManager, Camera, Canvas, EventSystem, Player) is moved into the
/// prefab - so spawner parents keep their hierarchy no matter how they are named.
/// A Level component carries the values that used to live on that scene's GameManager.
///
/// Run via menu: Tools > Convert Levels To Prefabs.
/// The source scenes are NOT saved, so they remain as backups.
/// DELETE this file once the prefabs have been generated.
/// </summary>
public static class LevelToPrefabConverter
{
    // Source scene -> output prefab name
    private static readonly (string scenePath, string prefabName)[] s_Levels =
    {
        ("Assets/Scenes/Level1.unity", "Level1"),
        ("Assets/Scenes/Level2.unity", "Level2"),
        ("Assets/Scenes/Level3.unity", "Level3"),
    };

    private const string c_OutputFolder = "Assets/Prefabs/Levels";

    [MenuItem("Tools/Convert Levels To Prefabs")]
    public static void Convert()
    {
        if (!AssetDatabase.IsValidFolder(c_OutputFolder))
        {
            AssetDatabase.CreateFolder("Assets/Prefabs", "Levels");
        }

        foreach (var l_entry in s_Levels)
        {
            ConvertOne(l_entry.scenePath, l_entry.prefabName);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("LevelToPrefabConverter: done. Check " + c_OutputFolder +
                  ". You can delete Assets/Editor/LevelToPrefabConverter.cs now.");
    }

    /// <summary>
    /// Description:
    /// Returns true if a scene root is shared infrastructure that must stay in the
    /// Game scene (managers / camera / UI / player) rather than go into the prefab.
    /// Input:
    /// GameObject a_root (a scene root object)
    /// Return:
    /// bool (true if the root should remain in the scene)
    /// </summary>
    /// <param name="a_root">A scene root object</param>
    private static bool IsSharedRoot(GameObject a_root)
    {
        if (a_root.CompareTag("Player") || a_root.CompareTag("MainCamera"))
        {
            return true;
        }
        if (a_root.GetComponentInChildren<GameManager>(true) != null) return true;
        if (a_root.GetComponentInChildren<UIManager>(true) != null) return true;
        if (a_root.GetComponentInChildren<InputManager>(true) != null) return true;
        if (a_root.GetComponentInChildren<Camera>(true) != null) return true;
        if (a_root.GetComponentInChildren<Canvas>(true) != null) return true;
        if (a_root.GetComponentInChildren<EventSystem>(true) != null) return true;
        return false;
    }

    /// <summary>
    /// Description:
    /// Extracts all non-shared roots from one scene into a single prefab and stamps a
    /// Level component with that scene's GameManager values.
    /// Input:
    /// string a_scenePath, string a_prefabName
    /// Return:
    /// void (no return)
    /// </summary>
    /// <param name="a_scenePath">Path of the source level scene</param>
    /// <param name="a_prefabName">Name to give the generated prefab</param>
    private static void ConvertOne(string a_scenePath, string a_prefabName)
    {
        Scene l_scene = EditorSceneManager.OpenScene(a_scenePath, OpenSceneMode.Single);

        // Pull the level-specific values off the scene's GameManager.
        int l_enemiesToDefeat = 10;
        GameObject l_victoryEffect = null;
        GameManager l_gm = Object.FindObjectOfType<GameManager>();
        if (l_gm != null)
        {
            l_enemiesToDefeat = l_gm.enemiesToDefeat;
            l_victoryEffect = l_gm.victoryEffect;
        }
        else
        {
            Debug.LogWarning("No GameManager found in " + a_scenePath + " - using defaults.");
        }

        // Build a single prefab root and reparent every non-shared root under it.
        GameObject l_root = new GameObject(a_prefabName);

        foreach (GameObject l_go in l_scene.GetRootGameObjects())
        {
            if (l_go == l_root) continue;
            if (IsSharedRoot(l_go)) continue;
            l_go.transform.SetParent(l_root.transform, true);
        }

        // Level fields are private+serialized, so write them through SerializedObject.
        Level l_level = l_root.AddComponent<Level>();
        SerializedObject l_so = new SerializedObject(l_level);
        l_so.FindProperty("m_DisplayName").stringValue = a_prefabName;
        l_so.FindProperty("m_EnemiesToDefeat").intValue = l_enemiesToDefeat;
        l_so.FindProperty("m_VictoryEffect").objectReferenceValue = l_victoryEffect;
        l_so.ApplyModifiedPropertiesWithoutUndo();

        string l_prefabPath = c_OutputFolder + "/" + a_prefabName + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(l_root, l_prefabPath);
        Debug.Log("Saved " + l_prefabPath + " (enemiesToDefeat=" + l_enemiesToDefeat +
                  ", roots moved into prefab)");

        // Do NOT save the scene - leave the originals untouched as backups.
    }
}
#endif
