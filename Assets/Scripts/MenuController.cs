using UnityEngine;

/// <summary>
/// Controls the Main Menu -> Team Members -> Play flow, and opens social/repo links.
/// Attach this to an empty GameObject (e.g. "MenuController") in your scene.
/// </summary>
public class MenuController : MonoBehaviour
{
    [Header("Panels")]
    [Tooltip("Full-screen panel with the UNITY DEEP RL background + Play/Team Members/GitHub buttons")]
    public GameObject mainMenuPanel;

    [Tooltip("Full-screen panel with the Team Members background + per-person GitHub/LinkedIn buttons")]
    public GameObject teamMembersPanel;

    [Header("Optional")]
    [Tooltip("Any camera/UI scripts you want disabled while a menu panel is open (e.g. the orbit camera). Leave empty to skip.")]
    public MonoBehaviour[] disableWhileMenuOpen;

    void Start()
    {
        ShowMainMenu();
    }

    /// <summary>Called by the "Team Members" button on the Main Menu.</summary>
    public void ShowTeamMembers()
    {
        mainMenuPanel.SetActive(false);
        teamMembersPanel.SetActive(true);
    }

    /// <summary>Called by a Back button on the Team Members panel.</summary>
    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        teamMembersPanel.SetActive(false);
        SetInteractionEnabled(false);
    }

    /// <summary>Called by the "Play" button. Hides all menu UI and reveals the arena.</summary>
    public void PlayGame()
    {
        mainMenuPanel.SetActive(false);
        teamMembersPanel.SetActive(false);
        SetInteractionEnabled(true);
    }

    /// <summary>
    /// Called by any GitHub/LinkedIn button. Set the target URL in the button's
    /// OnClick() parameter field in the Inspector (each button passes its own URL string).
    /// </summary>
    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }

    private void SetInteractionEnabled(bool enabled)
    {
        if (disableWhileMenuOpen == null) return;
        foreach (var script in disableWhileMenuOpen)
        {
            if (script != null) script.enabled = enabled;
        }
    }
}
