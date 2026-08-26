using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GridGenerator
/// Spawns a grid of UI cells (rows x columns) under a RectTransform parent (e.g. a Canvas).
/// Row/column counts and cell layout (size, spacing) come from an assigned GridSettings
/// ScriptableObject asset. In the Editor, this component subscribes to that asset's Changed
/// event so the grid regenerates immediately whenever rows/columns/size/spacing are edited,
/// without needing to enter Play mode. The cell prefab and grid parent stay here since
/// they're scene-specific references.
/// </summary>
public class GridGenerator : MonoBehaviour
{
    [Header("Layout Data")]
    [Tooltip("ScriptableObject asset holding rows, columns, cell size and spacing.")]
    public GridSettings settings;

    [Header("Cell Setup")]
    [Tooltip("Prefab used for each grid cell. Should have a RectTransform (UI element).")]
    public GameObject cellPrefab;

    [Tooltip("Parent RectTransform the cells are placed under. Defaults to this object's RectTransform.")]
    public RectTransform gridParent;

    [Header("Behaviour")]
    [Tooltip("If true, the grid is (re)generated automatically when the scene starts playing.")]
    public bool generateOnStart = true;

#if UNITY_EDITOR
    private GridSettings subscribedSettings;
#endif

    private void Start()
    {
        if (generateOnStart)
        {
            Generate();
        }
    }

    /// <summary>
    /// Clears any previously generated cells and instantiates a fresh rows x columns grid
    /// using the dimensions and layout defined in the assigned GridSettings asset.
    /// </summary>
[ContextMenu("Generate Grid")]
    public void Generate()
    {
        RectTransform parent = gridParent != null ? gridParent : GetComponent<RectTransform>();
        if (parent == null || cellPrefab == null || settings == null)
        {
            Debug.LogWarning("GridGenerator: missing gridParent/RectTransform, cellPrefab, or settings.");
            return;
        }

        Clear(parent);

        int rows = settings.rows;
        int columns = settings.columns;
        Vector2 cellSize = settings.cellSize;
        Vector2 spacing = settings.spacing;

        float startX = -(columns - 1) * (cellSize.x + spacing.x) * 0.5f;
        float startY = (rows - 1) * (cellSize.y + spacing.y) * 0.5f;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                GameObject cell = Instantiate(cellPrefab, parent);
                cell.name = $"Cell_{row}_{col}";

                RectTransform cellRect = cell.GetComponent<RectTransform>();
                if (cellRect == null)
                {
                    cellRect = cell.AddComponent<RectTransform>();
                }

                cellRect.sizeDelta = cellSize;
                cellRect.anchoredPosition = new Vector2(
                    startX + col * (cellSize.x + spacing.x),
                    startY - row * (cellSize.y + spacing.y));
            }
        }
    }

    /// <summary>
    /// Removes all previously generated child cells from the grid parent.
    /// </summary>
    private void Clear(RectTransform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
            }
            else
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        SubscribeToSettings();
    }

    private void OnDisable()
    {
        UnsubscribeFromSettings();
    }

    private void OnValidate()
    {
        // Rebind if the "settings" reference itself was swapped in the Inspector.
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this == null) return;
            SubscribeToSettings();
        };
    }

private void SubscribeToSettings()
    {
        // Always resubscribe: after a domain reload, C# event subscriber lists are cleared
        // even though this field's reference survives, so an equality guard would skip re-binding.
        UnsubscribeFromSettings();
        subscribedSettings = settings;

        if (subscribedSettings != null)
        {
            subscribedSettings.Changed += HandleSettingsChanged;
        }
    }

    private void UnsubscribeFromSettings()
    {
        if (subscribedSettings != null)
        {
            subscribedSettings.Changed -= HandleSettingsChanged;
        }
        subscribedSettings = null;
    }

private void HandleSettingsChanged()
    {
        // Defer: Unity disallows destroying/reparenting objects directly from within an
        // OnValidate callback (logs "SendMessage cannot be called during ... OnValidate").
        UnityEditor.EditorApplication.delayCall += DelayedRegenerate;
    }

private void DelayedRegenerate()
    {
        UnityEditor.EditorApplication.delayCall -= DelayedRegenerate;
        if (this != null)
        {
            Generate();
        }
    }
#endif
}
