using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
[Serializable]
public class Puzzle
{
    public int difficulty;
    [SerializeField]
    public List<int> puzzle;
}
[Serializable]
public class PuzzleData
{
    [SerializeField]
    public List<Puzzle> data;
}



public class SudokuBoard : MonoBehaviour
{
    public static SudokuBoard Instance { get; private set; }

    public TextAsset textAsset;
    // Board generation
    public SudokuCell sudokuCell;//Cell prefab
    private SudokuCell[,] SudokuCellArray;
    public Vector2 startPosition;
    public float offset;
    //current game data
    private SudokuCell currentSelectedCell;
    private List<int[,]> Solutions;
    public List<CellData[]> BoardHistory;
    //
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
        Solutions = new List<int[,]>();
        BoardHistory = new List<CellData[]>();
        CreateSudokuBoard();
        SetupSudokuGrid();
        //GenerateSudoku(Random.Range(25,40));
        //GenerateSudoku(Random.Range(70,80));
        FillSudokuBoard(GetSudokuData(0));
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
    //grab sudoku data from json file
    private List<int> GetSudokuData(int difficulty)
    {
        
        string json = File.ReadAllText(Application.dataPath + "/SudokuData/Data.json") ;
        PuzzleData puzzleDataList = JsonUtility.FromJson<PuzzleData>(json);
        List<List<int>> puzzles = new List<List<int>>();
        foreach (var puzzle in puzzleDataList.data)
        {
            // if (puzzle.difficulty == difficulty)
            // {
            //     puzzles.Add(puzzle.valueList);
            // }
            puzzles.Add(puzzle.puzzle);
        }

        return puzzles[Random.Range(0, puzzles.Count)];

    }

    public void FillSudokuBoard(List<int> puzzle)
    {
        int position = 0;
        bool isValid = false;
        bool isEditable = false;
        foreach (SudokuCell cell in SudokuCellArray)
        {

            if (puzzle[position]!=0)
            {
                isValid = true;
                isEditable = false;
            }
            else
            {
                isValid = false;
                isEditable = true;
            }
            cell.SetCellValue(puzzle[position], isValid, isEditable);
            position++;
        }
    }
    //
    // puzzle generation// 

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
    private bool UnUsedInRow(int i, int num) {
        for (int j = 0; j < 9; j++) {
            if (SudokuCellArray[i, j].GetValue() == num) {
                return false;
            }
        }
        return true;
    }
    private bool UnUsedInCol(int j, int num) {
        for (int i = 0; i < 9; i++) {
            if (SudokuCellArray[i, j].GetValue() == num) {
                return false;
            }
        }
        return true;
    }
    private bool CheckIfSafe(int i, int j, int num) {
        return (UnUsedInRow(i, num) && UnUsedInCol(j, num) && UnUsedInBox(i - i % 3, j - j % 3, num));
    }

    public void GenerateSudoku(int a)
         {
             int[,] originalSolution = new int[9, 9];
     
     
             ResetBoard();
             for (int i = 0; i < 9; i++)
             {
                 for (int j = 0; j < 9; j++)
                 {
                     originalSolution[i, j] = SudokuCellArray[i, j].GetValue();
                 }
             }
             Solutions.Insert(0,originalSolution);
             UpdateBoardDataHistory();
         }
    private void ResetBoard()
    {
        Solutions = new List<int[,]>();
        BoardHistory = new List<CellData[]>();
        currentSelectedCell = null;
        foreach (SudokuCell cell in SudokuCellArray)
        {
            cell.SetCellDefault();
        }
    }
    //puzzle generation
    //
    //highlight cells
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
                    )
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
        List<SudokuCell> relatedHighlightSudokuCellList = new List<SudokuCell>();
        int i = currentSelectedCell.data.i;
        int j = currentSelectedCell.data.j;
        GetRelatedRow(i,j, relatedHighlightSudokuCellList);
        GetRelatedColumn(i,j, relatedHighlightSudokuCellList);
        GetRelatedBox(i,j, relatedHighlightSudokuCellList);
        GetRelatedNumber(i,j, relatedHighlightSudokuCellList);
        foreach (SudokuCell cell in relatedHighlightSudokuCellList)
        {
                cell.SetBackgroundState(SudokuCellBackgroundState.Related);
        }
    }
    private void HighlightWarning()
    {
        foreach (SudokuCell cell in SudokuCellArray)
        {
            if (cell.isEditable)
            {
                
            }
            //if(cell.GetValue() == )
        }
    }
    public void UpdateBoardState()
    {
        foreach (SudokuCell _cell in SudokuCellArray)
        {
            _cell.SetBackgroundState(SudokuCellBackgroundState.Normal);
        }
        //List<SudokuCell> cellsWithSameValue = new List<SudokuCell>();
        HighlightRelated();
        //HighlightWarning();
        HighlightSelectedCell();
        
        
    }
    //highlight cells
    //
    //check win condition/ player input
    public bool IsPlacedNumberValid(int value)
    {

        // if (Solutions != null && Solutions.Count > 0)
        // {
        //     if (Solutions[0][currentSelectedCell.data.i,currentSelectedCell.data.j] == value)
        //     {
        //         return true;
        //     }
        // }
        // return false;
        return false;
    }
    public bool CheckWinCondition()
    {
        int count = 0;
        foreach (SudokuCell cell in SudokuCellArray)
        {
            if (cell.data.value != 0 && cell.isValueValid == true)
            {
                count++;
            }
        }

        if (count == 81)
        {
            GameManager.Instance.ShowGameResult(true);
        }

        return false;

    }
    //
    //player actions
    public void SelectCell(SudokuCell cell)
    {
        if (GameManager.Instance.isPaused)
        {
            return;
        }
        if (currentSelectedCell !=null)
        {
            currentSelectedCell.isSelected = false;
        }
        currentSelectedCell = cell;
        currentSelectedCell.isSelected = true;
        UpdateBoardState();
    }

    public void PlaceNumber(int value)
    {
        if (currentSelectedCell == null)
        {
            return;
        }
        if (currentSelectedCell.isEditable == false)
        {
            return;
        }

        bool isValid = false;
        if (IsPlacedNumberValid(value))
        {
            isValid = true;
        }
        // else
        // {
        //     isValid = false;
        //     GameManager.Instance.IncrementMistakeCount();
        // }
        
        currentSelectedCell.SetCellValue(value, isValid, false);
        UpdateBoardState();
        UpdateBoardDataHistory();
        CheckWinCondition();
    }

    private void UpdateBoardDataHistory()
    {
        int position = 0;
        CellData[] boardData = new CellData[81];
        foreach (SudokuCell currentCell in SudokuCellArray)
        {
            boardData[position] = currentCell.data;
            position++;
        }
        BoardHistory.Insert(0,boardData);
    }
    public void Undo()
    {
        if (GameManager.Instance.isPaused)
        {
            return;
        }
        Debug.Log("undo");
        if (BoardHistory.Count <= 1)
        {
             Debug.Log("cant undo anymore");
             return;
        }
        int position = 0;
        foreach (SudokuCell currentCell in SudokuCellArray)
        {
            currentCell.data = BoardHistory[1][position];
            currentCell.UpdateCell();
            position++;
        }
        BoardHistory.RemoveAt(0);
    }
    public void Erase()
    {
        if (currentSelectedCell == null)
        {
            return;
        }
        if (GameManager.Instance.isPaused)
        {
            return;
        }
        if (!currentSelectedCell.isEditable)
        {
            return;
        }
        currentSelectedCell.EraseCellValue();
        UpdateBoardState();
        UpdateBoardDataHistory();
    }
    public void Pencil()
    {
        if (GameManager.Instance.isPaused)
        {
            return;
        }
    }

    public void ToggleBoardActive(bool value)
    {
        foreach (var cell in SudokuCellArray)
        {
            cell.gameObject.SetActive(!value);
        }
    }
    //player actions

}
