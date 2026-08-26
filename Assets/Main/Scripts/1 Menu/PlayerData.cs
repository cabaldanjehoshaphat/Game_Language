using System;

/// <summary>
/// PlayerData
/// Plain serializable data model for one player's save data: identity plus per-game
/// level-completion progress. This is the schema persisted to disk by PlayerDatabase.
/// To track a new game's progress later, add one more int field here (following the
/// existing "*Progress" fields) and a matching property in PlayerDatabase.
/// </summary>
[Serializable]
public class PlayerData
{
    public string playerNameOrId = "jeho";

    public int wordleProgress = 0;
    public int crosswordProgress = 0;
    public int wordSearchProgress = 0;
}
