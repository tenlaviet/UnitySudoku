using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Difficulty
{
    Easy,
    Medium,
    Hard
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

// {
//     "data":[
//     {
//         "difficulty": 0,
//         "puzzle": [
//         2,7,6,0,1,3,0,0,0,
//         0,4,0,8,7,0,0,0,0,
//         0,0,8,6,9,0,7,0,2,
//         0,5,1,0,6,9,0,3,0,
//         0,0,0,0,0,1,5,4,0,
//         0,8,0,7,3,0,0,9,0,
//         5,1,0,0,2,6,8,0,3,
//         3,6,7,1,0,0,9,0,0,
//         0,0,0,0,4,0,0,6,0
//             ]
//     },
//     {
//         "difficulty": 1,
//         "puzzle":
//         [
//         5,8,0,1,0,2,0,4,0,
//         0,0,0,0,0,0,2,1,6,
//         9,1,0,7,0,4,0,0,3,
//         0,0,0,0,2,0,0,0,0,
//         2,0,3,9,1,0,7,0,0,
//         7,9,0,0,0,3,4,0,5,
//         6,0,0,0,0,1,5,0,4,
//         1,5,0,0,4,0,6,7,2,
//         3,0,0,2,0,0,0,8,9
//             ]
//     }
//
//     ]
// }
