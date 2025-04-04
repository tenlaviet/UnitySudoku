using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class SudokuBoard : MonoBehaviour
{
    public static SudokuBoard Instance { get; private set; }
    
    public SudokuCell sudokuCell;//Cell prefab
    private SudokuCell[,] SudokuCellArray;
    private SudokuCell currentSelectedCell;
    public List<SudokuCell> HighlightedSudokuCellList;
    public List<SudokuCell> InvalidSudokuCellList;
    private List<int[,]> Solutions;
    public Vector2 startPosition;
    public float offset;
    
    //undo related properties
    public List<CellData[]> BoardHistory;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        SudokuCellArray = new SudokuCell[9, 9];
        HighlightedSudokuCellList = new List<SudokuCell>();
        InvalidSudokuCellList = new List<SudokuCell>();
        Solutions = new List<int[,]>();
        BoardHistory = new List<CellData[]>();
        CreateSudokuBoard();
        SetupSudokuGrid();
        GenerateSudoku(Random.Range(25,40));
        //GenerateSudoku(Random.Range(70,80));
    }
    
    private void CreateSudokuBoard()// instantiate and save the sudoku cells in an array
    {
        for (int i = 0 ; i < SudokuCellArray.GetLength(0) ; i++)
        {
            for (int j = 0; j < SudokuCellArray.GetLength(1); j++)
            {
                string name = i + "x" + j;
                SudokuCellArray[i,j] = Instantiate(sudokuCell);
                SudokuCellArray[i, j].gameObject.name = name;
                SudokuCellArray[i,j].transform.SetParent(this.transform, false);//set parent for Cell
                SudokuCellArray[i,j].transform.localScale = new Vector3(1, 1, 1);
                SudokuCellArray[i,j].SetPosition(i,j);
            }
        }
    }

    private void SetupSudokuGrid()// sudoku grid setup
    {
        Rect SudokuCellRect = sudokuCell.GetComponent<RectTransform>().rect;
        Vector2 cellOffset = Vector2.zero;
        cellOffset.x = SudokuCellRect.width + offset;
        cellOffset.y = SudokuCellRect.height + offset;
        for (int i = 0 ; i < SudokuCellArray.GetLength(0) ; i++)
        {
            for (int j = 0; j < SudokuCellArray.GetLength(1); j++)
            {
                float xOffset = cellOffset.x * j;
                float yOffset = cellOffset.y * i;
                SudokuCell cell = SudokuCellArray[i, j];
                cell.GetComponent<RectTransform>().anchoredPosition = new Vector2(startPosition.x + xOffset, startPosition.y - yOffset);
            }
        }
    }

    private void FillBox(int rowStart, int colStart)
    {
        int value = 0;
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++)
            {
                do
                {
                    value = Random.Range(1, 10);
                } while (!UnUsedInBox(rowStart, colStart, value));

                SudokuCellArray[rowStart + i, colStart + j].SetValue(value, true, SudokuCellTextState.Default);
            }
        }
    }
    
    private void FillDiagonal() {
        // Fill each 3x3 subgrid diagonally
        for (int i = 0; i < 9; i = i + 3)
        {
            FillBox( i,i);
        }
    }
    private bool UnUsedInBox(int rowStart, int colStart, int num) {
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++) {
                if (SudokuCellArray[rowStart + i, colStart + j].GetValue() == num) {
                    return false;
                }
            }
        }
        return true;
    }
    // Check if it's safe to put num in row i
    // Ensure num is not already used in the row
    private bool UnUsedInRow(int i, int num) {
        for (int j = 0; j < 9; j++) {
            if (SudokuCellArray[i, j].GetValue() == num) {
                return false;
            }
        }
        return true;
    }

    // Check if it's safe to put num in column j
    // Ensure num is not already used in the column
    private bool UnUsedInCol(int j, int num) {
        for (int i = 0; i < 9; i++) {
            if (SudokuCellArray[i, j].GetValue() == num) {
                return false;
            }
        }
        return true;
    }


    // Check if it's safe to put num in the cell (i, j)
    // Ensure num is not used in row, column, or box
    private bool CheckIfSafe(int i, int j, int num) {
        return (UnUsedInRow(i, num) && UnUsedInCol(j, num) && UnUsedInBox(i - i % 3, j - j % 3, num));
    }
    
    // Fill remaining blocks in the grid
    // Recursively fill the remaining cells with valid numbers
    private bool FillRemaining(int i, int j) {
        if (i == 9) {
            return true;
        }
        if (j == 9) {
            return FillRemaining(i + 1, 0);
        }
        if (SudokuCellArray[i, j].GetValue() != 0) {
            return FillRemaining(i, j + 1);
        }
        for (int num = 1; num <= 9; num++) {
            if (CheckIfSafe(i, j, num)) {
                SudokuCellArray[i, j].SetValue(num,true, SudokuCellTextState.Default);
                if (FillRemaining(i, j + 1)) {
                    return true;
                }
                SudokuCellArray[i, j].SetValue(0,true,  SudokuCellTextState.Default);
            }
        }
        return false;
    }
    
    // Remove K digits randomly from the grid
    // This will create a Sudoku puzzle by removing digits
    private void RemoveRandom(int a) {
        while (a > 0) {
            int i = Random.Range(0,9);
            int j = Random.Range(0,9);
            if (SudokuCellArray[i, j].GetValue() != 0)
            {
                SudokuCellArray[i, j].data.isEditable = true;
                SudokuCellArray[i, j].SetValue(0, true, SudokuCellTextState.Default);
                a--;
            }
        }
    }
    private void ResetBoard()
    {
        HighlightedSudokuCellList = new List<SudokuCell>();
        InvalidSudokuCellList = new List<SudokuCell>();
        Solutions = new List<int[,]>();
        foreach (SudokuCell cell in SudokuCellArray)
        {
            cell.SetCellDefault();
        }
    }
    //Generate a Sudoku grid with (a) amount of empty cells
    public void GenerateSudoku(int a)
    {
        int[,] originalSolution = new int[9, 9];


        ResetBoard();
        FillDiagonal();
        FillRemaining(0, 0);
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                originalSolution[i, j] = SudokuCellArray[i, j].GetValue();
            }
        }
        Solutions.Insert(0,originalSolution);
        RemoveRandom(a);
        UpdateBoardDataHistory();
    }

    private void GetRelatedRow(int i, int j, List<SudokuCell> list)
    {
        if (list == null)
        {
            Debug.Log("list is null");
            return;
        }
        for (int k = 0; k < 9; k++)
        {
            if (!list.Contains(SudokuCellArray[i, k]))
            {
                list.Add(SudokuCellArray[i, k]);
            }
        }
    }
    
    private void GetRelatedColumn(int i, int j, List<SudokuCell> list)
    {
        if (list == null)
        {
            Debug.Log("list is null");
            return;
        }
        for (int k = 0; k < 9; k++)
        {
            if (!list.Contains(SudokuCellArray[k, j]))
            {
                list.Add(SudokuCellArray[k, j]);
            }
        }
    }
    private void GetRelatedBox(int i, int j, List<SudokuCell> list)
    {        
        if (list == null)
        {
            Debug.Log("list is null");
            return;
        }
        //get start row and col of subgrid with given row col
    
        int rowStart = (i/3)*3;
        int colStart = (j/3)*3;
        //int startRow = row - (row % 3), startCol = col - (col % 3);this also works
        
        for (int k = 0; k < 3; k++)
        {
            for (int g = 0; g < 3; g++)
            {
                if (!list.Contains(SudokuCellArray[rowStart + k, colStart+ g]))
                {
                    list.Add(SudokuCellArray[rowStart + k, colStart+ g]);
                }
            }
        }
    }

    private void GetRelatedNumber(int i, int j, List<SudokuCell> list)
    {
        if (list == null)
        {
            Debug.Log("list is null");
            return;
        }
        if (currentSelectedCell.GetValue() == 0)
        {
            return;
        }
        for (int k = 0; k < 9; k++)
        {
            for (int g = 0; g < 9; g++)
            {
                if (SudokuCellArray[k, g].GetValue() == currentSelectedCell.GetValue()
                    && SudokuCellArray[k, g].data.isValueValid
                    && k != i
                    && g != j)
                {
                    if (!list.Contains(SudokuCellArray[k, g]))
                    {
                        list.Add(SudokuCellArray[k, g]);
                    }
                }
            }
        }
    }

    private void HighlightSelectedCell()
    {
        currentSelectedCell.SetBackgroundState(SudokuCellBackgroundState.Selected);
    }

    private void HighlightRelated()
    {
        foreach (SudokuCell cell in HighlightedSudokuCellList)
        {
            if (cell.data.backgroundState == SudokuCellBackgroundState.Warning)
            {
                continue;
            }
            cell.SetBackgroundState(SudokuCellBackgroundState.Normal);
        }
        HighlightedSudokuCellList.Clear();
        
        int i = currentSelectedCell.data.i;
        int j = currentSelectedCell.data.j;
        GetRelatedRow(i,j, HighlightedSudokuCellList);
        GetRelatedColumn(i,j, HighlightedSudokuCellList);
        GetRelatedBox(i,j, HighlightedSudokuCellList);
        GetRelatedNumber(i,j, HighlightedSudokuCellList);
        foreach (SudokuCell cell in HighlightedSudokuCellList)
        {
                cell.SetBackgroundState(SudokuCellBackgroundState.Related);
        }
    }

    private void HighlightWarning()
    {
        foreach (SudokuCell cell in InvalidSudokuCellList)
        {
            List<SudokuCell> cellsWithSameValue = new List<SudokuCell>();
            int i = cell.data.i;
            int j = cell.data.j;
            List<SudokuCell> relatedList = new List<SudokuCell>();
            GetRelatedRow(i,j, relatedList);
            GetRelatedColumn(i,j, relatedList);
            GetRelatedBox(i,j, relatedList);
            foreach (SudokuCell _cell in relatedList)
            {

                if (_cell.GetValue() !=0 && _cell.GetValue() == cell.GetValue())
                {
                    cellsWithSameValue.Add(_cell);
                }
            }

            if (cellsWithSameValue.Count >= 2)
            {
                foreach (SudokuCell _cell in cellsWithSameValue)
                {
                    _cell.SetBackgroundState(SudokuCellBackgroundState.Warning);
                }
            }
        }
    }

    public void UpdateBoardState()
    {
        foreach (SudokuCell _cell in SudokuCellArray)
        {
            _cell.SetBackgroundState(SudokuCellBackgroundState.Normal);
        }
        
        HighlightRelated();
        HighlightWarning();
        HighlightSelectedCell();
        
        
    }
    
    public void SelectCell(SudokuCell cell)
    {
        if (currentSelectedCell !=null)
        {
            currentSelectedCell.data.isSelected = false;
        }
        currentSelectedCell = cell;
        currentSelectedCell.data.isSelected = true;
        UpdateBoardState();
    }
    
    
    

    public bool isPlacedNumberValid(int value)
    {
        if (Solutions != null && Solutions.Count > 0)
        {
            if (Solutions[0][currentSelectedCell.data.i,currentSelectedCell.data.j] == value)
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckWinCondition()
    {
        int count = 0;
        foreach (SudokuCell cell in SudokuCellArray)
        {
            if (cell.data.value != 0 && cell.data.isValueValid == true)
            {
                count++;
            }
        }

        if (count == 81)
        {
            Debug.Log("u won");
            return true;
        }

        return false;

    }
    public void PlaceNumber(int value)
    {
        if (currentSelectedCell.data.isEditable == false)
        {
            return;
        }

        SudokuCellTextState state = SudokuCellTextState.Default;
        bool isValid = false;
        if (isPlacedNumberValid(value))
        {
            if (InvalidSudokuCellList.Contains(currentSelectedCell))
            {
                InvalidSudokuCellList.Remove(currentSelectedCell);
            }
            isValid = true;
            state = SudokuCellTextState.Valid;
            
        }
        else
        {
            if (!InvalidSudokuCellList.Contains(currentSelectedCell))
            {
                InvalidSudokuCellList.Add(currentSelectedCell);
            }
            isValid = false;
            state = SudokuCellTextState.Invalid;

        }
        
        currentSelectedCell.SetValue(value, isValid, state);
        UpdateBoardState();
        UpdateBoardDataHistory();
        CheckWinCondition();


    }

    public void UpdateBoardDataHistory()
    {
        int position = 0;
        CellData[] boardData = new CellData[81];
        foreach (SudokuCell currentCell in SudokuCellArray)
        {
            boardData[position] = currentCell.data;
            position++;
        }
        BoardHistory.Insert(0,boardData);
        Debug.Log("history count:"+ BoardHistory.Count);
        string dattb = "";

        foreach (CellData data in BoardHistory[0])
        {
            dattb +="" +data.value;
        
        }
        Debug.Log("new board values:" + dattb);

    }
    public void Undo()
    {
        // Debug.Log("undo");
        // if (BoardHistory.Count <= 1)
        // {
        //     Debug.Log("history count:"+ BoardHistory.Count);
        //     return;
        // }
        // int position = 0;
        // foreach (SudokuCell currentCell in SudokuCellArray)
        // {
        //     currentCell.data = BoardHistory[0][position];
        //     currentCell.UpdateCell();
        //     position++;
        // }
        // BoardHistory.RemoveAt(0);
    }

    public void Erase()
    {
        if (InvalidSudokuCellList.Contains(currentSelectedCell))
        {
            InvalidSudokuCellList.Remove(currentSelectedCell);
        }
        currentSelectedCell.EraseCell();
        UpdateBoardDataHistory();
    }

    public void Pencil()
    {
        
    }
}
