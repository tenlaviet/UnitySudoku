using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public struct CellData
{
    public int i;
    public int j;
    public int value;
    public SudokuCellBackgroundState backgroundState;
    public SudokuCellTextState textState;
}
public class SudokuCell : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public Button button;
    public bool isValueValid;
    public bool isEditable;
    public bool isSelected;
    public CellData data;
    public void SetPosition(int i, int j)
    {
        this.data.i = i;
        this.data.j = j;
    }

    public int GetValue()
    {
        return data.value;
    }
    public void SetCellValue(int value, bool isEditable)
    {
        
        this.data.value = value;
        this.isEditable = isEditable;
        UpdateCell();
    }
    
    public void SetCellState(SudokuCellBackgroundState backgroundState, SudokuCellTextState textState)
    {

        this.data.textState = textState;
        this.data.backgroundState = backgroundState;
        UpdateCell();
    }

    public void SetCellDefault()
    {
        this.isValueValid = true;
        this.isEditable = false;
        this.isSelected = false;
        this.data.backgroundState = 0;
        this.data.textState = 0;
        this.data.value = 0;
        UpdateCell();

    }

    public void EraseCellValue()
    {
        this.isValueValid = true;
        this.data.textState = 0;
        this.data.value = 0;
        textComponent.text = "";
        UpdateCell();
    }

    public void UpdateCell()
    {
        this.textComponent.color = Data.cellTextFontColor[this.data.textState];
        this.button.image.color = Data.cellBackgroundColor[this.data.backgroundState];//background change
        // if (this.isSelected == true)
        // {
        //     this.button.image.color = Data.cellBackgroundColor[SudokuCellBackgroundState.Selected];//background change
        // }
        if ( 0< this.data.value && this.data.value <=9)
        {
            textComponent.text = this.data.value.ToString();            
        }
        else
        {
            textComponent.text = "";
        }
    }

    public void OnClick()
    {
        SudokuBoard.Instance.SelectCell(this);
    }
    
    
    
}
