using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class CrosswordManager : MonoBehaviour
{
    public static CrosswordManager Instance;

    [Header("References")]
    public GameObject cellPrefab;
    public Transform gridParent;

    [Header("Puzzle Input")]
    public string[] puzzleRows;

    private CrosswordCell[,] grid;
    private char[,] solution;
    private char[,] playerFill;
    private CrosswordCell selectedCell;
    private Button selectedClueButton;
    public Transform clueListParent;
    public GameObject clueItemPrefab;

    [System.Serializable]
    public class Clue
    {
        public string clueText;
        public int row;
        public int col;
        public bool isAcross;
    }

    [Header("Clues")]
    public Clue[] clues;

    enum Direction { Across, Down }
    Direction currentDirection = Direction.Across;

    void Awake() => Instance = this;

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        CreateTestPuzzle();
    }

    void CreateTestPuzzle()
    {
        foreach (Transform child in gridParent)
            Destroy(child.gameObject);

        if (puzzleRows == null || puzzleRows.Length == 0)
        {
            Debug.LogError("No puzzle data!");
            return;
        }

        for (int i = 0; i < puzzleRows.Length; i++)
        {
            if (puzzleRows[i].Length != puzzleRows[0].Length)
            {
                Debug.LogError($"Row {i} length mismatch!");
                return;
            }
        }

        int rows = puzzleRows.Length;
        int cols = puzzleRows[0].Length;

        GridLayoutGroup layout = gridParent.GetComponent<GridLayoutGroup>();
        layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layout.constraintCount = cols;

        grid = new CrosswordCell[rows, cols];
        playerFill = new char[rows, cols];
        solution = new char[rows, cols];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                GameObject go = Instantiate(cellPrefab, gridParent);
                CrosswordCell cell = go.GetComponent<CrosswordCell>();

                cell.row = r;
                cell.col = c;

                char ch = puzzleRows[r][c];

                if (ch == '-') // if empty, cell becomes black
                {
                    cell.SetBlack(true);
                }
                else // otherwise it is white, and can be filled with letters
                {
                    cell.SetBlack(false);
                    solution[r, c] = char.ToUpper(ch);
                }

                grid[r, c] = cell;
            }
        }

        int number = 1; //this sets the numbers of the crossword, whether across, down, or both

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (grid[r, c].isBlack) continue;

                bool startAcross =
                    c == 0 || grid[r, c - 1].isBlack;

                bool startDown =
                    r == 0 || grid[r - 1, c].isBlack;

                if (startAcross || startDown)
                {
                    grid[r, c].clueNumber = number; // store number in cell
                    grid[r, c].numberText.text = number.ToString(); // display it
                    number++;
                }
            }
        }
        Debug.Log("a crossword should be created here! if somethings amiss uhhhhh gl lol.");
        GenerateClueUI();
    }

    void GenerateClueUI() // generates the clue list UI based on the clues array, and sets up button listeners to select the corresponding clue when clicked
    {
        
        foreach (Transform child in clueListParent)
            Destroy(child.gameObject);

        foreach (var clue in clues)
        {
            
            GameObject go = Instantiate(clueItemPrefab, clueListParent);
            go.transform.localScale = Vector3.one; //temporary
            TMP_Text text = go.GetComponentInChildren<TMP_Text>();

            text.text = clue.clueText;

            Button btn = go.GetComponent<Button>();
            Clue capturedClue = clue;
            Button capturedButton = btn;

            btn.onClick.AddListener(() => SelectClue(capturedClue, capturedButton));
            Debug.Log($"Clue: {clue.clueText}");
            Debug.Log(go.GetComponent<RectTransform>().rect.size + "If this is a small number like 20, 20, then the clue UI is not scaling properly. If it's a larger number like 200, 200, then it is scaling properly.");
        }
        Debug.Log($"Generating {clues.Length} clues");
    }

    public void SelectClue(Clue clue, Button btn)
    {
        if (selectedClueButton != null)
        {
            selectedClueButton.image.color = Color.white;
        }
        selectedClueButton = btn;
        selectedClueButton.image.color = Color.yellow;
        currentDirection = clue.isAcross ? Direction.Across : Direction.Down;
        SelectCell(grid[clue.row, clue.col], false);
    }

    public void DeselectClue()
    {
        if (selectedClueButton != null)
        {
            selectedClueButton.image.color = Color.white;
            selectedClueButton = null;
        }
    }

    public void SelectCell(CrosswordCell cell, bool allowToggle = true)
    {
        if (cell.isBlack) return;

        // toggle direction if same cell clicked
        if (allowToggle && selectedCell == cell)
        {
            currentDirection = (currentDirection == Direction.Across)
                ? Direction.Down
                : Direction.Across;
        }

        selectedCell = cell;
        HighlightWord(cell);

        Debug.Log($"Selected: {cell.row},{cell.col}");
    }

    void MoveSelection(int rowOffset, int colOffset)
    {
        int r = selectedCell.row + rowOffset;
        int c = selectedCell.col + colOffset;

        if (r < 0 || r >= grid.GetLength(0))
            return;

        if (c < 0 || c >= grid.GetLength(1))
            return;

        if (grid[r, c].isBlack)
            return;

        SelectCell(grid[r, c]);
    }

    void Update()
        {
            if (selectedCell == null) return;

            if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    currentDirection = Direction.Across;
                    MoveSelection(0, -1);
                }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                currentDirection = Direction.Across;
                MoveSelection(0, 1);
                }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    currentDirection = Direction.Down;
                    MoveSelection(-1, 0);
                }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    currentDirection = Direction.Down;
                    MoveSelection(1, 0);
                } //these are the arrow key controls for moving through the crossword grid, and they also set the current direction based on the arrow pressed.

            if (Input.GetKeyDown(KeyCode.Tab))
                {
                    currentDirection = currentDirection == Direction.Across
                        ? Direction.Down
                        : Direction.Across;

                    HighlightWord(selectedCell);
                }

            if (Input.GetKeyDown(KeyCode.Backspace))
                {
                    int r = selectedCell.row;
                    int c = selectedCell.col;

                    // Always erase the current cell
                    playerFill[r, c] = '\0';
                    selectedCell.letterText.text = "";
                    MoveToPreviousCell();
                    }
                else if (!string.IsNullOrEmpty(Input.inputString))
                    {   
                        char letter = char.ToUpper(Input.inputString[0]);

                        if (char.IsLetter(letter))
                            {
                                int r = selectedCell.row;
                                int c = selectedCell.col;

                                playerFill[r, c] = letter;
                selectedCell.letterText.text = letter.ToString();

                MoveToNextCell();
                    if (CheckWin())
                    {
                        Debug.Log("Puzzle Complete!");
                        // gameStatus = GameStatus.End;
                        // Show win panel, play sound, etc.
                    }
            }
        }
    }

        void MoveToNextCell()
    {
        int r = selectedCell.row;
        int c = selectedCell.col;

        if (currentDirection == Direction.Across)
        {
            c++;
            while (c < grid.GetLength(1) && grid[r, c].isBlack)
                c++;

            if (c < grid.GetLength(1))
                SelectCell(grid[r, c]);
        }
        else
        {
            r++;
            while (r < grid.GetLength(0) && grid[r, c].isBlack)
                r++;

            if (r < grid.GetLength(0))
                SelectCell(grid[r, c]);
        }
    }
        void MoveToPreviousCell()
    {
        int r = selectedCell.row;
        int c = selectedCell.col;

        if (currentDirection == Direction.Across)
        {
            c--;
            while (c >= 0 && grid[r, c].isBlack)
                c--;

            if (c >= 0)
                SelectCell(grid[r, c]);
        }   
        else
        {
            r--;
            while (r >= 0 && grid[r, c].isBlack)
            r--;

            if (r >= 0)
                SelectCell(grid[r, c]);
        }
    }    

    void HighlightWord(CrosswordCell cell)
    {
        ClearHighlights();

        int r = cell.row;
        int c = cell.col;

        if (currentDirection == Direction.Across) //if clicked when across, becomes down
        {
            int start = c;
            while (start > 0 && !grid[r, start - 1].isBlack) start--;

            int end = c;
            while (end < grid.GetLength(1) - 1 && !grid[r, end + 1].isBlack) end++;

            for (int i = start; i <= end; i++)
                grid[r, i].SetHighlight(true, grid[r, i] == selectedCell); // the clicked cell should highlight blue, the rest of the word should highlight yellow
        }
        else // vice versa
        {
            int start = r;
            while (start > 0 && !grid[start - 1, c].isBlack) start--;

            int end = r;
            while (end < grid.GetLength(0) - 1 && !grid[end + 1, c].isBlack) end++;

            for (int i = start; i <= end; i++)
                grid[i, c].SetHighlight(true, grid[i, c] == selectedCell); // same idea as the other, just swapped through rows instead of columns
        }
    }

    void ClearHighlights()
    {
        foreach (var cell in grid)
        {
            if (!cell.isBlack)
                cell.SetHighlight(false);
        }
    }

    bool CheckWin()
    {
        for (int r = 0; r < grid.GetLength(0); r++)
        {
            for (int c = 0; c < grid.GetLength(1); c++)
            {
                if (grid[r, c].isBlack)
                continue;

                if (playerFill[r, c] != solution[r, c])
                return false;
            }
        }
        return true;
    }
}