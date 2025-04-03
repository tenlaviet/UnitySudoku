using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public enum SudokuCellState
{
    Normal = 0,
    Related,
    Warning,
    Selected,
}

[Serializable]
public class CellData
{
    public int i;
    public int j;
    public int _value;
    public bool isValueValid;
    public bool isEditable;
    public SudokuCellState _state;
    public bool test;

}
public class SudokuCell : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public Button button;
    public Color32 _backgroundColor;
    public Color32 _textColor;
    public CellData data;
    public void SetPosition(int i, int j)
    {
        this.data.i = i;
        this.data.j = j;
    }
    public void SetValue(int value)
    {
        
        this.data._value = value;
        this.data.isValueValid = true;
        if ( 0<value && value <=9)
        {
            textComponent.text = value.ToString();            
        }
        else
        {
            textComponent.text = "";
        }

    }
    
    public void SetCellState(SudokuCellState state)
    {
        this.data._state = state;
        this.button.image.color = Data.cellBackgroundColor[this.data._state];
    }
    public void EmptyCell()
    {
        
        this.data._value = 0;
        textComponent.text = "";
    }

    public void OnClick()
    {
        SudokuBoard.Instance.SelectCell(this);
    }
    
    
    
}
