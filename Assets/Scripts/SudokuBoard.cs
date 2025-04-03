using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class SudokuBoard : MonoBehaviour
{
    public static SudokuBoard Instance { get; private set; }
    
    public SudokuCell sudokuCell;//Cell prefab
    private SudokuCell[,] SudokuCellArray;
    private SudokuCell currentSelectedCell;
    private List<SudokuCell> HighlightedSudokuCellList;
    private List<int[,]> Solutions; 
    private int rows = 9;
    private int columns = 9;
    public Vector2 startPosition;
    public float offset;

    
    

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
        Solutions = new List<int[,]>();
        CreateSudokuBoard();
        SetupSudokuGrid();
        GenerateSudoku(Random.Range(25,40));
    }
    
    private void CreateSudokuBoard()// instantiate and save the sudoku cells in an array
    {
        for (int i = 0 ; i < SudokuCellArray.GetLength(0) ; i++)
        {
            for (int j = 0; j < SudokuCellArray.GetLength(1); j++)
            {
                SudokuCellArray[i,j] = Instantiate(sudokuCell);
                SudokuCellArray[i,j].transform.SetParent(this.transform, false);//set parent for Cell
                SudokuCellArray[i,j].transform.localScale = new Vector3(1, 1, 1);
                SudokuCellArray[i,j].SetPosition(i,j);
                SudokuCellArray[i,j].SetValue(0, true, SudokuCellTextState.Default);
                //SudokuCellArray[i,j].SetBackgroundState(SudokuCellState.Normal);
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
                if (SudokuCellArray[rowStart + i, colStart + j].data.value == num) {
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
            if (SudokuCellArray[i, j].data.value == num) {
                return false;
            }
        }
        return true;
    }

    // Check if it's safe to put num in column j
    // Ensure num is not already used in the column
    private bool UnUsedInCol(int j, int num) {
        for (int i = 0; i < 9; i++) {
            if (SudokuCellArray[i, j].data.value == num) {
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
        if (SudokuCellArray[i, j].data.value != 0) {
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
            if (SudokuCellArray[i, j].data.value != 0)
            {
                SudokuCellArray[i, j].data.isEditable = true;
                SudokuCellArray[i, j].SetValue(0, true, SudokuCellTextState.Default);
                a--;
            }
        }
    }
    private void EmptyBoard()
    {
        foreach (SudokuCell cell in SudokuCellArray)
        {
            cell.EmptyCell();
        }
    }
    //Generate a Sudoku grid with (a) amount of empty cells
    public void GenerateSudoku(int a)
    {
        int[,] solution = new int[9, 9];

        EmptyBoard();
        FillDiagonal();
        FillRemaining(0, 0);
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                solution[i, j] = SudokuCellArray[i, j].data.value;
            }
        }
        Solutions.Add(solution);
        RemoveRandom(a);
    }

    private void GetRelatedRow(int i, int j)
    {
        for (int k = 0; k < 9; k++)
        {
            HighlightedSudokuCellList.Add(SudokuCellArray[i,k]);
        }
    }
    
    private void GetRelatedColumn(int i, int j)
    {
        for (int k = 0; k < 9; k++)
        {
            HighlightedSudokuCellList.Add(SudokuCellArray[k,j]);
        }
    }
    private void GetRelatedBox(int i, int j)
    {
        //get start row and col of subgrid with given row col
    
        int rowStart = (i/3)*3;
        int colStart = (j/3)*3;
        //int startRow = row - (row % 3), startCol = col - (col % 3);this also works
        
        for (int k = 0; k < 3; k++)
        {
            for (int g = 0; g < 3; g++)
            {
                HighlightedSudokuCellList.Add(SudokuCellArray[rowStart + k,colStart+ g]);
            }
        }
    }

    private void GetRelatedNumber(int i, int j)
    {
        if (currentSelectedCell.data.value == 0)
        {
            return;
        }
        for (int k = 0; k < 9; k++)
        {
            for (int g = 0; g < 9; g++)
            {

                if (SudokuCellArray[k, g].data.value == currentSelectedCell.data.value
                    && SudokuCellArray[k, g].data.isValueValid
                    && k != i
                    && g != j)
                {
                        HighlightedSudokuCellList.Add(SudokuCellArray[k, g]);
                }
            }
        }
    }

    private void HighlightWarning()
    {
        
    }
    private void HighlightSelectedCell()
    {
        currentSelectedCell.SetBackgroundState(SudokuCellBackgroundState.Selected);
    }

    private void HighlightRelated()
    {
        HighlightedSudokuCellList.Clear();
        
        int i = currentSelectedCell.data.i;
        int j = currentSelectedCell.data.j;
        GetRelatedRow(i,j);
        GetRelatedColumn(i,j);
        GetRelatedBox(i,j);
        GetRelatedNumber(i,j);
        foreach (SudokuCell cell in HighlightedSudokuCellList)
        {
            cell.SetBackgroundState(SudokuCellBackgroundState.Related);
        }
    }
    public void SelectCell(SudokuCell cell)
    {
        foreach (SudokuCell _cell in SudokuCellArray)// unhighlight all cell
        {
            _cell.SetBackgroundState(SudokuCellBackgroundState.Normal);
        }
        currentSelectedCell = cell;
        HighlightRelated();
        HighlightSelectedCell();
    }
    // private void ClearPreviousHighlightedCells()
    // {
    //     if (HighlightedSudokuCellList != null && HighlightedSudokuCellList.Count > 0)
    //     {
    //         foreach (SudokuCell cell in HighlightedSudokuCellList)
    //         {
    //             cell.SetBackgroundState(SudokuCellState.Normal);
    //         }
    //         HighlightedSudokuCellList.Clear();
    //     }
    // }
    // public void UpdateCurrentSelectedCell(SudokuCell cell)
    // {
    //     currentSelectedCell = cell;
    //     HighlightAllRelatedCells(cell);
    //     ClearPreviousHighlightedCells();
    //
    //     //
    // }
    
    public void PlaceNumber(int value)
    {
        if (currentSelectedCell.data.isEditable == false)
        {
            Debug.Log("cant edit this cell");
            return;
        }

        SudokuCellTextState state = SudokuCellTextState.Default;
        bool isValid = false;
        if (isPlacedNumberValid(value))
        {
            isValid = true;
            state = SudokuCellTextState.Valid;
            Debug.Log("valid");
        }
        else
        {
            isValid = false;
            state = SudokuCellTextState.Invalid;
            Debug.Log("invalid");
        }
        currentSelectedCell.SetValue(value, isValid, state);

    }

    public bool isPlacedNumberValid(int value)
    {
        if (Solutions != null && Solutions.Count > 0)
        {
            if (Solutions[0][currentSelectedCell.data.i,currentSelectedCell.data.j] == value)
            {
                Debug.Log("solution value: " + Solutions[0][currentSelectedCell.data.i,currentSelectedCell.data.j] + "input value: "+ value);
                return true;
            }
        }


        return false;
    }

    public void Undo()
    {
        
    }

    public void Erase()
    {
        
    }
}
