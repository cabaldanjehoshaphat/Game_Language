using TMPro;
using UnityEngine;

/// <summary>
/// ProfileDisplay
/// Pushes the current player's per-game progress from PlayerDatabase onto the Profile
/// screen's TMP text elements ("... data score - Text (TMP)" objects in the
/// "2 Profile Icon" scene). Assign each Text (TMP) reference in the Inspector.
///
/// To display a future game's progress here: add a new TMP_Text field below, then add
/// its matching line in RefreshDisplay().
/// </summary>
public class ProfileDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text wordleScoreText;
    [SerializeField] private TMP_Text crosswordScoreText;
    [SerializeField] private TMP_Text wordSearchScoreText;

    private void Start()
    {
        RefreshDisplay();
    }

    /// <summary>Re-reads PlayerDatabase and writes each value into its Text (TMP) target.</summary>
    public void RefreshDisplay()
    {
        if (PlayerDatabase.Instance == null)
        {
            Debug.LogWarning("ProfileDisplay: no PlayerDatabase found in the scene.");
            return;
        }

        var db = PlayerDatabase.Instance;

        if (wordleScoreText != null) wordleScoreText.text = db.WordleProgress.ToString();
        if (crosswordScoreText != null) crosswordScoreText.text = db.CrosswordProgress.ToString();
        if (wordSearchScoreText != null) wordSearchScoreText.text = db.WordSearchProgress.ToString();
    }
}
