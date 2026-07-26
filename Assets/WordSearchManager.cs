using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class WordSearchManager : MonoBehaviour
{
    public static WordSearchManager Instance;
//    [System.Serializable]
//    public class WordEntry
//    {
//        public string word;
//        public bool found;
//    }
//    public List<WordEntry> words;
    public TMP_Text wordListText;
    

    [Header("Board")]
    public int width = 12;
    public int height = 12;

    [Header("Puzzle")]
    public string puzzleTitle = "Animals";

    public string[] words;
    private HashSet<string> foundWords = new HashSet<string>();


    [Header("References")]
    public GameObject cellPrefab;
    public Transform boardParent;
    public TMP_Text titleText;

    private char[,] board;
    private List<WordSearchCell> selectedCells = new List<WordSearchCell>();
    private bool isDragging = false;
    private Vector2Int dragDirection;
    private bool directionLocked = false;

    Vector2Int[] directions =
    {
        new Vector2Int(1, 0),   // →
        new Vector2Int(-1, 0),  // ←
        new Vector2Int(0, 1),   // ↓
        new Vector2Int(0, -1),  // ↑
        new Vector2Int(1, 1),   // ↘
        new Vector2Int(-1, -1), // ↖
        new Vector2Int(1, -1),  // ↗
        new Vector2Int(-1, 1)   // ↙
    };

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        //the ifs below are safety checks to make sure the error isn't vague

        if (titleText == null)
    {
        Debug.LogError("Title Text is not assigned!");
        return;
    }

    if (wordListText == null)
    {
        Debug.LogError("Word List Text is not assigned!");
        return;
    }

    if (cellPrefab == null)
    {
        Debug.LogError("Cell Prefab is not assigned!");
        return;
    }

    if (boardParent == null)
    {
        Debug.LogError("Board Parent is not assigned!");
        return;
    }
        titleText.text = puzzleTitle;

        board = new char[height, width];

        System.Array.Sort(words, (a, b) => b.Length.CompareTo(a.Length));

        PlaceWords();

        UpdateWordList();

        FillRemaining();

        CreateBoard();
    }

    void Update()
    {
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            FinishSelection();
        }
    }

    void UpdateWordList()
    {
        wordListText.text = "";

        foreach (string word in words)
        if (foundWords.Contains(word))
        {
            wordListText.text += "<s>• " + word + "</s>\n";
        }
        else
        {
            wordListText.text += "○ " + word + "\n";
        }
    }

    void PlaceWords()
    {
        foreach (string word in words)
        {
            List<(Vector2Int start, Vector2Int dir)> possiblePositions = new();

            foreach (Vector2Int dir in directions)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (CanPlaceWord(word, x, y, dir))
                        {
                            possiblePositions.Add((new Vector2Int(x, y), dir));
                        }
                    }
                }
            }

            if (possiblePositions.Count == 0)
            {
                Debug.LogWarning(word + " could not be placed.");
                continue;
            }

            var chosen = possiblePositions[Random.Range(0, possiblePositions.Count)];

            for (int i = 0; i < word.Length; i++)
            {
                int x = chosen.start.x + chosen.dir.x * i;
                int y = chosen.start.y + chosen.dir.y * i;

                board[y, x] = word[i];
            }
        }
    }

    bool CanPlaceWord(string word, int startX, int startY, Vector2Int dir)
    {
        int endX = startX + dir.x * (word.Length - 1);
        int endY = startY + dir.y * (word.Length - 1);

        if (endX < 0 || endX >= width)
            return false;

        if (endY < 0 || endY >= height)
            return false;

        for (int i = 0; i < word.Length; i++)
        {
            int x = startX + dir.x * i;
            int y = startY + dir.y * i;

            if (board[y, x] != '\0' &&
                board[y, x] != word[i])
                return false;
        }

        return true;
    }

    void FillRemaining()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (board[y, x] == '\0')
                    board[y, x] = (char)Random.Range('A', 'Z' + 1);
            }
        }
    }

    void CreateBoard()
    {
        GridLayoutGroup layout = boardParent.GetComponent<GridLayoutGroup>();
        layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layout.constraintCount = width;

        foreach (Transform child in boardParent)
            Destroy(child.gameObject);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject obj = Instantiate(cellPrefab, boardParent);

                //TMP_Text text = obj.GetComponentInChildren<TMP_Text>();

                WordSearchCell cell = obj.GetComponent<WordSearchCell>();

                cell.row = y;
                cell.column = x;
                cell.SetLetter(board[y, x]);

                //text.text = board[y, x].ToString();
            }
        }
    }

    public void StartSelection(WordSearchCell cell)
    {
        directionLocked = false;
        dragDirection = Vector2Int.zero;
        isDragging = true;
        selectedCells.Clear();
        AddCell(cell);
    }

    public void ContinueSelection(WordSearchCell cell)
    {

        // this is for if the player wants to go backwards to UN-highlight a cell
        if (selectedCells.Count >= 2 && cell == selectedCells[selectedCells.Count - 2])
        {
            WordSearchCell back = selectedCells[selectedCells.Count - 1];
            back.LowLight();
            selectedCells.RemoveAt(selectedCells.Count - 1);

            //unlock direction if only one cell is highlighted
            if (selectedCells.Count == 1)
            {
                directionLocked = false;
                dragDirection = Vector2Int.zero;
            }
            return;
        }


        if (selectedCells.Contains(cell))
            return;

        WordSearchCell last = selectedCells[selectedCells.Count - 1];

        int dx = cell.column - last.column;
        int dy = cell.row - last.row;

        if (Mathf.Abs(dx) > 1 || Mathf.Abs(dy) > 1) // Ignore if the cell is not adjacent
            return;

        dx = Mathf.Clamp(dx, -1, 1);
        dy = Mathf.Clamp(dy, -1, 1);

        Vector2Int newDirection = new Vector2Int(dx, dy);

        if (newDirection == Vector2Int.zero) // Ignore if the cell is not adjacent
            return;
        
        if (!directionLocked) // Lock the direction on the first valid move
        {
            dragDirection = newDirection;
            directionLocked = true;
        }

        else if (newDirection != dragDirection) // Ignore if the direction changes
        {
            return;
        }
        
        AddCell(cell);
    }

    void FinishSelection()
    {
        isDragging = false;

        string selectedWord = "";

        foreach (WordSearchCell cell in selectedCells)
        {
            selectedWord += cell.letter;
        }

        Debug.Log(selectedWord);

        bool found = false;

        foreach (string word in words)
        {
            if (selectedWord.Equals(word, System.StringComparison.OrdinalIgnoreCase) && !foundWords.Contains(word))
            {
                found = true;
                foundWords.Add(selectedWord);
                UpdateWordList();

                if (foundWords.Count == words.Length)
                {
                    Debug.Log("All words found!");
                }

                break;
            }
        }

        foreach (WordSearchCell cell in selectedCells)
        {
            if (found)
                cell.MarkFound();
            else     
                cell.LowLight();
        }

        selectedCells.Clear();
    }

    void AddCell(WordSearchCell cell) // Add a cell to the selection when a dragging input is detected
    {
        selectedCells.Add(cell);
        cell.Highlight();
    }
}