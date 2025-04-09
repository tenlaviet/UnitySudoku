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
    public List<int> matrix;
}
[Serializable]
public class Root
{
    [SerializeField]
    public List<Puzzle> Puzzle;
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
        BoardHistory = new List<CellData[]>();
        CreateSudokuBoard();
        SetupSudokuGrid();
        //FillSudokuBoard(GetSudokuData(0));
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

    public void FillSudokuBoard(List<int> puzzle)
    {
        RestoreBoardToDefault();
        int position = 0;
        foreach (SudokuCell cell in SudokuCellArray)
        {
            bool isEditable = puzzle[position] == 0;

            cell.SetCellValue(puzzle[position], isEditable);
            position++;
        }
        UpdateBoardDataHistory();
    }
    //
    // puzzle generation// 
    private bool isOnlyOneInBox(int rowStart, int colStart, int num)
    {
        int count = 0;
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++) {
                if (SudokuCellArray[rowStart + i, colStart + j].GetValue() == num)
                {
                    count++;
                }
            }
        }
        if (count>1)
        {
            return false;
        }
        return true;
    }
    private bool isOnlyOneInRow(int i, int num)
    {
        int count = 0;
        for (int j = 0; j < 9; j++) {
            if (SudokuCellArray[i, j].GetValue() == num)
            {
                count++;
            }
        }

        if (count>1)
        {
            return false;
        }
        return true;
    }
    private bool isOnlyOneInCol(int j, int num)
    {
        int count = 0;
        for (int i = 0; i < 9; i++) {
            if (SudokuCellArray[i, j].GetValue() == num) {
                count++;
            }
        }
        if (count>1)
        {
            return false;
        }
        return true;
    }
    private bool CheckIfCellValueValid(int i, int j, int num) {

        return (isOnlyOneInRow(i, num) && isOnlyOneInCol(j, num) && isOnlyOneInBox(i - i % 3, j - j % 3, num));
    }
    private void RestoreBoardToDefault()
    {
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
    private void GetRelatedRow(int i,List<SudokuCell> list)
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
    private void GetRelatedColumn(int j, List<SudokuCell> list)
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
        currentSelectedCell.SetCellState(SudokuCellBackgroundState.Selected, currentSelectedCell.data.textState);
    }
    private void HighlightRelated()
    {
        List<SudokuCell> relatedHighlightSudokuCellList = new List<SudokuCell>();
        int i = currentSelectedCell.data.i;
        int j = currentSelectedCell.data.j;
        GetRelatedRow(i, relatedHighlightSudokuCellList);
        GetRelatedColumn(j, relatedHighlightSudokuCellList);
        GetRelatedBox(i, j, relatedHighlightSudokuCellList);
        GetRelatedNumber(i, j, relatedHighlightSudokuCellList);
        foreach (SudokuCell cell in relatedHighlightSudokuCellList)
        {
                cell.SetCellState(SudokuCellBackgroundState.Related, cell.data.textState);
        }
    }
    private void HighlightWarning()
    {
        foreach (SudokuCell cell in SudokuCellArray)
        {
            if (!cell.isEditable)
            {
                continue;
            }
            List<SudokuCell> relatedList = new List<SudokuCell>();
            List<SudokuCell> cellsWithSameValue = new List<SudokuCell>();
            int i = cell.data.i;
            int j = cell.data.j;
            GetRelatedRow(i, relatedList);
            GetRelatedColumn(j, relatedList);
            GetRelatedBox(i, j, relatedList);
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
                    SudokuCellTextState textState = _cell.data.textState;
                    if (_cell.isEditable)
                    {
                        textState= SudokuCellTextState.Invalid;
                    }
                    _cell.SetCellState(SudokuCellBackgroundState.Warning, textState);
                }
            }
        }
    }
    public void UpdateBoardState()
    {
        foreach (SudokuCell _cell in SudokuCellArray)
        {
            SudokuCellTextState textState;
            if (!_cell.isEditable)
            {
                textState = SudokuCellTextState.Default;
            }
            else
            {
                textState = SudokuCellTextState.Valid;

            }
            _cell.SetCellState(SudokuCellBackgroundState.Normal, textState);
        }
        HighlightRelated();
        HighlightWarning();
        HighlightSelectedCell();
        
        
    }
    //highlight cells
    //
    //check win condition/ player input
    public bool CheckWinCondition()
    {
        foreach (SudokuCell cell in SudokuCellArray)
        {
            if (cell.data.value == 0)
            {
                Debug.Log(cell.data.i + "x" + cell.data.j + ":"+ cell.data.value);

                Debug.Log("value == 0");
                return false;
            }
        
            if (!CheckIfCellValueValid(cell.data.i, cell.data.j, cell.data.value))
            {
                Debug.Log(cell.data.i + "x" + cell.data.j + ":"+ cell.data.value + "not valid");
                return false;
            }
        }
        return true;
        // for (int i = 0 ; i<9 ;i++)
        // {
        //     for (int j = 0 ; j <9; j++)
        //     {
        //         if (SudokuCellArray[i,j].data.value == 0)
        //         {
        //             Debug.Log("value == 0");
        //             return false;
        //         }
        //         if (!CheckIfCellValueValid(i,j,SudokuCellArray[i,j].data.value))
        //         {
        //             Debug.Log(SudokuCellArray[i,j].data.i + "x" + SudokuCellArray[i,j].data.j + ":"+ SudokuCellArray[i,j].data.value + "not valid");
        //
        //             return false;
        //         }
        //     }
        // }
        // return true;
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
        currentSelectedCell.SetCellValue(value, true);
        UpdateBoardState();
        UpdateBoardDataHistory();
        GameManager.Instance.ShowGameResult(CheckWinCondition());
    }
    public void Undo()
    {
        if (GameManager.Instance.isPaused)
        {
            return;
        }
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
        Debug.Log("undo");
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
    private void UpdateBoardDataHistory()
    {
        int position = 0;
        CellData[] boardData = new CellData[81];
        foreach (SudokuCell cell in SudokuCellArray)
        {
            boardData[position] = cell.data;
            position++;
        }
        BoardHistory.Insert(0,boardData);
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
