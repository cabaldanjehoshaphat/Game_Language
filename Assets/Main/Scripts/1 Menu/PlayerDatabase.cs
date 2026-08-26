using System.IO;
using UnityEngine;

/// <summary>
/// PlayerDatabase
/// Singleton persistence manager for the player's save data (PlayerData): loads it from
/// disk on startup, exposes read/write access to identity and each game's progress, and
/// saves back to disk whenever a value changes. Survives scene loads (DontDestroyOnLoad),
/// so any scene (menu, profile screen, or a game itself) can reach it via
/// PlayerDatabase.Instance.
///
/// To add a new game's progress later: add a field to PlayerData, then add a property
/// here following the same pattern as WordleProgress/CrosswordProgress/WordSearchProgress.
/// </summary>
public class PlayerDatabase : MonoBehaviour
{
    public static PlayerDatabase Instance { get; private set; }

    [SerializeField]
    private PlayerData data = new PlayerData();

    public PlayerData Data => data;

    private string SavePath => Path.Combine(Application.persistentDataPath, "player_data.json");

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    public string PlayerNameOrId
    {
        get => data.playerNameOrId;
        set { data.playerNameOrId = value; Save(); }
    }

    public int WordleProgress
    {
        get => data.wordleProgress;
        set { data.wordleProgress = value; Save(); }
    }

    public int CrosswordProgress
    {
        get => data.crosswordProgress;
        set { data.crosswordProgress = value; Save(); }
    }

    public int WordSearchProgress
    {
        get => data.wordSearchProgress;
        set { data.wordSearchProgress = value; Save(); }
    }

    /// <summary>Loads player_data.json from disk, or starts fresh defaults if none exists yet.</summary>
    public void Load()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            data = JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            data = new PlayerData();
        }
    }

    /// <summary>Writes the current PlayerData to disk as JSON.</summary>
    public void Save()
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }
}
