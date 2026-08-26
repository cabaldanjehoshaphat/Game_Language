using UnityEngine;

// [DefaultExecutionOrder(-1)]
public class Board : MonoBehaviour
{
    private static readonly KeyCode[] SUPPORTED_KEYS = new KeyCode[]
    {
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D,
        KeyCode.E, KeyCode.F, KeyCode.G, KeyCode.H,
        KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L,
        KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, 
        KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T, 
        KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X, 
        KeyCode.Y, KeyCode.Z,
    };

    private static readonly string[] SEPARATOR = new string[] { "\r\n", "\r", "\n" };
    private Row[] rows;
    private string[] solutions;
    private string[] validWords;

    private string word;

    private int rowIndex;
    private int columnIndex;

    // title of header
    [Header("States of jeho")]
    public Tile.State emptyState;
    public Tile.State occupiedState;
    public Tile.State correctState;
    public Tile.State wrongSpotState;
    public Tile.State incorrectState;


    private void Awake()
    {
        rows = GetComponentsInChildren<Row>();
        // Debug.Log("Rows found: " + rows.Length); //display the number of rows
    }
    
    private void Start()
    {
        LoadData();
        SetRandomWord();
    }

    private void LoadData()
    {
        // Resouces dictionary list
        /*
        TextAsset textFile = Resources.Load("official_wordle_common") as TextAsset;
        solutions  = textFile.text.Split(SEPARATOR, System.StringSplitOptions.None);

        textFile = Resources.Load("official_wordle_all") as TextAsset;
        validWords = textFile.text.Split(SEPARATOR, System.StringSplitOptions.None);
        */
        TextAsset textFile = Resources.Load("Dictionary/words") as TextAsset;
        solutions  = textFile.text.Split(SEPARATOR, System.StringSplitOptions.None);
        
    }

    private void SetRandomWord()
    {
        word = solutions[Random.Range(0, solutions.Length)];
        word = word.ToLower().Trim();
        // word = "abang";
    }

    private void Update()
    {
        Row currentRow = rows[rowIndex];

        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            // backspace
            columnIndex = Mathf.Max(columnIndex - 1, 0);
            currentRow.tiles[columnIndex].SetLetter('\0');
            currentRow.tiles[columnIndex].SetState(emptyState);
            
        }

        else if (columnIndex >= currentRow.tiles.Length)
        {
            // keyboard input and submit when the row is complete
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SubmitRow(currentRow);
            }
        }

        else
        {
            // current column to next max column then next to row repeat
            for (int i = 0; i < SUPPORTED_KEYS.Length; i++)
            {
                if (Input.GetKeyDown(SUPPORTED_KEYS[i]))
                {
                    currentRow.tiles[columnIndex].SetLetter((char)SUPPORTED_KEYS[i]);
                    currentRow.tiles[columnIndex].SetState(occupiedState);
                    columnIndex++;
                    break;
                }
            }
        }    
    }

    private void SubmitRow(Row row)
    {
        // for (int i = 0; i < row.tiles.Length; i++)
        // {
        //     Tile tile = row.tiles[i];

        //     if (tile.letter == word[i])
        //     {
        //         // correct
        //         tile.SetState(correctState);
        //     }
        //     else if (word.Contains(tile.letter))
        //     {
        //         // wrong spot
        //         tile.SetState(wrongSpotState);
        //     }
        //     else
        //     {
        //         // incorrect
        //             tile.SetState(incorrectState);
        //     }
        // }

        string remaining = word;

        // solving/correcting the position of every letters 
        for (int i = 0; i < row.tiles.Length; i++)
        {
            Tile tile = row.tiles[i];

            if (tile.letter == word[i])
            {
                tile.SetState(correctState);

                remaining = remaining.Remove(i, 1);
                remaining = remaining.Insert(i, " ");
            }
            else if (!word.Contains(tile.letter))
            {
                tile.SetState(incorrectState);
            }    
        }
        // solving/correcting the position of every letters 
        for (int i = 0; i < row.tiles.Length; i++)
        {
            Tile tile = row.tiles[i];

            if (tile.state != correctState && tile.state != incorrectState)
            {
                if (remaining.Contains(tile.letter))
                {
                    tile.SetState(wrongSpotState);
                    int index = remaining.IndexOf(tile.letter);
                    remaining = remaining.Remove(index, 1);
                    remaining = remaining.Insert(index, " ");

                }
                else
                {
                    tile.SetState(incorrectState);
                }
            }
        }


        rowIndex++;
        columnIndex = 0;

        // Debug.Log("Row count: " + rowIndex + " Column Count: " + columnIndex);

        if (rowIndex >= rows.Length)
        {
            enabled = false;
        }
    }
}
