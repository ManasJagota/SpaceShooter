using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class stores relevant information about a page of UI
/// </summary>
public class UIPage : MonoBehaviour
{
    [Tooltip("The default UI to have selected when opening this page")]
    public GameObject defaultSelected;

    /// <summary>
    /// Description:
    /// Sets the selected UI selectable to the default defined by this UIPage
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    public void SetSelectedUIToDefault()
    {
        if (defaultSelected != null)
        {
            UIManager l_uiManager = (GameManager.instance != null) ? GameManager.instance.uiManager : null;
            if (l_uiManager != null && l_uiManager.eventSystem != null)
            {
                l_uiManager.eventSystem.SetSelectedGameObject(null);
                l_uiManager.eventSystem.SetSelectedGameObject(defaultSelected);
            }
        }
        
    }
}
