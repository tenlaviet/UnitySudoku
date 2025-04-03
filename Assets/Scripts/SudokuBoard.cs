using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Random = UnityEngine.Random;

public class SudokuBoard : MonoBehaviour
{
    public static SudokuBoard Instance { get; private set; }
    
    public SudokuCell sudokuCell;//Cell prefab
    private SudokuCell[,] SudokuCellArray;
    private SudokuCell currentSelectedCell;
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
        //HighlightedSudokuCellList = new List<SudokuCell>();
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
                SudokuCellArray[i,j].SetValue(0);
                //SudokuCellArray[i,j].SetCellState(SudokuCellState.Normal);
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

                SudokuCellArray[rowStart + i, colStart + j].SetValue(value);
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
                if (SudokuCellArray[rowStart + i, colStart + j].data._value == num) {
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
            if (SudokuCellArray[i, j].data._value == num) {
                return false;
            }
        }
        return true;
    }

    // Check if it's safe to put num in column j
    // Ensure num is not already used in the column
    private bool UnUsedInCol(int j, int num) {
        for (int i = 0; i < 9; i++) {
            if (SudokuCellArray[i, j].data._value == num) {
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
        if (SudokuCellArray[i, j].data._value != 0) {
            return FillRemaining(i, j + 1);
        }
        for (int num = 1; num <= 9; num++) {
            if (CheckIfSafe(i, j, num)) {
                SudokuCellArray[i, j].SetValue(num);
                if (FillRemaining(i, j + 1)) {
                    return true;
                }
                SudokuCellArray[i, j].SetValue(0);
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
            if (SudokuCellArray[i, j].data._value != 0)
            {
                SudokuCellArray[i, j].data.isEditable = true;
                SudokuCellArray[i, j].SetValue(0);
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
                solution[i, j] = SudokuCellArray[i, j].data._value;
            }
        }
        Solutions.Add(solution);
        RemoveRandom(a);
    }

    private void HighlightRow(int i, int j)
    {
        for (int k = 0; k < 9; k++)
        {
            SudokuCellArray[i,k].SetCellState(SudokuCellState.Related);
        }
    }
    
    private void HighlightColumn(int i, int j)
    {
        for (int k = 0; k < 9; k++)
        {
            SudokuCellArray[k,j].SetCellState(SudokuCellState.Related);
        }
    }
    private void HighlightBox(int i, int j)
    {
        //get start row and col of subgrid with given row col
    
        int rowStart = (i/3)*3;
        int colStart = (j/3)*3;
        //int startRow = row - (row % 3), startCol = col - (col % 3);this also works
        
        for (int k = 0; k < 3; k++)
        {
            for (int g = 0; g < 3; g++)
            {
                SudokuCellArray[rowStart + k,colStart+ g].SetCellState(SudokuCellState.Related);
            }
        }
    }

    private void HighlightSameNumber(int i, int j)
    {
        if (currentSelectedCell.data._value == 0)
        {
            return;
        }
        for (int k = 0; k < 9; k++)
        {
            for (int g = 0; g < 9; g++)
            {

                if (SudokuCellArray[k, g].data._value == currentSelectedCell.data._value
                    && SudokuCellArray[k, g].data.isValueValid
                    && k == i
                    && g == j)
                {

                        SudokuCellArray[k, g].SetCellState(SudokuCellState.Related);

                }
            }
        }
    }

    private void HighlightSelectedCell()
    {
        currentSelectedCell.SetCellState(SudokuCellState.Selected);
    }
    public void SelectCell(SudokuCell cell)
    {
        foreach (SudokuCell _cell in SudokuCellArray)// unhighlight all cell
        {
            _cell.SetCellState(SudokuCellState.Normal);
        }
        currentSelectedCell = cell;
        HighlightRow(currentSelectedCell.data.i,currentSelectedCell.data.j);
        HighlightColumn(currentSelectedCell.data.i,currentSelectedCell.data.j);
        HighlightBox(currentSelectedCell.data.i,currentSelectedCell.data.j);
        HighlightSameNumber(currentSelectedCell.data.i,currentSelectedCell.data.j);
        HighlightSelectedCell();
    }
    // private void ClearPreviousHighlightedCells()
    // {
    //     if (HighlightedSudokuCellList != null && HighlightedSudokuCellList.Count > 0)
    //     {
    //         foreach (SudokuCell cell in HighlightedSudokuCellList)
    //         {
    //             cell.SetCellState(SudokuCellState.Normal);
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
        //check valid
        
        //
        if (currentSelectedCell.data.isEditable == false)
        {
            Debug.Log("cant edit this cell");
            return;
        }

        if (isPlacedNumberValid(value))
        {
            //curent cell state = incorrect
            Debug.Log("valid");
        }
        else
        {
            Debug.Log("invalid");
        }
        currentSelectedCell.SetValue(value);

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
