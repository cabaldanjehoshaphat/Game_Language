using System;
using UnityEngine;

/// <summary>
/// GridSettings
/// Holds the row/column dimensions and cell layout data (size, spacing) for GridGenerator,
/// as a shared, re-usable ScriptableObject asset. Create one via
/// Assets > Create > Grid > Grid Settings, then assign it to a GridGenerator's "settings" field.
/// Raises Changed whenever a value is edited in the Inspector, so any GridGenerator using this
/// asset can regenerate its grid immediately instead of waiting for Play mode.
/// </summary>
[CreateAssetMenu(fileName = "GridSettings", menuName = "Grid/Grid Settings")]
public class GridSettings : ScriptableObject
{
    [Header("Grid Size")]
    [Tooltip("Number of rows to generate.")]
    [Min(1)] public int rows = 3;

    [Tooltip("Number of columns to generate.")]
    [Min(1)] public int columns = 3;

    [Header("Cell Layout")]
    [Tooltip("Width/height of each cell.")]
    public Vector2 cellSize = new Vector2(100f, 100f);

    [Tooltip("Horizontal/vertical spacing between cells.")]
    public Vector2 spacing = new Vector2(10f, 10f);

    /// <summary>Raised in the Editor whenever this asset's values change via the Inspector.</summary>
    public event Action Changed;

#if UNITY_EDITOR
    private void OnValidate()
    {
        Changed?.Invoke();
    }
#endif
}
