using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Data
{
    public static readonly Dictionary<SudokuCellState, Color> cellBackgroundColor = new Dictionary<SudokuCellState, Color>()
    {
         {SudokuCellState.Normal, new Color32(255,255,255,255)},
         {SudokuCellState.Selected, new Color32(100,180,255,255)},
         {SudokuCellState.Related, new Color32(180,200,255,255)},
         {SudokuCellState.Warning, new Color32(255,150,150,255)},
    };
    public static readonly Dictionary<bool, Color> cellTextFontColor = new Dictionary<bool, Color>()
    {
        {true, new Color32(0,0,0,255)},
        {false, new Color32(150,0,0,255)}
    };
}
