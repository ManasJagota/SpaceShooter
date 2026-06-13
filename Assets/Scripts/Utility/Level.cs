using UnityEngine;

/// <summary>
/// Configuration component placed on the root of a level prefab.
/// Holds the values that used to live on the per-scene GameManager and that
/// differ from one level to the next. The LevelManager reads these on load and
/// pushes them into the shared GameManager via GameManager.ConfigureForLevel.
/// </summary>
public class Level : MonoBehaviour
{
    [SerializeField, Tooltip("Optional human readable name for this level (debug / UI)")]
    private string m_DisplayName = "";

    [SerializeField, Tooltip("The number of enemies that must be defeated to clear this level")]
    private int m_EnemiesToDefeat = 10;

    [SerializeField, Tooltip("The effect to spawn when this level is cleared")]
    private GameObject m_VictoryEffect = null;

    public string DisplayName => m_DisplayName;
    public int EnemiesToDefeat => m_EnemiesToDefeat;
    public GameObject VictoryEffect => m_VictoryEffect;
}
