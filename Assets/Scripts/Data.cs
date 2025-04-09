using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Difficulty
{
    Easy = 0,
    Medium = 1,
    Hard = 2
}
public enum SudokuCellBackgroundState
{
    Normal = 0,
    Related = 1,
    Warning = 2,
    Selected = 3,
}
public enum SudokuCellTextState
{
    Default,
    Valid,
    Invalid
}
public static class Data
{
    public static readonly Dictionary<SudokuCellBackgroundState, Color> cellBackgroundColor = new Dictionary<SudokuCellBackgroundState, Color>()
    {
         {SudokuCellBackgroundState.Normal, new Color32(255,255,255,255)},
         {SudokuCellBackgroundState.Selected, new Color32(100,180,255,255)},
         {SudokuCellBackgroundState.Related, new Color32(180,200,255,255)},
         {SudokuCellBackgroundState.Warning, new Color32(255,150,150,255)},
    };
    public static readonly Dictionary<SudokuCellTextState, Color> cellTextFontColor = new Dictionary<SudokuCellTextState, Color>()
    {
        {SudokuCellTextState.Default, new Color32(0,0,0,255)},
        {SudokuCellTextState.Valid, new Color32(0,35,255,255)},
        {SudokuCellTextState.Invalid, new Color32(150,0,0,255)},
    };
}


