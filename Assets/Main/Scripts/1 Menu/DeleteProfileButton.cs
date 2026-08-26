using UnityEngine;

/// <summary>
/// DeleteProfileButton
/// Wired to the Profile screen's delete_button. Placeholder for future use (e.g. resetting
/// PlayerData / deleting the save file) — intentionally left unimplemented since the exact
/// delete behavior (reset progress vs. delete save vs. something else) hasn't been decided yet.
/// </summary>
public class DeleteProfileButton : MonoBehaviour
{
    /// <summary>Hooked to delete_button's OnClick. TODO: implement delete/reset behavior.</summary>
    public void OnDeleteButtonPressed()
    {
        Debug.Log("DeleteProfileButton: not implemented yet.");
    }
}
