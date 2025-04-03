using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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

[Serializable]
public class CellData
{
    public int i;
    public int j;
    public int value;
    public bool isValueValid;
    public bool isEditable;
    public SudokuCellBackgroundState backgroundState;
    public SudokuCellTextState textState;
}
public class SudokuCell : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public Button button;
    public CellData data;
    public void SetPosition(int i, int j)
    {
        this.data.i = i;
        this.data.j = j;
    }
    public void SetValue(int value, bool isValueValid, SudokuCellTextState textState)
    {
        
        this.data.value = value;
        this.data.isValueValid = isValueValid;
        this.data.textState = textState;
        this.textComponent.color = Data.cellTextFontColor[this.data.textState];
        if ( 0<value && value <=9)
        {
            textComponent.text = value.ToString();            
        }
        else
        {
            textComponent.text = "";
        }
    }
    
    public void SetBackgroundState(SudokuCellBackgroundState backgroundState)
    {
        this.data.backgroundState = backgroundState;
        this.button.image.color = Data.cellBackgroundColor[this.data.backgroundState];//background change
    }

    public void EmptyCell()
    {
        this.data.value = 0;
        textComponent.text = "";
    }

    public void OnClick()
    {
        SudokuBoard.Instance.SelectCell(this);
    }
    
    
    
}
