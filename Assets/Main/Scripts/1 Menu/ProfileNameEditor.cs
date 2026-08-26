using TMPro;
using UnityEngine;

/// <summary>
/// ProfileNameEditor
/// Wired to the Profile screen's edit_button. On Start, shows the player's current
/// name (from PlayerDatabase.PlayerNameOrId) in the "User Name" Text (TMP). Clicking
/// edit_button swaps that Text for an editable TMP_InputField pre-filled with the
/// current name; clicking it again (now acting as "confirm") writes the typed value
/// back into PlayerData.playerNameOrId via PlayerDatabase.PlayerNameOrId and restores
/// the Text display. One button does double duty as Edit/Confirm.
/// </summary>
public class ProfileNameEditor : MonoBehaviour
{
    [SerializeField] private TMP_Text userNameDisplayText;
    [SerializeField] private TMP_InputField nameInputField;

    private bool isEditing = false;

    private void Start()
    {
        if (nameInputField != null)
        {
            nameInputField.gameObject.SetActive(false);
        }
        RefreshDisplay();
    }

    /// <summary>Re-reads the saved player name and shows it in the display Text.</summary>
    public void RefreshDisplay()
    {
        if (PlayerDatabase.Instance == null || userNameDisplayText == null)
        {
            return;
        }

        userNameDisplayText.text = PlayerDatabase.Instance.PlayerNameOrId;
    }

    /// <summary>Hooked to edit_button's OnClick. Toggles between edit mode and confirm.</summary>
    public void OnEditButtonPressed()
    {
        if (userNameDisplayText == null || nameInputField == null)
        {
            Debug.LogWarning("ProfileNameEditor: missing userNameDisplayText or nameInputField reference.");
            return;
        }

        if (!isEditing)
        {
            nameInputField.text = userNameDisplayText.text;
            nameInputField.gameObject.SetActive(true);
            userNameDisplayText.gameObject.SetActive(false);
            nameInputField.Select();
            nameInputField.ActivateInputField();
            isEditing = true;
        }
        else
        {
            string newName = nameInputField.text.Trim();
            if (!string.IsNullOrEmpty(newName) && PlayerDatabase.Instance != null)
            {
                PlayerDatabase.Instance.PlayerNameOrId = newName;
            }

            RefreshDisplay();
            nameInputField.gameObject.SetActive(false);
            userNameDisplayText.gameObject.SetActive(true);
            isEditing = false;
        }
    }
}
